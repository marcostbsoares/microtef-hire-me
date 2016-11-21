using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.Xml.Serialization;
using PebbleClient.DataTypes;
using PebbleShared.DataTypes;

namespace PebbleServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextWriter consoleWriter;
        private SqlConnection dbConnection;

        private TcpListener tcpListener;

        public MainWindow()
        {

            InitializeComponent();
            consoleWriter = new ConsoleWriter(TextBox);
            Console.SetOut(consoleWriter);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            //Initialize SQL Connection
            //var directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //try
            //{
            //    dbConnection =
            //        new SqlConnection(
            //            "Data Source=(LocalDB)\\v11.0;AttachDbFilename=\"C:\\Users\\Marcos T\\Documents\\Visual Studio 2013\\Projects\\Apps\\PebbleClient\\PebbleServer\\Database.mdf\";Integrated Security=True");

            //    dbConnection.Open();
            //}
            //catch
            //{

            //}

            //Initialize Server and TCP Listener
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            tcpListener.Start();
            new Thread(StartServer).Start();
        }

        /// <summary>
        /// Starts a server connection to accept and reply to remote requests
        /// </summary>
        void StartServer()
        {
            Socket socket = null;

            try
            {

                //Accept remote inbound connection
                Console.WriteLine("Listener created. Awaiting Connection...");

                socket = tcpListener.AcceptSocket();

                Console.WriteLine("Connection received from {0}", socket.RemoteEndPoint);


                while (true)
                {
                    byte[] data = new byte[4096];
                    socket.Receive(data);

                    string receivedString = Encoding.UTF8.GetString(data);
                    //Console.WriteLine("Data received: {0}", receivedString);

                    if (receivedString.Contains("RequestConnect"))
                    {

                        //Receive handshake and send answer
                        socket.Send(Encoding.UTF8.GetBytes("RequestAccept"));
                        Console.WriteLine("Connection Accepted");

                        //Receive transaction data in Xml format and deserialize
                        socket.Receive(data);
                        StringReader stringReader = new StringReader(Encoding.UTF8.GetString(data));
                        Transaction transaction = (Transaction)new XmlSerializer(typeof(Transaction)).Deserialize(stringReader);

                        Console.WriteLine("Transaction data received.");

                        //Validate transaction and generate return code
                        string returnCode = ValidateTransaction(transaction);
                        socket.Send(Encoding.UTF8.GetBytes(returnCode));

                        Console.WriteLine("Return code {0} sent back to client", returnCode);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Socket error: {0}", e.Message);
            }
            finally
            {
                //Close socket and restart server
                if (socket != null)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    Console.WriteLine("Socket closed. Restarting server");
                }
                Thread.Sleep(1000);
                new Thread(StartServer).Start();
            }
        }

        private string ValidateTransaction(Transaction transaction)
        {
            if (transaction.Amount < 0.10d)
                return "ERROR_UnderMinValue";

            if (IsCardBlocked(transaction.Card))
                return "ERROR_BlockedCard";

            if (transaction.TransactionType == "Credit" && !HasCredit(transaction))
                return "ERROR_InsufficientCredit";

            if (transaction.TransactionType == "Debit" && !HasBalance(transaction))
                return "Error_InsufficientBalance";

            if (!ValidatePassword(transaction.Card))
                return "Error_InvalidPassword";

            //Random validation parameters to simulate diverse errors
            Random rand = new Random(System.DateTime.Now.Millisecond);
            if (rand.Next(0, 100) < 5)
                return "ERROR_TransactionDenied";

            return "SUCCESS";
        }


        bool HasBalance(Transaction transaction)
        {
            //TODO: Implement when Database query is implemented
            return true;
        }

        bool HasCredit(Transaction transaction)
        {
            //TODO: Implement when Database query is implemented
            return true;
        }

        bool IsCardBlocked(Card card)
        {
            //TODO: Implement when Database query is implemented
            return false;
        }

        bool ValidatePassword(Card card)
        {
            //TODO: Implement when Database query is implemented
            return true;
        }

    }

    /// <summary>
    /// TextWriter to output System.Console text to WPF Textbox
    /// </summary>
    class ConsoleWriter : TextWriter
    {
        private TextBox textBox;

        public ConsoleWriter(TextBox textBox)
            : base()
        {
            this.textBox = textBox;
        }

        public override void WriteLine(string value)
        {
            base.Write(value);
            AppendText(value + '\n');
        }

        public override void WriteLine(string format, object arg0)
        {
            base.WriteLine(format, arg0);
            AppendText(string.Format(format, arg0) + '\n');
        }

        private void AppendText(string text)
        {
            textBox.Dispatcher.Invoke(() => { textBox.AppendText(text); });

        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
