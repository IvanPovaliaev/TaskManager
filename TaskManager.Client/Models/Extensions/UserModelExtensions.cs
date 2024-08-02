using System;
using System.IO;
using System.Windows.Media.Imaging;
using TaskManager.Common.Models;

namespace TaskManager.Client.Models.Extensions
{
    public static class UserModelExtensions
    {
        public static BitmapImage LoadPhoto(this UserModel model)
        {
            if (model?.Photo == null || model.Photo.Length == 0)
            {
                var photo = new BitmapImage();
                photo.BeginInit();
                photo.UriSource = new Uri("pack://application:,,,/Resources/Images/DefaultUserImage.png", UriKind.Absolute);
                photo.EndInit();
                return photo;
            }

            var image = new BitmapImage();
            using (var memSrm = new MemoryStream(model.Photo))
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
