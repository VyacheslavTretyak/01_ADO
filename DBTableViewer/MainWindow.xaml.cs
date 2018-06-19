using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
using RandomNameGenerator;

namespace DBTableViewer
{
	public class People
	{
		public int ID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public override string ToString()
		{
			return $"{FirstName} {LastName}";
		}
	}
	public class Sale
	{
		public int ID { get; set; }
		public string Trader { get; set; }
		public string Buyer { get; set; }
		public int Amount { get; set; }
		public string Date { get; set; }
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>	
	public partial class MainWindow : Window
	{
		private List<People> traders;
		private List<People> buyers;
		private List<Sale> sales;		
		public MainWindow()
		{
			InitializeComponent();
			traders = new List<People>();
			buyers = new List<People>();
			sales = new List<Sale>();
			SqlDataReader reader = null;
			SqlConnection conn = new SqlConnection();
			try
			{
				Connecting(conn);
				//InitData(conn);
				ReadData(conn, reader);
			}
			catch (Exception ex)
			{
				ShowMessage(ex.Message, Color.FromRgb(0xbb, 0x33, 0x33));				
			}
			finally
			{
				conn?.Close();
				reader?.Close();
				ShowMessage("Connection Closed.", Color.FromRgb(0x33, 0x33, 0xaa));				
			}
				
		}

		private void ReadData(SqlConnection conn, SqlDataReader reader)
		{
			StringBuilder commands = new StringBuilder();
			commands.Append(@"SELECT TABLE_NAME
								FROM INFORMATION_SCHEMA.TABLES
								WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='TraderAndBuyer';");
			commands.Append(@"SELECT * FROM traders;");
			commands.Append(@"SELECT * FROM buyers;");
			commands.Append(@"SELECT * FROM sales;");

			SqlCommand sqlCommand = new SqlCommand(commands.ToString(), conn);

			

			reader = sqlCommand.ExecuteReader();
			while (reader.Read())
			{				
				cbTables.Items.Add(reader[0]);
			}
			ShowMessage("Name of Table is read.", Color.FromRgb(0x99, 0x99, 0x00));
			
			if (reader.NextResult())
			{
				while (reader.Read())
				{
					traders.Add(new People() { ID = int.Parse(reader[0].ToString()), FirstName = reader[1].ToString(), LastName = reader[2].ToString() });
				}				
				ShowMessage($"Traders Table is read.", Color.FromRgb(0x99, 0x99, 0x00));				
			}
			if (reader.NextResult())
			{
				while (reader.Read())
				{
					buyers.Add(new People() { ID = int.Parse(reader[0].ToString()), FirstName = reader[1].ToString(), LastName = reader[2].ToString() });
				}
				ShowMessage($"Buyers Table is read.", Color.FromRgb(0x99, 0x99, 0x00));
				
			}
			if (reader.NextResult())
			{
				while (reader.Read())
				{
					sales.Add(new Sale()
					{
						ID = int.Parse(reader[0].ToString()),
						Trader = traders.OfType<People>().Single(people => people.ID == int.Parse(reader[1].ToString())).ToString(),
						Buyer = traders.OfType<People>().Single(people => people.ID == int.Parse(reader[2].ToString())).ToString(),
						Amount = int.Parse(reader[3].ToString()),
						Date = reader[4].ToString()						
					});
				}
				ShowMessage($"Sales Table is read.", Color.FromRgb(0x99, 0x99, 0x00));
				
			}
			reader?.Close();
		}

		private void Connecting(SqlConnection conn)
		{
			try
			{
				conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
				conn.Open();
				ShowMessage("Connection Opened.", Color.FromRgb(0x33, 0xaa, 0x33));
			}
			catch (Exception)
			{
				throw;
			}			
		}

		private void cbTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			dataGrid.AutoGenerateColumns = true;
			switch(cbTables.SelectedValue.ToString())
			{
				case "Traders":
					dataGrid.ItemsSource = traders;
					break;
				case "Buyers":
					dataGrid.ItemsSource = buyers;
					break;
				case "Sales":
					dataGrid.ItemsSource = sales;
					break;
			}				
		}
		private void InitData(SqlConnection conn)
		{
			for (int i = 0; i < 20; i++)
			{
				string[] name = NameGenerator.Generate(Gender.Male).Split(' ');
				string command = $"insert into Traders values ('{name[0]}', '{name[1]}')";
				SqlCommand com = new SqlCommand(command, conn);
				com.ExecuteNonQuery();
			}

			for (int i = 0; i < 20; i++)
			{
				string[] name = NameGenerator.Generate(Gender.Female).Split(' ');
				string command = $"insert into Buyers values ('{name[0]}', '{name[1]}')";
				SqlCommand com = new SqlCommand(command, conn);
				com.ExecuteNonQuery();
			}

			Random rnd = new Random();
			for (int i = 0; i < 10; i++)
			{
				DateTime date = new DateTime(rnd.Next(0, 19) + 2000, rnd.Next(1, 13), rnd.Next(1, 29));
				string d = date.ToString("yyyy-MM-dd");
				string command = $"insert into Sales values ({rnd.Next(1, 21)}, {rnd.Next(1, 21)}, {rnd.Next(10, 100) * 100}, '{d}')";
				SqlCommand com = new SqlCommand(command, conn);
				com.ExecuteNonQuery();
			}
		}
		private void ShowMessage(string mess, Color col)
		{
			ListBoxItem item = new ListBoxItem()
			{
				Content = mess,
				Foreground = new SolidColorBrush(col)
			};
			lbConsole.Items.Add(item);
		}

	}
}
