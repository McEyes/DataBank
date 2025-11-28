using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Exceptions
{
    public class SqlQueryIllegalSQLDataException : DataQueryException
    {
        public SqlQueryIllegalSQLDataException(string message, string tableName) :
            base($"{message}, illegal SQL! SQL cannot contain multiple tables, only the data of table {tableName} in the API can be queried.<br />非法sql，Sql不能包含多表,联表查询")
        {

        }
    }
}
