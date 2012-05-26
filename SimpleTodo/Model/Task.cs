using System;
using GalaSoft.MvvmLight;

namespace SimpleTodo.Model
{
	public class Task : ViewModelBase
	{
		private string _description;

		public Guid Id { get; set; }
		public bool Done { get; set; }

		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				RaisePropertyChanged("Description");
			}
		}
		public Task()
		{
			Id = Guid.NewGuid();
		}
	}
}