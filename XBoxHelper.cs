using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Playnite.SDK;
using Playnite.SDK.Data;
using System.Threading;
using Playnite.SDK.Models;
using System.Net;
using System.Text.RegularExpressions;

namespace XCloudLibrary
{
    static class XBoxHelper
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private const string allGamesDataApiBaseUrl = @"https://catalog.gamepass.com/sigls/v2?id=29a81209-df6f-41fd-a528-2ae6b91f719c&market={0}&language={1}";
        private const string catalogDataApiBaseUrl = @"https://displaycatalog.mp.microsoft.com/v7.0/products?bigIds={0}&market={1}&languages={2}&MS-CV=F.1";
        private const string playBaseUrl = @"{0}https://www.xbox.com/{1}/play/launch/{2}/{3}";
        /*
          https://www.xbox.com/{0}/play/launch/{1}/{2}
          https://account.xbox.com/account/signin?returnUrl=https://www.xbox.com/{}/play/launch/{1}/{2}
          https://login.live.com/login.srf?wa=wsignin1.0&wp=MBI_SSL&wreply=https:%2f%2faccount.xbox.com%2f{0}%2faccountcreation%3freturnUrl%3dhttps%253a%252f%252fwww.xbox.com%252f{0}%252fplay%252flaunch%252f{1}%252f{2}&aadredir=1
         */

        public static readonly string XcloudPlatformName = "Microsoft X Cloud";
        public static readonly string XCloudGameidTag = "XCloudGame__";

        public static string SanitazieProductTitleUrl(string producttitle)
        {
            string tmp = producttitle.ToLower().
                Replace(" ", "-").
                Replace("™", "").
                Replace(":", "").
                Replace("®", "").
                Replace("(", "").
                Replace(")", "").
                Replace("!", "").
                Replace("?", ""). //assumptions starting from here
                Replace("©", "").
                Replace(".", "");

            if (tmp.Length > 50)
            {
                tmp = tmp.Substring(0, 50);
            }
            return tmp;
        }

        //based on code from normalizegamename from playnite common
        private static string SanitazieProductTitle(string producttitle)
        {
            string newName = producttitle;
            if (string.IsNullOrEmpty(newName))
            {
                return newName;
            }
            newName = newName.
                Replace("™", "").
                Replace("®", "").
                Replace("©", "").
                Replace("_", " ").
                Replace(".", " ").
                Replace('’', '\'');
            newName = Regex.Replace(newName, @"\[.*?\]", "");
            newName = Regex.Replace(newName, @"\(.*?\)", "");
            newName = Regex.Replace(newName, @"\s*:\s*", ": ");
            newName = Regex.Replace(newName, @"\s+", " ");
            if (Regex.IsMatch(newName, @",\s*The$"))
            {
                newName = "The " + Regex.Replace(newName, @",\s*The$", "", RegexOptions.IgnoreCase);
            }

            return newName.Trim();
        }

        // Taken from darklink's gamepass browser extension
        private static string[] companiesStringToArray(string companiesString)
        {
            var companiesList = new List<string>();

            // Replace ", Inc" for ". Inc" and other terms so it doesn't conflict in the split operation
            companiesString = companiesString.
                Replace("Developed by ", "").
                Replace(", Inc", ". Inc").
                Replace(", INC", ". INC").
                Replace(", inc", ". inc").
                Replace(", Llc", ". Llc").
                Replace(", LLC", ". LLC").
                Replace(", Ltd", ". Ltd").
                Replace(", LTD", ". LTD");

            string[] stringSeparators = new string[] { ", ", "|", "/", "+", " and ", " & " };
            var splitArray = companiesString.Split(stringSeparators, StringSplitOptions.None);
            foreach (string splittedString in splitArray)
            {
                companiesList.Add(splittedString.
                    Replace(". Inc", ", Inc").
                    Replace(". INC", ", INC").
                    Replace(". inc", ", inc").
                    Replace(". Llc", ", Llc").
                    Replace(". LLC", ", LLC").
                    Replace(". Ltd", ", Ltd").
                    Replace(". LTD", ", LTD").
                    Trim());
            }

            return companiesList.ToArray();
        }

        static ImagePurpose GetImagePurpose(string SizeValue, bool PreferTitleBackground, bool Backup = false)
        {
            string[] tmp = SizeValue.Split('|');
            if (tmp.Length == 3)
            {
                switch (tmp[2])
                {
                    case "P":
                        if (Backup)
                        {
                            return ImagePurpose.BoxArt;
                        }
                        else
                        {
                            return ImagePurpose.Poster;
                        }
                    case "S":
                        if (Backup)
                        {
                            return ImagePurpose.Poster;
                        }
                        else
                        {
                            return ImagePurpose.BoxArt;
                        }
                    case "B":
                        if (PreferTitleBackground)
                        {
                            if (Backup)
                            {
                                return ImagePurpose.SuperHeroArt;
                            }
                            else
                            {
                                return ImagePurpose.TitledHeroArt;
                            }
                        }
                        else
                        {
                            if (Backup)
                            {
                                return ImagePurpose.TitledHeroArt;
                            }
                            else
                            {
                                return ImagePurpose.SuperHeroArt;
                            }
                        }

                    default:
                        break;
                }
            }
            return ImagePurpose.BoxArt;
        }

        static string GetImageWidth(string SizeValue)
        {
            var tmp = SizeValue.Split('|');
            if (tmp.Length == 3)
            {
                return tmp[0];
            }
            return "128";
        }

        static string GetImageHeight(string SizeValue)
        {
            var tmp = SizeValue.Split('|');
            if (tmp.Length == 3)
            {
                return tmp[1];
            }
            return "128";
        }

       
        public static List<XCloudGame> GetXCloudGamesForIDs(string IDs, string Region, string Language, XCloudLibrarySettings Settings)
        {
            List<XCloudGame> result = new List<XCloudGame>();
            string catalogDataApiUrl = string.Format(catalogDataApiBaseUrl, IDs, Region.Substring(3, 2), Language);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var catalogresponse = client.GetAsync(catalogDataApiUrl);
                    var catalogcontents = catalogresponse.Result.Content.ReadAsStringAsync();
                    if (catalogresponse.Status == TaskStatus.RanToCompletion)
                    {
                        CatalogData data = Serialization.FromJson<CatalogData>(catalogcontents.Result);
                        if ((data.Products != null) && (data.Products.Length > 0))
                        {
                            foreach (CatalogProduct product in data.Products)
                            {
                                if (product.ProductBSchema == "ProductAddOn;3")
                                {
                                    continue;
                                }

                                if ((product.LocalizedProperties != null) &&
                                    (product.LocalizedProperties.Length > 0))
                                {
                                    try
                                    {
                                        XCloudGame xgame = new XCloudGame
                                        {
                                            Category = product.Properties.Category,
                                            Categories = product.Properties.Categories,
                                            CoverImageUrl = string.Format("https:{0}?mode=scale&q=90&h=900&w=600", product.LocalizedProperties[0].Images?.Where(x => x.ImagePurpose == ImagePurpose.Poster)?.FirstOrDefault()?.Uri),
                                            Description = product.LocalizedProperties[0].ProductDescription,
                                            Name = SanitazieProductTitle(product.LocalizedProperties[0].ProductTitle),
                                            GameId = XCloudGameidTag + product.ProductId,
                                            Publishers = companiesStringToArray(product.LocalizedProperties[0].PublisherName),
                                            ReleaseDate = product.MarketProperties.FirstOrDefault().OriginalReleaseDate.UtcDateTime,
                                            OriginalProductTitle = product.LocalizedProperties[0].ProductTitle
                                        };

                                        ImagePurpose BackgroundImagePurpose = GetImagePurpose(Settings.BackgroundSize, Settings.PreferTitledBackground, false);
                                        string BackgroundImageWidth = GetImageWidth(Settings.BackgroundSize);
                                        string BackgroundImageHeight = GetImageHeight(Settings.BackgroundSize);
                                        if (product.LocalizedProperties[0].Images.Any(x => x.ImagePurpose == BackgroundImagePurpose))
                                        {
                                            xgame.BackgroundImageUrl = string.Format("https:{0}?mode=scale&q=90&w={1}&h={2}", product.LocalizedProperties[0].Images?.Where(
                                                x => x.ImagePurpose == BackgroundImagePurpose)?.FirstOrDefault()?.Uri, BackgroundImageWidth, BackgroundImageHeight);
                                        }
                                        else
                                        {
                                            BackgroundImagePurpose = GetImagePurpose(Settings.BackgroundSize, Settings.PreferTitledBackground, true);
                                            if (product.LocalizedProperties[0].Images.Any(x => x.ImagePurpose == BackgroundImagePurpose))
                                            {
                                                xgame.BackgroundImageUrl = string.Format("https:{0}?mode=scale&q=90&w={1}&h={2}", product.LocalizedProperties[0].Images?.Where(
                                                    x => x.ImagePurpose == BackgroundImagePurpose)?.FirstOrDefault()?.Uri, BackgroundImageWidth, BackgroundImageHeight);
                                            }
                                        }

                                        ImagePurpose CoverImagePurpose = GetImagePurpose(Settings.CoverSize, Settings.PreferTitledBackground, false);
                                        string CoverImageWidth = GetImageWidth(Settings.CoverSize);
                                        string CoverImageHeight = GetImageHeight(Settings.CoverSize);
                                        if (product.LocalizedProperties[0].Images.Any(x => x.ImagePurpose == CoverImagePurpose))
                                        {
                                            xgame.CoverImageUrl = string.Format("https:{0}?mode=scale&q=90&w={1}&h={2}", product.LocalizedProperties[0].Images?.Where(
                                                x => x.ImagePurpose == CoverImagePurpose)?.FirstOrDefault()?.Uri, CoverImageWidth, CoverImageHeight);
                                        }
                                        else
                                        {
                                            CoverImagePurpose = GetImagePurpose(Settings.CoverSize, Settings.PreferTitledBackground, true);
                                            if (product.LocalizedProperties[0].Images.Any(x => x.ImagePurpose == CoverImagePurpose))
                                            {
                                                xgame.CoverImageUrl = string.Format("https:{0}?mode=scale&q=90&w={1}&h={2}", product.LocalizedProperties[0].Images?.Where(
                                                    x => x.ImagePurpose == CoverImagePurpose)?.FirstOrDefault()?.Uri, CoverImageWidth, CoverImageHeight);
                                            }
                                        }

                                        ImagePurpose IconImagePurpose = GetImagePurpose(Settings.IconSize, Settings.PreferTitledBackground, false);
                                        string IconImageWidth = GetImageWidth(Settings.IconSize);
                                        string IconImageHeight = GetImageHeight(Settings.IconSize);

                                        if (product.LocalizedProperties[0].Images.Any(x => x.ImagePurpose == IconImagePurpose))
                                        {
                                            xgame.IconUrl = string.Format("https:{0}?mode=scale&q=90&w={1}&h={2}", product.LocalizedProperties[0].Images?.Where(
                                                x => x.ImagePurpose == IconImagePurpose)?.FirstOrDefault()?.Uri, IconImageWidth, IconImageHeight);
                                        }
                                        else
                                        {
                                            IconImagePurpose = GetImagePurpose(Settings.IconSize, Settings.PreferTitledBackground, true);
                                            if (product.LocalizedProperties[0].Images.Any(x => x.ImagePurpose == IconImagePurpose))
                                            {
                                                xgame.IconUrl = string.Format("https:{0}?mode=scale&q=90&w={1}&h={2}", product.LocalizedProperties[0].Images?.Where(
                                                    x => x.ImagePurpose == IconImagePurpose)?.FirstOrDefault()?.Uri, IconImageWidth, IconImageHeight);
                                            }
                                        }

                                        if (string.IsNullOrEmpty(product.LocalizedProperties[0].DeveloperName))
                                        {
                                            xgame.Developers = xgame.Publishers;
                                        }
                                        else
                                        {
                                            xgame.Developers = companiesStringToArray(product.LocalizedProperties[0].DeveloperName);
                                        }
                                        result.Add(xgame);
                                    }
                                    catch (Exception e)
                                    {
                                        logger.Error(e, "Failure getting game data for productid: " + product.ProductId);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Failure getting data " + catalogDataApiUrl);
            }
            return result;
    }

        public static List<XCloudGame> GetXCloudGamesList(IPlayniteAPI PlayniteApi, string Region, string Language, XCloudLibrarySettings Settings, CancellationToken CancelToken)
        {
            using (HttpClient client = new HttpClient())
            {
                string bigIDs = string.Empty;
                string allGamesDataApiUrl = string.Format(allGamesDataApiBaseUrl, Region.Substring(3,2), Language);
                try
                {
                    var response = client.GetAsync(allGamesDataApiUrl);
                    var contents = response.Result.Content.ReadAsStringAsync();
                    if (response.Status == TaskStatus.RanToCompletion)
                    {
                        List<GameID> gameIDS = Serialization.FromJson<List<GameID>>(contents.Result);
                        foreach (GameID id in gameIDS.GetRange(1, gameIDS.Count - 1))
                        {
                            if (CancelToken.IsCancellationRequested)
                            {
                                break;
                            }
                            if (!string.IsNullOrEmpty(bigIDs))
                            {
                                bigIDs += ",";
                            }
                            bigIDs += id.id;
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, "Failure getting data " + allGamesDataApiUrl);
                }

                if (!CancelToken.IsCancellationRequested)
                {
                    if (!string.IsNullOrEmpty(bigIDs))
                    {
                        return GetXCloudGamesForIDs(bigIDs, Region, Language, Settings);
                    }
                }
            }
            return null;
        }

        // Taken from darklink's gamepass browser extension
        private static string StringToHtml(string s, bool nofollow)
        {
            s = WebUtility.HtmlEncode(s);
            string[] paragraphs = s.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
            StringBuilder sb = new StringBuilder();
            foreach (string par in paragraphs)
            {
                sb.AppendLine("<p>");
                string p = par.Replace(Environment.NewLine, "<br />\r\n");
                if (nofollow)
                {
                    p = Regex.Replace(p, @"\[\[(.+)\]\[(.+)\]\]", "<a href=\"$2\" rel=\"nofollow\">$1</a>");
                    p = Regex.Replace(p, @"\[\[(.+)\]\]", "<a href=\"$1\" rel=\"nofollow\">$1</a>");
                }
                else
                {
                    p = Regex.Replace(p, @"\[\[(.+)\]\[(.+)\]\]", "<a href=\"$2\">$1</a>");
                    p = Regex.Replace(p, @"\[\[(.+)\]\]", "<a href=\"$1\">$1</a>");
                    sb.AppendLine(p);
                }
                sb.AppendLine(p);
                sb.AppendLine("</p>");
            }
            return sb.ToString();
        }

        private static HashSet<MetadataProperty> arrayToCompanyMetadataProperty(IPlayniteAPI PlayniteApi, string[] array)
        {
            var list = new HashSet<MetadataProperty>();
            foreach (string str in array)
            {
                var company = PlayniteApi.Database.Companies.Add(str);
                list.Add(new MetadataIdProperty(company.Id));
            }

            return list;
        }

        public static GameMetadata ConvertXCloudGameToMetadata(IPlayniteAPI PlayniteApi, XCloudGame xgame, Guid pluginid, bool basicdata = true, bool SetGameAsInstalled = true)
        {
            try
            {
                var newGame = new GameMetadata();
                string[] platformarray = { XcloudPlatformName };
                newGame.Platforms = new HashSet<MetadataProperty> { new MetadataNameProperty(XcloudPlatformName) };
                newGame.Source = new MetadataNameProperty("XCloud");
                newGame.Name = xgame.Name;
                newGame.IsInstalled = SetGameAsInstalled;
                newGame.GameId = xgame.GameId;
                if (!basicdata)
                {
                    newGame.Developers = arrayToCompanyMetadataProperty(PlayniteApi, xgame.Developers);
                    newGame.Publishers = arrayToCompanyMetadataProperty(PlayniteApi, xgame.Publishers);
                    newGame.Description = StringToHtml(xgame.Description, true);
                    newGame.ReleaseDate = new ReleaseDate(xgame.ReleaseDate);
                    newGame.Icon = new MetadataFile(xgame.IconUrl);
                    newGame.CoverImage = new MetadataFile(xgame.CoverImageUrl);
                    newGame.BackgroundImage = new MetadataFile(xgame.BackgroundImageUrl);
                }
                return newGame;
            }
            catch (Exception E)
            {
                logger.Error(E, "Error during converting of game with productID " + xgame.GameId);
                return null;
            }
        }

        public static string GetPlayUrlFromGame(Game game, XCloudLibrarySettings Settings)
        {
            if (game.GameId.StartsWith(XCloudGameidTag))
            {
                string productid = GetProductIdFromGame(game);
                //we need to grab the producttitle again in the region from the market and not the region from where
                //we gotten the gamedata as the urls uses parts of the localized producttitle
                //so only way is to regrab them
                List<XCloudGame> foundgames = GetXCloudGamesForIDs(productid, Settings.Region, Settings.Region, Settings);
                if(foundgames.Count == 1)
                {
                    return string.Format(playBaseUrl, Settings.PrependUrl, Settings.Region, SanitazieProductTitleUrl(foundgames[0].OriginalProductTitle), productid);
                }
            }
            return string.Empty;
        }

        public static string GetProductIdFromGame(Game game)
        {
            return game.GameId.Replace(XCloudGameidTag, "") ;
        }

    }
}
