namespace SCILL.Client
{
    public class HttpMethod
    {
        public static HttpMethod Get { get; } = new HttpMethod("GET");
        public static HttpMethod Post { get; } = new HttpMethod("POST");
        public static HttpMethod Put { get; } = new HttpMethod("PUT");
        public static HttpMethod Delete { get; } = new HttpMethod("Delete");
        public static HttpMethod Head { get; } = new HttpMethod("HEAD");

        private string _method;

        private HttpMethod(string method)
        {
            _method = method;
        }

        public override string ToString()
        {
            return _method;
        }
    }
}