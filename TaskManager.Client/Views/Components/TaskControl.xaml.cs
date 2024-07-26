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
        public Grid TaskGrid { get; private set; }
        public TaskControl(TaskClient task)
        {
            InitializeComponent();
            DataContext = task;
            TaskGrid = taskGrid;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var commonViewService = new CommonViewService();
            var currentTask = (TaskClient)DataContext;
            commonViewService.DownloadFile(currentTask.Model.File, currentTask.Model.FileName);
        }
    }
}
