namespace Beursfuif.Server.Messages
{
    public class ChangeVisibilityMessage
    {
        public string ClassName { get; set; }

        public ChangeVisibilityMessage(string classname)
        {
            ClassName = classname;
        }

        public ChangeVisibilityMessage()
        {

        }
    }

}
