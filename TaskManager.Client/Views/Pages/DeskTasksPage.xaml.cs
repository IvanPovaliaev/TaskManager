using System.Windows.Controls;


namespace TaskManager.Client.Views.Pages
{
    /// <summary>
    /// Interaction logic for DeskTasksPage.xaml
    /// </summary>
    public partial class DeskTasksPage : Page
    {
        public Grid TasksGrid { get; private set; }
        public DeskTasksPage()
        {
            InitializeComponent();
            TasksGrid = tasksGrid;
        }
    }
}
