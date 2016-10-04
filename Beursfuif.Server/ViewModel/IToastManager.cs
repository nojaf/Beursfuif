namespace Beursfuif.Server.ViewModel
{
    public interface IToastManager
    {
        void ShowToast(string title, string message = "");
    }
}
