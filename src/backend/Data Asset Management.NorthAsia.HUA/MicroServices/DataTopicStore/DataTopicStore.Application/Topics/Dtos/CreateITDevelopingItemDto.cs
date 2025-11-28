
namespace DataTopicStore.Application.Topics.Dtos
{
    public class CreateITDevelopingItemDto
    {
        public DateTime? process_time { get; set; }       
        public string? pic_ntid { get; set; }
        public string? pic { get; set; }
    }

    public class CreateITDevelopingModel
    {
        public CreateITDevelopingItemDto DataSourceDefinition { get; set; }
        public CreateITDevelopingItemDto DataSourceIngestion { get; set; }
        public CreateITDevelopingItemDto PhysicalModeling { get; set; }
        public CreateITDevelopingItemDto TScriptGeneration { get; set; }
        public CreateITDevelopingItemDto APIGeneration { get; set; }
    }

    public class CreateITDevelopingDto
    {
        public long topic_id { get; set; }
        public CreateITDevelopingModel Data { get; set; }
    }
}
