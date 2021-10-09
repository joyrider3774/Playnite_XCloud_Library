using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCloudLibrary
{
    static class Constants
    {
        private static IResourceProvider resources = new ResourceProvider();

        public static string GameAddedText = resources.GetString("LOC_XCLOUDLIBRARY_GameAdded");
        public static string GameRemovedText = resources.GetString("LOC_XCLOUDLIBRARY_GameRemoved");
        public static string SetupMessageText = resources.GetString("LOC_XCLOUDLIBRARY_SetupMessage");
        public static string SetupBrowserMessageText = resources.GetString("LOC_XCLOUDLIBRARY_SetupBrowserMessage");
        public static string IconText = resources.GetString("LOC_XCLOUDLIBRARY_Icon");
        public static string PosterText = resources.GetString("LOC_XCLOUDLIBRARY_Poster");
        public static string SquareText = resources.GetString("LOC_XCLOUDLIBRARY_Square");

    }
}
