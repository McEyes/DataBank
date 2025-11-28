using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    public class UserDto :PageEntity<string>
    {
        //public  string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string Department { get;  set; }
    }
}
