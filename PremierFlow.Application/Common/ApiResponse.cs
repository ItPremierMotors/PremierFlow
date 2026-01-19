using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? message { get; set; }
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }
         
        public static ApiResponse<T> ok(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = 200,
                Data = data,
                message = message
            };
        }
        public static ApiResponse<T> fail(int statusCode, IEnumerable<string>? errors = null, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Errors = errors,
                message = message
            };
        }

    }
}
