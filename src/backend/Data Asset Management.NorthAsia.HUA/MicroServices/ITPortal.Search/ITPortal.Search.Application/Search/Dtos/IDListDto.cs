using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.Search.Dtos
{
    public class IDListDto<T>
    {
        public IEnumerable<T> Ids { get; set; }


        public IDListDto(IEnumerable<T> ids)
        {
            Ids = ids;
        }

    }
}
