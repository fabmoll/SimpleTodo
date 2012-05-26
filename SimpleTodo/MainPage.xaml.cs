using System;
using System.Windows;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using SimpleTodo.Utilities;
using SimpleTodo.ViewModel;

namespace SimpleTodo
{
	public partial class MainPage : PhoneApplicationPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var viewModel = DataContext as MainViewModel;

			if(viewModel != null)
			{
				viewModel.LoadTodoItemFiles();
			}

		}
	}
}