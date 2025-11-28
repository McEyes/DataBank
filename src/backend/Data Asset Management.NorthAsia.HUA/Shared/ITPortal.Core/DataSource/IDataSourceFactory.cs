using SqlSugar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.DataSource
{

    public interface IDataSourceFactory
    {

        /// <summary>
        /// 创建数据源实例
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        ISqlSugarClient CreateSqlClient(DbSchema property);
    }

}
