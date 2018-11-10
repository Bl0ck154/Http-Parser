using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HttpParser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			KeyDown += MainWindow_KeyDown;
			Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			textboxAddress.Focus();
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				btnStart.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
		}

		private void btnStart_Click(object sender, RoutedEventArgs e)
		{
			string Url = textboxAddress.Text;

			if (string.IsNullOrWhiteSpace(Url) || Url.Length < 3)
				return;

			if (!Url.StartsWith("http://") && !Url.StartsWith("https://"))
				Url = Url.Insert(0, "http://");
			try
			{
				WebRequest request = WebRequest.Create(Url);
				WebResponse response = request.GetResponse();
				using (Stream stream = response.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						string page = reader.ReadToEnd();
						List<string> words = new List<string>();
						Regex.Matches(page, ">([^<]*)<").Cast<Match>()
							.Select(m => m.Value.Trim(new char[] { '>', '<' }).Trim(' '))
							.Where(s => !String.IsNullOrWhiteSpace(s))
							.Select(s => s.Split(new char[] { ' ', '.', ',', '-', '!', '?', '"', '"', ':', ';', '/' }, StringSplitOptions.RemoveEmptyEntries)
							.Where(ss => !String.IsNullOrWhiteSpace(ss)))
							.ToList().ForEach(s => words.AddRange(s));
						myOutput.ItemsSource = words.GroupBy(s => s)
							.Select(s => new { Word = s.Key, Count = s.Count() })
							.OrderByDescending(s => s.Count);
					}
				}
				response.Close();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
	}
}
