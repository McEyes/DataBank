

using DataTopicStore.Core.Enums;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class CreateOrUpdateTopicDto
    {
        public long id { get; set; }

        public Guid category_id { get; set; }
        public string name { get; set; }

        public string cover { get; set; }
  
        public string version { get; set; }
  
        public string description { get; set; }

        public string content { get; set; }
     
        public string json_data_lineage { get; set; }
    
        public string json_formula { get; set; }
     
        public string sql_scripts { get; set; }
        public string tagList { get; set; }
        public string owner_id { get; set; }
    }
}
