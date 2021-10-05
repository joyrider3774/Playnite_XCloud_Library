using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCloudLibrary
{
    public class XCloudInstallController : InstallController
    {

        public XCloudInstallController(Game game) : base(game)
        {
            Name = "Install";
        }

        public override void Install(InstallActionArgs args)
        {
            InvokeOnInstalled(new GameInstalledEventArgs());
        }
    }

    public class XCloudUninstallController : UninstallController
    {

        public XCloudUninstallController(Game game) : base(game)
        {
            Name = "Uninstall";
        }

        public override void Uninstall(UninstallActionArgs args)
        {
            InvokeOnUninstalled(new GameUninstalledEventArgs());
        }
    }
}
