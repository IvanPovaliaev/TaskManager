﻿using System;
using System.Windows.Controls;
using TaskManager.Client.ViewModels;
using TaskManager.Common.Models;

namespace TaskManager.Client.Views.Pages
{
    /// <summary>
    /// Interaction logic for ProjectsPage.xaml
    /// </summary>
    public partial class ProjectsPage : Page
    {
        public ProjectsPage()
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
