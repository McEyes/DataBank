namespace Sqllineage
{
    public class ResponseResult
    {
        public int StatusCode { get; set; }
        public bool Succeeded { get; set; }
        public string Data { get; set; }
        public string Errors { get; set; }
    }
}
