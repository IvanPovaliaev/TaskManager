using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.Models;
using TaskManager.Client.Services;

namespace TaskManager.Client.Views.Components
{
    /// <summary>
    /// Interaction logic for TaskControl.xaml
    /// </summary>
    public partial class TaskControl : UserControl
    {
        public TaskControl(TaskClient task)
        {
            InitializeComponent();
            DataContext = task;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var commonViewService = new CommonViewService();
            commonViewService.DownloadFile(((TaskClient)DataContext).Model.File);
        }
    }
}
