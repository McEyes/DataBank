using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    [SugarTable(TableName = "asset_api_daily_visited")]
    public class DayVisitedDto
    {

        public DateTime Daily { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string DailyStr
        {
            get { return Daily.ToString("yyyy-MM-dd"); }
        }

        public long Visited { get; set; }
    }

}