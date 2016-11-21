using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using PebbleClient.DataTypes;

namespace PebbleClient
{
    /// <summary>
    /// Interaction logic for TransactionHistory.xaml
    /// </summary>
    public partial class TransactionHistory : Window
    {

        public List<Transaction> TransactionList
        {
            get { return TransactionLog.TransactionList; }
        }

        public TransactionHistory()
        {
            InitializeComponent();

            (Grid.FindName("HistoryGrid") as DataGrid).ItemsSource = TransactionLog.TransactionList;
            var grid = Grid.FindName("HistoryGrid") as DataGrid;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
