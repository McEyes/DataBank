using Furion;

using SqlSugar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Repositorys
{

    public class Repository<T> : SimpleClient<T> where T : class, new()
    {
        public Repository(ISqlSugarClient db) : base(db)
        {
        }
        public Repository()
        {
            base.Context = App.GetService<ISqlSugarClient>();
            //base.Context = SqlSugarHelper.Db;
            //.NET自带IOC:  base.Context = 你存储的Services.GetService<ISqlSugarClient>(); 
            //Furion:       base.Context=App.GetService<ISqlSugarClient>();
            //Furion脚手架:    base.Context=DbContext.Instance
            //SqlSugar.Ioc:    base.Context=DbScoped.SugarScope; 
            //手动去赋值:     base.Context=DbHelper.GetDbInstance()     
        }
    }
}
