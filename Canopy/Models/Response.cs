namespace Canopy.Models
{
    public class Request
    {
        public string Data { get; set; }
        public string Meta { get; set; }
    }
    public class Response
    {
        public bool status { get; set; }
        public string Message { get; set; }
    }
}
