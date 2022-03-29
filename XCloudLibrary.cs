using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace XCloudLibrary
{
    public class XCloudLibrary : LibraryPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        
        public XCloudLibrarySettingsViewModel settings { get; set; }
        
        public override Guid Id { get; } = Guid.Parse("5befaf7a-a0af-4ce2-992c-bc048d94e71b");

        // Change to something more appropriate
        public override string Name => "XCloud";

        // Implementing Client adds ability to open it via special menu in playnite.
        public override LibraryClient Client { get; } = null;

        public XCloudLibrary(IPlayniteAPI api) : base(api)
        {
            string pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Localization.SetPluginLanguage(pluginFolder, api.ApplicationSettings.Language);
            settings = new XCloudLibrarySettingsViewModel(this);
            Properties = new LibraryPluginProperties
            {
                HasSettings = true,
                CanShutdownClient = false,
                HasCustomizedGameImport = true
            };
        }

        public override IEnumerable<Game> ImportGames(LibraryImportGamesArgs args)
        {           
            List<Game> result = new List<Game>();
            if (settings.Settings.FirstTimeSetupDone)
            {
                List<XCloudGame> xcloudgameslist = XBoxHelper.GetXCloudGamesList(PlayniteApi, settings.Settings.Region, settings.Settings.Language, settings.Settings, args.CancelToken);

                foreach (XCloudGame xgame in xcloudgameslist)
                {
                    if (args.CancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (PlayniteApi.ApplicationSettings.GetGameExcludedFromImport(xgame.GameId, Id))
                    {
                        continue;
                    }

                    var alreadyImported = PlayniteApi.Database.Games.FirstOrDefault(a => a.GameId == xgame.GameId && a.PluginId == Id);
                    if (alreadyImported == null)
                    {
                        GameMetadata newGame = XBoxHelper.ConvertXCloudGameToMetadata(PlayniteApi, xgame, Id, true, settings.Settings.SetGamesAsInstalled);
                        if (newGame != null)
                        {
                            result.Add(PlayniteApi.Database.ImportGame(newGame, this));
                            if (settings.Settings.NotifyAdditions)
                            {
                                PlayniteApi.Notifications.Add(new NotificationMessage(
                                    Guid.NewGuid().ToString(),
                                    string.Format(Constants.GameAddedText, newGame.Name),
                                    NotificationType.Info));
                            }
                        }
                    }
                }

                //need at least one game in case of errors when grabbing the games.
                if (xcloudgameslist.Count > 0)
                {
                    List<Game> currentxcloudgames = PlayniteApi.Database.Games.Where(o => o.PluginId.Equals(Id)).ToList();
                    foreach (Game currentxcloudgame in currentxcloudgames)
                    {
                        if (args.CancelToken.IsCancellationRequested)
                        {
                            break;
                        }
                        if (!xcloudgameslist.Any(o => o.GameId.Equals(currentxcloudgame.GameId)))
                        {
                            if (PlayniteApi.Database.Games.Remove(currentxcloudgame))
                            {
                                if (settings.Settings.NotifyRemovals)
                                {
                                    PlayniteApi.Notifications.Add(new NotificationMessage(
                                        Guid.NewGuid().ToString(),
                                        string.Format(Constants.GameRemovedText, currentxcloudgame.Name),
                                        NotificationType.Info));
                                }
                            }
                        }
                    }
                }
            }
            else
            {

                PlayniteApi.Notifications.Add(new NotificationMessage(
                    Guid.NewGuid().ToString(), Constants.SetupMessageText, NotificationType.Info));
            }
            return result;
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new XCloudLibrarySettingsView(this);
        }

        public override IEnumerable<PlayController> GetPlayActions(GetPlayActionsArgs args)
        {
            if (args.Game.PluginId != Id)
            {
                yield break;
            }

            if (string.IsNullOrEmpty(settings.Settings.BrowserPath) || !File.Exists(settings.Settings.BrowserPath))
            {
                PlayniteApi.Dialogs.ShowMessage(Constants.SetupBrowserMessageText);
                PlayniteApi.MainView.OpenPluginSettings(Id);
            }

            yield return new AutomaticPlayController(args.Game)
            {
                TrackingMode = TrackingMode.Process,
                Name = "Start using Browser",
                Path = settings.Settings.BrowserPath,
                Type = AutomaticPlayActionType.File,
                Arguments = settings.Settings.buildParameters(XBoxHelper.GetPlayUrlFromGame(args.Game, settings.Settings), Path.Combine(GetPluginUserDataPath(), "Browserdata")),
                WorkingDir = string.IsNullOrEmpty(settings.Settings.BrowserPath) ? string.Empty : Path.GetDirectoryName(settings.Settings.BrowserPath),
            };
        }

        public override IEnumerable<InstallController> GetInstallActions(GetInstallActionsArgs args)
        {
            if (args.Game.PluginId != Id)
            {
                yield break;
            }

            yield return new XCloudInstallController(args.Game);
        }

        public override IEnumerable<UninstallController> GetUninstallActions(GetUninstallActionsArgs args)
        {
            if (args.Game.PluginId != Id)
            {
                yield break;
            }

            yield return new XCloudUninstallController(args.Game);
        }

        public override LibraryMetadataProvider GetMetadataDownloader()
        {
            return new XCloudMetadataProvider(this);
        }
    }
}