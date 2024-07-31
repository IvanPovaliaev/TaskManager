using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.ViewModels;
using TaskManager.Common.Models;

namespace TaskManager.Client.Views.AddWindows
{
    /// <summary>
    /// Interaction logic for UsersFromExcelWindow.xaml
    /// </summary>
    public partial class UsersFromExcelWindow : Window
    {
        public UsersFromExcelWindow()
        {
            InitializeComponent();
        }

        private void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var context = (UsersPageViewModel)DataContext;

            foreach (var item in e.RemovedItems)
            {
                if (item is UserModel)
                    context.SelectedUsersFromExcel.Remove((UserModel)item);
            }
                

            foreach (var item in e.AddedItems)
            {
                if (item is UserModel)
                    context.SelectedUsersFromExcel.Add((UserModel)item);
            }               
        }
    }
}
