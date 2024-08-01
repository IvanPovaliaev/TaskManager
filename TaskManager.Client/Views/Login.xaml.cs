using System.Windows;
using TaskManager.Client.ViewModels;

namespace TaskManager.Client.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            ((LoginViewModel)DataContext).LoginWindow = this;
        }
    }
}
