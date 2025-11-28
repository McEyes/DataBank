using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.Common.Dtos
{
    public interface IHttpResult
    {
        int? Code { get; set; }
        object Msg { get; set; }
        bool Success { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        object Extras { get; set; }
        long Timestamp { get; set; }
        public static IHttpResult Successd()
        {
            return new HttpResult();
        }

        public static IHttpResult Fail(string errorMsg, int code = 503)
        {
            return new HttpResult().SetError(errorMsg, code);
        }
    }

    public interface IResult<T> : IHttpResult
    {
        T Data { get; set; }

        public new static IResult<T> Successd()
        {
            return new HttpResult<T>();
        }

        public new static IResult<T> Fail(string errorMsg, int code = 503)
        {
            return new HttpResult<T>().SetError(errorMsg, code);
        }
    }

    public class HttpResult : IHttpResult
    {
        public int? Code { get; set; } = 200;
        public object Msg { get; set; } = "success";
        public bool Success { get; set; } = true;
        /// <summary>
        /// 附加数据
        /// </summary>
        public object Extras { get; set; }
        public long Timestamp { get; set; } = DateTime.Now.Millisecond;

        public virtual HttpResult SetError(string msg = "", int code = 503)
        {
            Success = false;
            Code = code;
            if (string.IsNullOrWhiteSpace(msg)) Msg = "fail";
            else
                Msg = msg;
            return this;
        }
        public virtual HttpResult SetError(bool success)
        {
            if (!success) SetError();
            return this;
        }

        public static HttpResult Successd()
        {
            return new HttpResult();
        }

        public static HttpResult<T> Successd<T>(T data)
        {
            return new HttpResult<T>() { Data = data };
        }

        public static HttpResult Fail(string errorMsg, int code = 503)
        {
            return new HttpResult().SetError(errorMsg, code);
        }
    }

    public class HttpResult<T> : HttpResult, IResult<T>
    {
        public T Data { get; set; }

        public override HttpResult<T> SetError(string msg, int code = 503)
        {
            base.SetError(msg, code);
            return this;
        }
        public override HttpResult<T> SetError(bool success)
        {
            base.SetError(success);
            return this;
        }
        public static HttpResult<T> Successd(T data)
        {
            return new HttpResult<T>() { Data = data };
        }

        public new static HttpResult<T> Fail(string errorMsg, int code = 503)
        {
            return new HttpResult<T>().SetError(errorMsg, code);
        }
    }
}
