using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.Categories.Dtos
{
    public class CreateOrUpdateCategoryDto
    {
        public Guid? id { get; set; }
        public string name { get; set; }
        public Guid? parent_id { get; set; }
    }
}
