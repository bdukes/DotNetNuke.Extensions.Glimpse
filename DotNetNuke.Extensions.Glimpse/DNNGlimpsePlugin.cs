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
            get { return "DNN"; }
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
                var portalAliases = from PortalAliasInfo pa in new PortalAliasController().GetPortalAliasArrayByPortalID(portalSettings.PortalId)
                                    select pa.HTTPAlias;
                var httpContext = context.GetRequestContext<HttpContextBase>();
                var contextItems = from string key in httpContext.Items.Keys
                                   let value = httpContext.Items[key]
                                   where !key.StartsWith("__Glimpse")
                                   orderby key
                                   select new { key, value = value == null ? null : value is IDictionary<string, string> ? value : value.ToString() };
                return new {
                    Portal = new {
                        ID = portalSettings.PortalId,
                        Name = portalSettings.PortalName,
                        Aliases = portalAliases.ToArray(),
                        SSL = new { 
                            Enabled = portalSettings.SSLEnabled,
                            Enforced = portalSettings.SSLEnforced,
                        },
                    },
                    User = new {
                        ID = portalSettings.UserId,
                        Username = portalSettings.UserInfo.Username,
                        Roles = portalSettings.UserInfo.Roles,
                    },
                    Tab = new {
                        ID = portalSettings.ActiveTab.TabID,
                        Name = portalSettings.ActiveTab.TabName,
                        Title = portalSettings.ActiveTab.Title,
                        Path = portalSettings.ActiveTab.TabPath,
                        Secure = portalSettings.ActiveTab.IsSecure,
                        SkinPath = portalSettings.ActiveTab.SkinPath,
                        SkinSource = portalSettings.ActiveTab.SkinSrc,
                    },
                    Context = contextItems.ToArray(),
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