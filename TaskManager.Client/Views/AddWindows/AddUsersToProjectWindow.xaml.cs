using System;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.ViewModels;
using TaskManager.Common.Models;

namespace TaskManager.Client.Views.AddWindows
{
    /// <summary>
    /// Interaction logic for AddUsersToProjectWindow.xaml
    /// </summary>
    public partial class AddUsersToProjectWindow : Window
    {
        public AddUsersToProjectWindow()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = (ProjectsPageViewModel)DataContext;

            foreach (UserModel user in e.RemovedItems)
            {
                try
                {
                    viewModel.SelectedUsersForProject.Remove(user);
                }
                catch (NullReferenceException _)
                {

                }
            }

            foreach (UserModel user in e.AddedItems)
            {
                try
                {
                    viewModel.SelectedUsersForProject.Add(user);
                }
                catch (NullReferenceException _)
                {

                }
            }
        }
    }
}
