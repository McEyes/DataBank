

using DataTopicStore.Core.Enums;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class TopicDetailsDto
    {
        public long id { get; set; }
        public string name { get; set; }
        public string cover { get; set; }
        public decimal? ratings { get; set; }
        public int ratings_count { get; set; }
        public bool? is_liked { get; set; }
        public bool? is_favorite { get; set; }
        public string datalineage_url { get; set; }
        public IDictionary<string,double> star_ratio { get; set; }
        public string version { get; set; }
        public string description { get; set; }
        public string department { get; set; }
        public int? referenced_times { get; set; }
        public int? favorite_count { get; set; }
        public int? comments_count { get; set; }
        public string awards { get; set; }
        public string author_id { get; set; }
        public string author { get; set; }
        public string owner_id { get; set; }
        public string owner { get; set; }
        public string owner_email { get; set; }
        public string content { get; set; }
        public string json_data_lineage { get; set; }
        public string json_formula { get; set; }
        public DateTimeOffset created_time { get; set; }
        public DateTimeOffset? updated_time { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        //public string json_parameters_input { get; set; }
        //public string json_parameters_output { get; set; }
        //public string sql_scripts { get; set; }        
        public Guid? category_id { get; set; }
        public string tags { get; set; }
        public bool? has_datapreview_permission { get; set; }
    }
}
