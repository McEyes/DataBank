
namespace ITPortal.Core.Services
{
    public interface IPageResult
    {
        int PageNum { get; set; }
        int PageSize { get; set; }
        int Total { get; set; }
        long Times { get; set; }
    }
    public interface IPageResult<T> : IPageResult
    {
        T Data { get; set; }
    }


    public class PageResult<T>: IPageResult<List<T>> where T : class
    {
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<T> Data { get; set; }
        public long Times { get; set; }

        public PageResult()
        {
            this.Total = 0;
            this.Data = new List<T>();
        }
        public PageResult(int total, List<T> data)
        {
            this.Total = total;
            this.Data = data;
        }
        public PageResult(int total, List<T> data,int pageNum,int pageSize)
        {
            this.Total = total;
            this.Data = data;
            this.PageNum = pageNum;
            this.PageSize = pageSize;
        }
        public PageResult(int total, List<T> data, int pageNum, int pageSize, long times)
        {
            this.Total = total;
            this.Data = data;
            this.PageNum = pageNum;
            this.PageSize = pageSize;
            this.Times = times;
        }
    }
    public class PageResult : IPageResult<object>
    {
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public long Times { get; set; }
        public object Data { get; set; }

        public PageResult()
        {
            this.Total = 0;
            this.Data = new List<object>();
        }
        public PageResult(int total, object data)
        {
            this.Total = total;
            this.Data = data;
        }
        public PageResult(int total, object data, int pageNum, int pageSize)
        {
            this.Total = total;
            this.Data = data;
            this.PageNum = pageNum;
            this.PageSize = pageSize;
        }
        public PageResult(int total, object data, int pageNum, int pageSize, long times)
        {
            this.Total = total;
            this.Data = data;
            this.PageNum = pageNum;
            this.PageSize = pageSize;
            this.Times = times;
        }
    }
}
