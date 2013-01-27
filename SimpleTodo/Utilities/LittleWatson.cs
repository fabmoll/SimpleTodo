using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace SimpleTodo.Utilities
{
	public class LittleWatson
	{
		const string FILENAME = "LittleWatson.txt";

		internal static void ReportException(Exception ex, string extra)
		{
			try
			{
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					SafeDeleteFile(store);
					using (TextWriter output = new StreamWriter(store.CreateFile(FILENAME)))
					{
						output.WriteLine(extra);
						output.WriteLine(ex.Message);
						output.WriteLine(ex.StackTrace);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		internal static void CheckForPreviousException()
		{
			try
			{
				string contents = null;
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					if (store.FileExists(FILENAME))
					{
						using (TextReader reader = new StreamReader(store.OpenFile(FILENAME, FileMode.Open, FileAccess.Read, FileShare.None)))
						{
							contents = reader.ReadToEnd();
						}
						SafeDeleteFile(store);
					}
				}
				if (contents != null)
				{
					if (MessageBox.Show("A problem occurred the last time you ran this application. Would you like to send an email to report it?", "Problem Report", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
					{
						EmailComposeTask email = new EmailComposeTask();
						email.To = "your@email.com";
						email.Subject = "SimpleTodo auto-generated problem report";
						email.Body = contents;
						SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication()); // line added 1/15/2011
						email.Show();
					}
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
			}
		}

		private static void SafeDeleteFile(IsolatedStorageFile store)
		{
			try
			{
				store.DeleteFile(FILENAME);
			}
			catch (Exception)
			{
			}
		}
	}
}
