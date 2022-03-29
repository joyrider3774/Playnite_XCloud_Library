using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XCloudLibrary
{
    public class XCloudLibrarySettings
    {
        public string BrowserPath { get; set; } = string.Empty;
        public bool UseKioskMode { get; set; } = false;
        public bool UseDataDir { get; set; } = false;
        public string ExtraParameters { get; set; } = string.Empty;
        public string Region { get; set; } = "en-US";
        public string Language { get; set; } = "en-US";
        public bool NotifyAdditions { get; set; } = false;
        public bool NotifyRemovals { get; set; } = false;
        public bool FirstTimeSetupDone { get; set; } = false;
        public string PrependUrl { get; set; } = string.Empty;
        public string IconSize { get; set; } = "128|128|S";
        public string CoverSize { get; set; } = "600|900|P";
        public string BackgroundSize { get; set; } = "1280|720|B";
        public bool PreferTitledBackground { get; set; } = false;
        public bool SetGamesAsInstalled { get; set; } = true;

        [DontSerialize]
        public string buildParameters(string url, string BrowserDataPath)
        {
            string Arguments = ExtraParameters;
            
            if (UseDataDir)
            {
                if (!Directory.Exists(BrowserDataPath))
                {
                    Directory.CreateDirectory(BrowserDataPath);
                }

                if (Arguments != string.Empty)
                {
                    Arguments += " ";
                }
                Arguments += "--user-data-dir=\"" + BrowserDataPath + "\"";
            }

            if (UseKioskMode)
            {
                if (Arguments != string.Empty)
                {
                    Arguments += " ";
                }
                Arguments += "--kiosk";
            }

            if (Arguments != string.Empty)
            {
                Arguments += " ";
            }

            Arguments += url;

            return Arguments;
        }
    }

    public class XCloudLibrarySettingsViewModel : ObservableObject, ISettings
    {
        private string PreviousBrowserPath = string.Empty;
        private readonly XCloudLibrary plugin;
        private XCloudLibrarySettings editingClone { get; set; }

        private XCloudLibrarySettings settings;
        public XCloudLibrarySettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public XCloudLibrarySettingsViewModel(XCloudLibrary plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<XCloudLibrarySettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new XCloudLibrarySettings();
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);
            PreviousBrowserPath = editingClone.BrowserPath;
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            Settings.FirstTimeSetupDone = true;
            plugin.SavePluginSettings(Settings);

            // When browser has changed need to remove browserdatapath otherwise it can cause problems
            if (!Settings.BrowserPath.ToLower().Equals(PreviousBrowserPath.ToLower()))
            {
                string datapath = Path.Combine(plugin.GetPluginUserDataPath(), "Browserdata");
                if (Directory.Exists(datapath))
                {
                    Directory.Delete(datapath, true);
                }
            }
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}