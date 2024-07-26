using Microsoft.Win32;
using Prism.Mvvm;
using System.IO;
using System.Windows;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class CommonViewService
    {
        private const string _imageDialogFilterPattern  = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.tiff) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.tiff";
        public Window CurrentOpenWindow { get; private set; }
        public void ShowMessage(string message) => MessageBox.Show(message);
        public void ShowActionResult(System.Net.HttpStatusCode code, string message)
        {
            if (code == System.Net.HttpStatusCode.OK)
                ShowMessage($"{code}\n{message}");
            else
                ShowMessage($"{code}\nSomething went wrong");
        }
        public void OpenWindow(Window window, BindableBase viewModel)
        {
            CurrentOpenWindow = window;
            window.DataContext = viewModel;
            window.ShowDialog();
        }

        public string GetFileFromDialog(string filter)
        {
            var filepath = string.Empty;

            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = filter;

            bool? result = dialog.ShowDialog();

            if (result == true)
                filepath = dialog.FileName;

            return filepath;
        }

        public void SetImageForObject(CommonModel model)
        {
            var imagePath = GetFileFromDialog(_imageDialogFilterPattern);

            if (!string.IsNullOrEmpty(imagePath))
            {
                var imageBytes = File.ReadAllBytes(imagePath);
                model.Image = imageBytes;
            }
        }

        public void SetFileForTask(TaskModel model)
        {
            var filePath = GetFileFromDialog(_imageDialogFilterPattern);

            if (!string.IsNullOrEmpty(filePath))
            {
                var fileBytes = File.ReadAllBytes(filePath);
                model.File = fileBytes;
            }
        }

        public void DownloadFile(byte[] fileData)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = "downloadedFile.txt", // Default file name
                Filter = "All files (*.*)|*.*" // File types filter
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, fileData);
                MessageBox.Show("File downloaded successfully!", "Download Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
