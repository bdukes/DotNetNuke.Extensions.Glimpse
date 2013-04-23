using System;
using System.Linq;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;

using Glimpse.AspNet.Extensibility;
using Glimpse.Core.Extensibility;

namespace DotNetNuke.Extensions.Glimpse
{
    public class DNNModuleGlimpsePlugin : AspNetTab
    {
        public override string Name
        {
            get { return "DNN Modules"; }
        }

        public override object GetData(ITabContext context)
        {
            try
            {
                if (PortalSettings.Current.ActiveTab.TabID <= 0)
                {
                    return null;
                }

                return from module in new ModuleController().GetTabModules(PortalSettings.Current.ActiveTab.TabID).Values
                       select new
                                  {
                                      Title = module.ModuleTitle ?? module.DesktopModule.FriendlyName,
                                      module.ModuleID,
                                      module.AllTabs,
                                      module.CacheTime,
                                      module.ContainerPath,
                                      module.ContainerSrc,
                                      module.Header,
                                      module.Footer,
                                      module.InheritViewPermissions,
                                      module.DesktopModule.IsPremium,
                                      module.ModuleControl.ControlKey,
                                      module.ModuleControl.ControlSrc,
                                      module.DesktopModule.Permissions, // TODO: is this a reasonable value to show?
                                      module.PaneName,
                                      module.StartDate,
                                      module.EndDate,
                                      module.ModuleControl.SupportsPartialRendering,
                                      module.ModuleSettings,
                                  };
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return "There was an error loading the data for this tab";
            }
        }
    }
}