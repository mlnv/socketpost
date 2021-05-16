namespace Socketpost.DesktopApp.Models
{
    public class Message
    {
        public bool Informational { get; set; }
        public bool FromServer { get; set; }
        public string Data { get; set; }
    }
}
