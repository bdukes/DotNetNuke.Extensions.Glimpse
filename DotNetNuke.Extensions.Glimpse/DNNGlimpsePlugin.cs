using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using Glimpse.AspNet.Extensibility;
using Glimpse.Core.Extensibility;

namespace DotNetNuke.Extensions.Glimpse
{
    /// <summary>DotNetNuke Glimpse Plugin</summary>
    public class DNNGlimpsePlugin : AspNetTab
    {
        public override string Name
        {
            get { return "DotNetNuke"; }
        }

        /// <summary>
        /// Gets the data to send to the Glimpse client.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Data to send the the Glimpse client.</returns>
        public override object GetData(ITabContext context)
        {
            try
            {
                var portalSettings = PortalSettings.Current;
                var tabCreatedByUser = UserController.GetUserById(-1, portalSettings.ActiveTab.CreatedByUserID);
                var tabModifiedByUser = UserController.GetUserById(-1, portalSettings.ActiveTab.LastModifiedByUserID);
                var portalAliases = from PortalAliasInfo pa in new PortalAliasController().GetPortalAliasArrayByPortalID(portalSettings.PortalId)
                                    select pa.HTTPAlias;

                var httpContext = context.GetRequestContext<HttpContextBase>();
                var contextItems = (from string key in httpContext.Items.Keys 
                                    select new[] { key, httpContext.Items[key] }).ToList();

                return new List<object[]>
                           {
                               new object[] { "Property", "Value" },
                               new object[] { "Portal ID", portalSettings.PortalId },
                               new object[] { "Portal Name", portalSettings.PortalName },
                               new object[] { "Portal Aliases", portalAliases.ToArray() },
                               new object[] { "Portal SSL Enabled", portalSettings.SSLEnabled },
                               new object[] { "Portal SSL Enforced", portalSettings.SSLEnforced },
                               new object[] { "User ID", portalSettings.UserId },
                               new object[] { "User Name", portalSettings.UserInfo.Username },
                               new object[] { "User Roles", portalSettings.UserInfo.Roles },
                               new object[] { "Tab ID", portalSettings.ActiveTab.TabID },
                               new object[] { "Tab Name", portalSettings.ActiveTab.TabName },
                               new object[] { "Tab Title", portalSettings.ActiveTab.Title },
                               new object[] { "Tab Path", portalSettings.ActiveTab.TabPath },
                               new object[] { "Tab SSL Enabled", portalSettings.ActiveTab.IsSecure },
                               new object[] { "Tab Created By", (tabCreatedByUser == null) ? null : tabCreatedByUser.Username },
                               new object[] { "Tab Created Date", portalSettings.ActiveTab.CreatedOnDate },
                               new object[] { "Tab Modified By", (tabModifiedByUser == null) ? null : tabModifiedByUser.Username },
                               new object[] { "Tab Modified Date", portalSettings.ActiveTab.LastModifiedOnDate },
                               new object[] { "Tab Skin Path", portalSettings.ActiveTab.SkinPath },
                               new object[] { "Tab Skin Source", portalSettings.ActiveTab.SkinSrc },
                               new object[] { "Context Items", contextItems },
                           };
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return ex.Message + ex.StackTrace;
            }
        }
    }
}