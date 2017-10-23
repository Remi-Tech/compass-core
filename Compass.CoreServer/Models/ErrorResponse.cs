namespace Compass.CoreServer.Models
{
    public class ErrorResponse
    {
        public string Error { get; set; }
        public bool Success => false;
        public string StackTrace { get; set; }
    }
}
