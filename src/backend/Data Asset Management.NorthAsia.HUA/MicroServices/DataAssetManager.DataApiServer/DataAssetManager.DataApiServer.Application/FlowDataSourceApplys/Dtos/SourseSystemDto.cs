namespace DataAssetManager.DataApiServer.Application.FlowDataSourceApplys.Dtos
{
    public class SourseSystemDto
    {
        public string MasterData { get; set; }
        public string SourceSystem { get; set; }
        public string DataSource { get; set; }
        public bool? NeedStandard { get; set; }
        public bool? IsMapping { get; set; }
    }
}