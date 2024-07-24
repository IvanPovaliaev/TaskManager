using System.Windows.Controls;
using TaskManager.Client.Models;

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
    }
}
