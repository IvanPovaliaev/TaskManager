using System.Windows;

namespace TaskManager.Client.Services
{
    public class CommonViewService
    {
        public void ShowMessage(string message) => MessageBox.Show(message);
    }
}
