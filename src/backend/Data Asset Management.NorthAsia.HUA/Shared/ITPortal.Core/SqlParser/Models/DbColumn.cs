using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.SqlParser.Models
{
    public class DbColumn
    {
        public string Id { get; set; }
        /**
         * colName
         */
        public string ColName { get; set; }

        /**
         * dataType
         */
        public string DataType { get; set; }

        /**
         * dataLength
         */
        public string DataLength { get; set; }

        /**
         * dataPrecision
         */
        public string DataPrecision { get; set; }

        /**
         * dataScale
         */
        public string DataScale { get; set; }

        /**
         * isPk
         */
        public string? ColKey { get; set; }

        /**
         * nullable
         */
        public string? Nullable { get; set; }

        /**
         * colPosition
         */
        public int? ColPosition { get; set; }

        /**
         * col default value
         */
        public string DataDefault { get; set; }

        /**
         * col comment
         */
        public string ColComment { get; set; }
    }
}