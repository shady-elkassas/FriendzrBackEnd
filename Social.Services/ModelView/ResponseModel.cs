using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Entity.ModelView
{
    public class ResponseModel<T>
    {
       
        public ResponseModel(int statusCode, bool isSuccessful, string message, T model)
        {
            IsSuccessful = isSuccessful;
            Message = message;
            StatusCode = statusCode;
            Model = model;
        }
        public T Model { get; set; }
        public bool IsSuccessful { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
    public class CommonResponse<T>
    {
        public int Code { get; set; }
        public bool Status { get; set; }


        public string Message { get; set; }

        public T Data { get; set; }
        public Dictionary<string, string> ModelErrors { get; set; }
        public static CommonResponse<T> GetResult(int code, bool Status, string msg, T data = default(T), Dictionary<string, string> ModelErrors = default(Dictionary<string, string>))
        {
            return new CommonResponse<T>
            {
                Code = code,
                Message = msg,
                Data = data,
                ModelErrors = ModelErrors,
                Status = Status

            };
        }
        public static CommonResponse<T> GetResult(string msg, Dictionary<string, string> ModelErrors, T data = default(T))
        {
            return new CommonResponse<T>
            {
                Code = 403,
                Message = msg,
                Data = data,
                ModelErrors = ModelErrors,
                Status = false

            };
        }
        public static CommonResponse<T> GetResult(bool Status, string msg, T data = default(T))
        {
            return new CommonResponse<T>
            {
                Message = msg,
                Data = data,
                Status = Status

            };
        }
    }
}
