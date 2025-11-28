using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.SqlParser.Models
{
    public class DbTable
    {
        /**
         * tableName
         */
        public string TableName { get; set; }

        /**
         * tableComment
         */
        public string TableComment { get; set; }

        public string Type { get; set;}
    }
}