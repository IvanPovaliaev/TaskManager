using System.Windows.Media.Imaging;
using TaskManager.Client.Models.Extensions;
using TaskManager.Common.Models;

namespace TaskManager.Client.Models
{
    public class UserModelClient
    {
        public UserModel Model { get; private set; }
        public BitmapImage Photo { get => Model.LoadPhoto(); }
        public UserModelClient(UserModel model)
        {
            Model = model;
        }
    }
}
