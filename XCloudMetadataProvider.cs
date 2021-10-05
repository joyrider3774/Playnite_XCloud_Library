using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCloudLibrary
{
    public class XCloudMetadataProvider : LibraryMetadataProvider
    {
        private XCloudLibrary plugin;

        public XCloudMetadataProvider(XCloudLibrary plugin)
        {
            this.plugin = plugin;
        }
        
        public override GameMetadata GetMetadata(Game game)
        {
            var Productid = XBoxHelper.GetProductIdFromGame(game);
            List<XCloudGame> games = XBoxHelper.GetXCloudGamesForIDs(Productid, plugin.settings.Settings.Region, plugin.settings.Settings.Language, plugin.settings.Settings);
            
            if (games.Count != 1)
            {
                return null;
            }

            GameMetadata gameInfo = XBoxHelper.ConvertXCloudGameToMetadata(plugin.PlayniteApi, games[0], plugin.Id, false);

            if (!string.IsNullOrEmpty(games[0].IconUrl))
            {
                gameInfo.Icon = new MetadataFile(games[0].IconUrl);
            }

            if (!string.IsNullOrEmpty(games[0].CoverImageUrl))
            {
                gameInfo.CoverImage = new MetadataFile(games[0].CoverImageUrl);
            }

            if (!string.IsNullOrEmpty(games[0].BackgroundImageUrl))
            {
                gameInfo.BackgroundImage = new MetadataFile(games[0].BackgroundImageUrl);
            }

            return gameInfo;
        }
    }
}
