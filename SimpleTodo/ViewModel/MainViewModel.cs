using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using Coding4Fun.Phone.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SimpleTodo.Model;

namespace SimpleTodo.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private const string TODO_ITEMS_FILE = "todoitems.xml";
		private const string DONE_ITEMS_FILE = "doneitems.xml";

		private ICommand _addCommand;
		private ICommand _doneCommand;
		private ICommand _undoneCommand;
		private ICommand _deleteTodoItemCommand;
		private ICommand _deleteDoneItemCommand;
		private ICommand _editTodoItemCommand;
		private Task _currentTask;

		private ObservableCollection<Task> _itemsTodo;
		private ObservableCollection<Task> _itemsDone;
		private string _taskDescription;

		#region Command

		public ICommand DeleteTodoItemCommand
		{
			get { return _deleteTodoItemCommand; }
		}

		public ICommand DeleteDoneItemCommand
		{
			get { return _deleteDoneItemCommand; }
		}

		public ICommand UndoneCommand
		{
			get { return _undoneCommand; }
		}

		public ICommand DoneCommand
		{
			get { return _doneCommand; }
		}

		public ICommand AddCommand
		{
			get { return _addCommand; }
		}

		public ICommand EditTodoItemCommand
		{
			get { return _editTodoItemCommand; }
		}

		#endregion


		#region Properties

		public string TaskDescription
		{
			get { return _taskDescription; }
			set
			{
				_taskDescription = value;
				RaisePropertyChanged("TaskDescription");
			}
		}

		public ObservableCollection<Task> ItemsTodo
		{
			get { return _itemsTodo; }

			set
			{
				_itemsTodo = value;
			}
		}

		public ObservableCollection<Task> ItemsDone
		{
			get { return _itemsDone; }

			set
			{
				_itemsDone = value;
			}
		}

		public string ApplicationTitle
		{
			get
			{
				return "Simple Todo";
			}
		}

		public string TodoPageName
		{
			get
			{
				return "Todo";
			}
		}

		public string DonePageName
		{
			get
			{
				return "Done";
			}
		}



		#endregion


		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{
			_addCommand = new RelayCommand(AddAction);
			_doneCommand = new RelayCommand<Task>(DoneAction);
			_undoneCommand = new RelayCommand<Task>(UndoneAction);
			_deleteTodoItemCommand = new RelayCommand<Task>(DeleteTodoItemAction);
			_deleteDoneItemCommand = new RelayCommand<Task>(DeleteDoneItemAction);
			_editTodoItemCommand = new RelayCommand<Task>(EditTodoItemAction);

			Initialize();
		}

		private void LoadDoneItems(string fileName)
		{
			_itemsDone = LoadFile(fileName);
		}

		private void LoadTodoItems(string fileName)
		{
			_itemsTodo = LoadFile(fileName);
		}


		private void SaveFile(string fileName, ObservableCollection<Task> items)
		{
			var xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;

			using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
			{
				using (var stream = isolatedStorage.OpenFile(fileName, FileMode.Create))
				{
					var serializer = new XmlSerializer(typeof(ObservableCollection<Task>));
					using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
					{
						serializer.Serialize(xmlWriter, items);
					}
				}
			}
		}

		private static ObservableCollection<Task> LoadFile(string fileName)
		{
			var items = new ObservableCollection<Task>();

			using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
			{
				if (myIsolatedStorage.FileExists(fileName))
				{
					using (var stream = myIsolatedStorage.OpenFile(fileName, FileMode.Open))
					{
						var serializer = new XmlSerializer(typeof(ObservableCollection<Task>));
						items = (ObservableCollection<Task>) serializer.Deserialize(stream);
					}
				}
			}

			return items;
		}

		private void EditTodoItemAction(Task task)
		{
			_currentTask = task;

			var input = new InputPrompt();

			input.IsCancelVisible = true;
			input.Completed += input_Completed;
			input.Value = task.Description;
			input.Title = "Edit description";

			input.Show();
		}

		// ReSharper disable InconsistentNaming
		private void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
		// ReSharper restore InconsistentNaming
		{
			var input = sender as InputPrompt;

			if (input != null && e.PopUpResult != PopUpResult.Cancelled)
			{
				var itemTodo = ItemsTodo.FirstOrDefault(x => x.Id == _currentTask.Id);

				if (itemTodo != null)
				{
					itemTodo.Description = input.Value;

					SaveFile(TODO_ITEMS_FILE, _itemsTodo);
				}
			}
		}

		private void DeleteDoneItemAction(Task task)
		{
			_itemsDone.Remove(task);

			SaveFile(DONE_ITEMS_FILE, _itemsDone);
		}

		private void DeleteTodoItemAction(Task task)
		{
			_itemsTodo.Remove(task);

			SaveFile(TODO_ITEMS_FILE, _itemsTodo);
		}

		private void UndoneAction(Task task)
		{
			_itemsDone.Remove(task);

			task.Done = false;

			_itemsTodo.Add(task);

			SaveFile(DONE_ITEMS_FILE, _itemsDone);
			SaveFile(TODO_ITEMS_FILE, _itemsTodo);
		}

		private void DoneAction(Task task)
		{
			_itemsTodo.Remove(task);

			task.Done = true;

			_itemsDone.Add(task);

			SaveFile(TODO_ITEMS_FILE, _itemsTodo);
			SaveFile(DONE_ITEMS_FILE, _itemsDone);
		}

		private void AddAction()
		{
			if (!string.IsNullOrEmpty(TaskDescription))
			{
				_itemsTodo.Add(new Task { Description = TaskDescription, Done = false });

				TaskDescription = string.Empty;

				SaveFile(TODO_ITEMS_FILE, _itemsTodo);
			}
		}

		public void Initialize()
		{
			_itemsDone = new ObservableCollection<Task>();
			_itemsTodo = new ObservableCollection<Task>();
		}

		public void LoadTodoItemFiles()
		{
			LoadTodoItems(TODO_ITEMS_FILE);

			LoadDoneItems(DONE_ITEMS_FILE);
		}
	}
}