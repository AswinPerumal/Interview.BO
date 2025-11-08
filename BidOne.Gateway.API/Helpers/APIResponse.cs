namespace BidOne.Gateway.API.Helpers
{
    public class APIResponse
    {
        public int StatusCode { get; set; }
        public string? Error { get; set; }
        public object? Data { get; set; }

        public static APIResponse Success(object? data, int statusCode = 200)
            => new APIResponse { StatusCode = statusCode, Data = data };

        public static APIResponse BadRequest(string error, int statusCode = 400)
            => new APIResponse { StatusCode = statusCode, Error = error };

        public static APIResponse InternalServerError(string error, int statusCode = 500)
         => new APIResponse { StatusCode = statusCode, Error = error };
    }
}
