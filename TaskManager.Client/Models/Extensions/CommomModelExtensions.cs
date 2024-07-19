using System.IO;
using System.Windows.Media.Imaging;
using TaskManager.Common.Models;

namespace TaskManager.Client.Models.Extensions
{
    public static class CommomModelExtensions
    {
        public static BitmapImage LoadImage(this CommonModel model)
        {
            if (model?.Image == null || model.Image.Length == 0)
                return null;

            var image = new BitmapImage();
            using (var memSrm = new MemoryStream(model.Image))
            {
                memSrm.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = memSrm;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
