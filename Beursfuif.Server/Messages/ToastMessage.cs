namespace Beursfuif.Server.Messages
{
    public class ToastMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public ToastMessage(string title, string message = "")
        {
            Title = title;
            Message = message;
        }
    }
}
