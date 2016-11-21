using PebbleClient.DataTypes;
using System;
using System.Collections.Generic;
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
using System.Xml;
using System.Xml.Serialization;

namespace PebbleClient
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
        
        //On click New Transaction button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TransactionInput inputWindow = new TransactionInput();
            inputWindow.Show();

            //Hide main window while transaction window is open
            Hide();

            //Subscribe to event to reopen main window when other window closes
            inputWindow.Closed += OnWindowClose;
        }


        private void OnWindowClose(object sender, EventArgs e)
        {
            //Reopen window and unsubscribe from event
            Show();
            (sender as Window).Closed -= OnWindowClose;
        }


        //On click history button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TransactionHistory historyWindow = new TransactionHistory();
            historyWindow.Show();

            //Hide main window while transaction window is open
            Hide();

            //Subscribe to event to reopen main window when other window closes
            historyWindow.Closed += OnWindowClose;
        }

        

    }
}
