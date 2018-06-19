using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace Create_DB
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();			
		}

		private void Connect()
		{
			SqlConnection conn = new SqlConnection();
			SqlConnectionStringBuilder str = new SqlConnectionStringBuilder();
			str.DataSource = tbServer.Text;
			str.UserID = tbLogin.Text;
			str.Password = pbPass.Password;
			//str.IntegratedSecurity = true;
			str.InitialCatalog = "groupsDB";
			conn.ConnectionString = str.ConnectionString;
			string command = @"create table groups
							(ID_GROUPS int primary key identity(1, 1),
							Name varchar(30) not null);";
			SqlCommand sqlCommand = new SqlCommand(command, conn);
			try
			{
				conn.Open();
				ListBoxItem item = new ListBoxItem()
				{
					Content = "Connection Opened.",
					Foreground = new SolidColorBrush(Color.FromRgb(0x33, 0xaa, 0x33))
				};
				listBox.Items.Add(item);
								
				sqlCommand.ExecuteNonQuery();
				item = new ListBoxItem()
				{
					Content = "Table created.",
					Foreground = new SolidColorBrush(Color.FromRgb(0x33, 0xaa, 0x33))
				};
				listBox.Items.Add(item);				
			}
			catch (Exception ex)
			{
				ListBoxItem item = new ListBoxItem()
				{
					Content = ex.Message,
					Foreground = new SolidColorBrush(Color.FromRgb(0xbb, 0x33, 0x33))
				};
				listBox.Items.Add(item);
			}
			finally
			{
				conn.Close();
				ListBoxItem item = new ListBoxItem()
				{
					Content = "Connection Closed.",
					Foreground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0xaa))
				};
				listBox.Items.Add(item);				
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Connect();
		}
	}
}
