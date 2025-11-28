
using Google.Rpc;

using Microsoft.AspNetCore.Identity;

using StackExchange.Profiling.Internal;

namespace ITPortal.Core.Services
{
    public interface IResult
    {
        int? Code { get; set; }
        object Msg { get; set; }
        bool Success { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        object Extras { get; set; }
        long Timestamp { get; set; }
        public static IResult Successd()
        {
            return new Result();
        }

        public static IResult Fail(string errorMsg, int code = 503)
        {
            return new Result().SetError(errorMsg, code);
        }
    }

    public interface IResult<T> : IResult
    {
        T Data { get; set; }

        public new static IResult<T> Successd()
        {
            return new Result<T>();
        }

        public new static IResult<T> Fail(string errorMsg, int code = 503)
        {
            return new Result<T>().SetError(errorMsg, code);
        }
    }

    public class Result : IResult
    {
        public int? Code { get; set; } = 200;
        public object Msg { get; set; } = "success";
        public bool Success { get; set; } = true;
        /// <summary>
        /// 附加数据
        /// </summary>
        public object Extras { get; set; }
        public long Timestamp { get; set; } = DateTimeOffset.Now.Millisecond;

        public virtual Result SetError(string msg="", int code = 503)
        {
            Success = false;
            Code = code;
            if (msg.IsNullOrWhiteSpace()) Msg = "fail";
            else
                Msg = msg;
            return this;
        }
        public virtual Result AddError(string msg = "", int code = 503)
        {
            Success = false;
            Code = code;
            if (msg.IsNullOrWhiteSpace()) Msg = "fail";
            else
            {
                if (Msg.ToString() == "success")
                    Msg = msg;
                else
                    Msg += msg;
            }
            return this;
        }
        public virtual Result AddMsg(IResult data)
        {
            if (data.Success) return this;
            AddError(data.Msg + "", data.Code.Value);
            return this;
        }

        public virtual Result SetError(bool success)
        {
            if (!success) SetError();
            return this;
        }

        public static Result Successd()
        {
            return new Result();
        }

        public static Result<T> Successd<T>(T data)
        {
            return new Result<T>() { Data = data };
        }

        public static Result Fail(string errorMsg, int code = 503)
        {
            return new Result().SetError(errorMsg, code);
        }
    }

    public class Result<T> : Result, IResult<T>
    {
        public T Data { get; set; }

        public override Result<T> SetError(string msg, int code = 503)
        {
            base.SetError(msg, code);
            return this;
        }
        //public override Result<T> SetError(bool success)
        //{
        //    base.SetError(success);
        //    return this;
        //}
        //public override Result<T> AddError(string msg, int code = 503)
        //{
        //    base.AddError(msg, code);
        //    return this;
        //}

        public override Result AddMsg(IResult data)
        {
            base.AddMsg(data);
            if (data is Result<T>)
            {
                var result = (Result<T>)data;
                if (result.Data != null)
                    this.Data = result.Data;
            }
            return this;
        }

        public static Result<T> Successd(T data)
        {
            return new Result<T>() { Data = data };
        }

        public new static Result<T> Fail(string errorMsg, int code = 503)
        {
            return new Result<T>().SetError(errorMsg, code);
        }
    }
}
