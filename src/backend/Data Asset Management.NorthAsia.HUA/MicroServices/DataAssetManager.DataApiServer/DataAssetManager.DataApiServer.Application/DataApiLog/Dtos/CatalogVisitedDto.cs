using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    public class CatalogVisitedDto
    {

        public string CtlId { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public string Visited { get; set; }
    }

}