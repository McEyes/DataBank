using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataTopicStore.Core.Extensions
{
    public static class IQueryCollectionExtensions
    {
        public static int GetPageIndex(this IQueryCollection query)
        {
            if (query.ContainsKey("pageindex"))
            {
                var pageindex = Convert.ToInt32(query["pageindex"].ToString());
                return pageindex <= 0 ? 1 : pageindex;
            }

            return -1;
        }

        public static int GetPageSize(this IQueryCollection query)
        {
            if (query.ContainsKey("pagesize"))
            {
                var pagesize = Convert.ToInt32(query["pagesize"].ToString());
                return pagesize <= 0 ? 10 : pagesize;
            }

            return -1;
        }
    }
}
