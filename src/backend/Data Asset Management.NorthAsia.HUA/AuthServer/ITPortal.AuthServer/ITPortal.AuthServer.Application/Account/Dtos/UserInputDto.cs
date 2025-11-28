using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    public class UserInputDto : Entity<string>
    {
        public string RoleId { get; set; }
    }
}
