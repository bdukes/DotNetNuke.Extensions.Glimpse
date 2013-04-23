
namespace DotNetNuke.Extensions.Glimpse
{
    using System;
    using System.Linq;

    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Log.EventLog;

    using global::Glimpse.AspNet.Extensibility;
    using global::Glimpse.Core.Extensibility;

    public class DNNLogViewerPlugin : AspNetTab
    {
        public override string Name
        {
            get { return "DNN Log"; }
        }

        public override object GetData(ITabContext context)
        {
            try
            {
                var totalRecords = 0;
                return from LogInfo log in new LogController().GetLog(PortalSettings.Current.PortalId, 15, 1, ref totalRecords)
                       select new
                                  {
                                      CreatedDate = log.LogCreateDate,
                                      LogType = log.LogTypeKey,
                                      Username = log.LogUserName,
                                      Content = log.LogProperties.Cast<LogDetailInfo>().ToDictionary(p => p.PropertyName, p => p.PropertyValue),
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