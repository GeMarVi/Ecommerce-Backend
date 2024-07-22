using System.Net;

namespace Ecommerce.Shared.DTOs
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public static ApiResponse Success(string message, object data = null)
        {
            return new ApiResponse
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                StatusCode = HttpStatusCode.OK
            };
        }

        public static ApiResponse Error(string message, HttpStatusCode statusCode, object data = null)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }
    }
}
