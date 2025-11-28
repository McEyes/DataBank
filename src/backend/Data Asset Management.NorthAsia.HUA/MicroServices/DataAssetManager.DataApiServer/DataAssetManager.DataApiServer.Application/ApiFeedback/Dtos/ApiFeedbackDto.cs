using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Xml;
using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    public class ApiFeedbackDto : PageEntity<Guid>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Files { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserDept { get; set; }

        public string Status { get; set; }

        public string ObjectId { get; set; }

        public string ObjectType { get; set; }

        public string ObjectName { get; set; }

        public string OwnerId { get; set; }

        public string OwnerName { get; set; }

        public string OwnerEmail { get; set; }



    }

}