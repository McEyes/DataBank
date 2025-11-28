using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    public class ApiVisitedDto
    {

        public string ApiId { get; set; }

        public string ApiName { get; set; }

        public string Visited { get; set; }
    }

}