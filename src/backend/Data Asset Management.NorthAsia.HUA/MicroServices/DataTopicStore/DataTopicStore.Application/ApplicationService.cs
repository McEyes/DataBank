using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Models;

namespace DataTopicStore.Application
{
    public abstract class ApplicationService : ITransient
    {
        protected ApplicationService()
        {
        }

        protected UserInfo CurrentUser
        {
            get
            {
                return App.HttpContext.GetCurrUserInfo();
            }
        }
    }
}
