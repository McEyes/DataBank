using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Exceptions
{
    public class DataQueryException : Furion.FriendlyException.AppFriendlyException
    {
        public DataQueryException(string message) : base(message, 800)
        {

        }
    }
}
