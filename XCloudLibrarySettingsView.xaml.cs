using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XCloudLibrary
{
    public class ImageTypeSize
    {
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public string Res { get; set; } = "";       
    }

        public partial class XCloudLibrarySettingsView : UserControl
    {
        XCloudLibrary plugin;

       
        private string[] languages = {
            "es-AR", "pt-BR", "en-CA", "fr-CA", "es-CL", "es-CO", "es-MX", "en-US", "nl-BE", "fr-BE", "cs-CZ", "da-DK", "de-DE", "es-ES", "fr-FR", "en-IE", "it-IT",
            "hu-HU", "nl-NL", "nb-NO", "de-AT", "pl-PL", "pt-PT", "de-CH", "sk-SK", "fr-CH", "fi-FI", "sv-SE", "en-GB", "el-GR", "ru-RU", "en-AU", "en-HK", "en-IN",
            "en-NZ", "en-SG", "ko-KR", "zh-CN", "zh-TW", "ja-JP", "zh-HK", "en-ZA", "tr-TR", "he-IL", "ar-AE", "ar-SA"
        };
        private Dictionary<string, string> languagesDict;
        private Dictionary<string, string> regionsDict;
        private List<ImageTypeSize> iconSizesList = new List<ImageTypeSize>();
        private List<ImageTypeSize> coverSizesList = new List<ImageTypeSize>();
        private List<ImageTypeSize> backgroundSizesList = new List<ImageTypeSize>();

        public XCloudLibrarySettingsView(XCloudLibrary plugin)
        {
            this.plugin = plugin;

            var tmpDict = new Dictionary<string, string>();
            foreach (string languageCode in languages)
            {
                CultureInfo cultureInfo = new CultureInfo(languageCode);
                tmpDict.Add(languageCode, cultureInfo.NativeName);
            }
            languagesDict = tmpDict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            tmpDict.Clear();
            foreach (string regionCode in languages)
            {
                CultureInfo cultureInfo = new CultureInfo(regionCode);
                RegionInfo regioninfo = new RegionInfo(cultureInfo.LCID);
                tmpDict.Add(regionCode, regioninfo.NativeName + " (" + cultureInfo.TwoLetterISOLanguageName + ")");
            }
            regionsDict = tmpDict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            //icon sizes   
            for (int i = 32; i <= 512; i *= 2)
            {
                iconSizesList.Add(new ImageTypeSize
                {
                    ID = string.Format("{0}|{1}|S", i, i),
                    Res = string.Format("{0}x{1}", i, i),
                    Name = "Icon ",
                  });
            };

            //cover sizes
            coverSizesList.Add(new ImageTypeSize
            {
                ID = "342|482|P",
                Res = "342x482",
                Name = "Poster ",
            });

            coverSizesList.Add(new ImageTypeSize
            {
                ID = "660|930|P",
                Res = "660x930",
                Name = "Poster ",
            });

            coverSizesList.Add(new ImageTypeSize
            {
                ID = "600|900|P",
                Res = "600x900",
                Name = "Poster ",
            });

            for (int i = 128; i <= 1024; i *= 2)
            {

                coverSizesList.Add(new ImageTypeSize
                {
                    ID = string.Format("{0}|{1}|S", i, i),
                    Res = string.Format("{0}x{1}", i, i),
                    Name = "Square ",
                });
            };


            backgroundSizesList.Add(new ImageTypeSize
            {
                ID = "960|540|B",
                Res = "960x540",
                Name = "qHD ",
            });

            backgroundSizesList.Add(new ImageTypeSize
            {
                ID = "1024|576|B",
                Res = "1024x576",
                Name = "PAL 16:9 ",
            });

            backgroundSizesList.Add(new ImageTypeSize
            {
                ID = "1280|720|B",
                Res = "1280x720",
                Name = "WXGA-H (720p) ",
            });

            backgroundSizesList.Add(new ImageTypeSize
            {
                ID = "1366|768|B",
                Res = "1366x768",
                Name = "HD Ready ",
            });

            backgroundSizesList.Add(new ImageTypeSize
            {
                ID = "1600|900|B",
                Res = "1600x900",
                Name = "HD+ ",
            });

            backgroundSizesList.Add(new ImageTypeSize
            {
                ID = "1920|1080|B",
                Res = "1920x1080",
                Name = "Full HD (1080p) ",
            });

            backgroundSizesList.Add(new ImageTypeSize
            {
                ID = "2048|1152|B",
                Res = "2048x1152",
                Name = "QWXGA ",
            });

            backgroundSizesList.Add(new ImageTypeSize
            {
                ID = "2560|1440|B",
                Res = "2560x1440",
                Name = "WQHD (2K) ",
            });

            backgroundSizesList.Add(new ImageTypeSize
            {
                ID = "3200|1800|B",
                Res = "3200x1800",
                Name = "WQXGA+ (3K) ",
            });

            backgroundSizesList.Add(new ImageTypeSize
            {
                ID = "3840|2160|B",
                Res = "3840x2160",
                Name = "UHD-1 (4K) ",
            });

            InitializeComponent();

            cbRegions.ItemsSource = regionsDict;
            cbLanguages.ItemsSource = languagesDict;
            cbIconSize.ItemsSource = iconSizesList;
            cbCoverSize.ItemsSource = coverSizesList;
            cbBackgroundSize.ItemsSource = backgroundSizesList;
        }

        private void SelectBrowser_Click(object sender, RoutedEventArgs e)
        {
            TbBrowserPath.Text = plugin.PlayniteApi.Dialogs.SelectFile("Exe Files|*.exe");
        }
    }
}
