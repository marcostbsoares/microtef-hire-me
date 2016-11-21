using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Serialization;
using PebbleClient.DataTypes;

namespace PebbleClient
{
    /// <summary>
    /// Interaction logic for TransactionInput.xaml
    /// </summary>
    public partial class TransactionInput : Window
    {
        private Transaction transactionData;

        //Server IP - Replace with remote server IP when needed
        private IPAddress serverIP = IPAddress.Parse("127.0.0.1");

        public TransactionInput()
        {
            InitializeComponent();
            transactionData = new Transaction();
            DataContext = this;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //Validate Data
            bool success = true;

            success &= ValidateName();
            success &= ValidateDates();
            success &= ValidateBrand();
            success &= ValidateType();
            success &= ValidateCardNumber();
            success &= ValidatePassword();
            success &= ValidateInstallments();
            success &= ValidateTransactionType();
            success &= ValidateTransactionValue();

            //Send request if all validation successful
            if (success)
            {
                //ConfirmButton.IsEnabled = false;
                new Thread(SendTransaction).Start();
            }
            //Show error message otherwise
            else
            {
                MessageBox.Show("Errors found. Transaction Canceled.\nCheck your inputs and try again");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Sends validated Transaction Data to Server
        /// </summary>
        private void SendTransaction()
        {
            //Disable confirm button until server returns
            var returnCode = "";
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(IPAddress.Parse("127.0.0.1"), 8888);

                socket.Send(Encoding.UTF8.GetBytes("RequestConnect"));
                byte[] dataBuffer = new byte[4096];

                //Receive content back from server
                socket.Receive(dataBuffer);
                if (Encoding.UTF8.GetString(dataBuffer).Contains("RequestAccept"))
                {
                    //Serialize transaction data as Xml and send to server
                    XmlSerializer serializer = new XmlSerializer(typeof (Transaction));
                    Console.WriteLine("Sending Xml transaction");
                    var stream = new StringWriter();
                    serializer.Serialize(stream, transactionData);
                    socket.Send(Encoding.UTF8.GetBytes(stream.ToString()));
                    Console.WriteLine("Sending transaction sent successfully");
                    
                    //Receive return code
                    dataBuffer = new byte[24];
                    socket.Receive(dataBuffer);
                    returnCode = Encoding.UTF8.GetString(dataBuffer);
                    returnCode = returnCode.Replace("\0", "");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Connection error : " + e.Message);
                return;
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

            }
            //Save transaction after send, but erase password for security and privacy purposes
            transactionData.Card.Password = "";
            transactionData.TransactionStatus = returnCode;
            TransactionLog.TransactionList.Add(transactionData);
            TransactionLog.SaveTransactionLog();

            //Reinstance Transaction to enable new transactions
            transactionData = new Transaction();

            //Restore Confirm button
            //ConfirmButton.IsEnabled = true;

            MessageBox.Show("Transaction Status : " + returnCode);
        }

        #region Data Validation

        bool ValidateName()
        {
            var name = NameBox.Text;

            Regex nameTester = new Regex(@"^[\w.']{2,}(\s[\w.']{2,})+$");

            if (nameTester.IsMatch(name))
            {
                transactionData.Card.CardholderName = name;
                return true;
            }

            MessageBox.Show("Invalid Name!");

            return false;
        }

        bool ValidateDates()
        {
            var monthBox = (TextBox)FindName("Month");
            var yearBox = (TextBox)FindName("Year");
            int month, year;
            if (int.TryParse(monthBox.Text, out month) && int.TryParse(yearBox.Text, out year))
            {
                if (month >= 0 && month <= 12 && year >= 2016)
                {
                    transactionData.Card.ExpirationMonth = month;
                    transactionData.Card.ExpirationYear = year;
                    return true;
                }
            }

            MessageBox.Show("Invalid Expiration Date");
            return false;
        }

        bool ValidateBrand()
        {

            if (String.IsNullOrEmpty(CardBrandComboBox.Text))
            {
                MessageBox.Show("Please select a card brand");
                return false;
            }

            transactionData.Card.CardBrand = CardBrandComboBox.Text;
            return true;
        }

        bool ValidateType()
        {
            var cardType = CardTypeComboBox.Text;

            if (String.IsNullOrEmpty(cardType))
            {
                MessageBox.Show("Please select the type of your card");
                return false;
            }

            transactionData.Card.CardType = cardType;
            return true;
        }

        bool ValidateCardNumber()
        {
            var cardNumber = CardNumberBox.Text;
            Regex regex = new Regex(@"\d{12,19}$");
            if (regex.IsMatch(cardNumber))
            {
                transactionData.Card.CardNumber = cardNumber;
                return true;
            }

            MessageBox.Show("Invalid Card Number");
            return false;
        }

        bool ValidatePassword()
        {
            //Password validation ignored. Password is hashed and sent to server as is for server-side validation
            string passwordHash = SHA256(PasswordBox.Password);
            transactionData.Card.Password = passwordHash;

            return true;
        }

        bool ValidateInstallments()
        {
            int installments;

            if (int.TryParse(InstallmentsBox.Text, out installments))
            {
                transactionData.Number = installments;
                return true;
            }

            MessageBox.Show("Invalid Number of Installments");
            return false;

        }

        bool ValidateTransactionType()
        {
            var transactionType = TransactionTypeBox.Text;
            if (string.IsNullOrEmpty(transactionType))
            {
                MessageBox.Show("Please select a transaction type");
                return false;
            }

            transactionData.TransactionType = transactionType;
            return true;
        }

        bool ValidateTransactionValue()
        {
            var valueBox = (TextBox)FindName("TransactionValueBox");
            double value;
            if (double.TryParse(valueBox.Text, out value))
            {
                transactionData.Amount = value;
                return true;
            }

            MessageBox.Show("Invalid transaction amount!");
            return false;
        }

        #endregion

        /// <summary>
        /// Encrypts a string using SHA-256 algorithm
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        static string SHA256(string password)
        {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

    }
}
