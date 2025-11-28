using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{
    public class LoginDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string EnglishName { get; set; }
        public string Surname { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string UserToken { get; set; }
        public List<string> Roles { get; set; }=new List<string>();
    }
}
