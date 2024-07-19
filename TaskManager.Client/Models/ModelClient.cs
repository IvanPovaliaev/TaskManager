using System.Windows.Media.Imaging;
using TaskManager.Client.Models.Extensions;
using TaskManager.Common.Models;

namespace TaskManager.Client.Models
{
    public class ModelClient<T> where T : CommonModel
    {
        public T Model { get; private set; }
        public BitmapImage Image { get => Model.LoadImage(); }
        public ModelClient(T model)
        {
            Model = model;
        }        
    }
}
