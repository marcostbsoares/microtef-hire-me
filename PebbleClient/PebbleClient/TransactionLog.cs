using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using PebbleClient.DataTypes;

namespace PebbleClient
{
    public static class TransactionLog
    {
        public static List<Transaction> TransactionList = new List<Transaction>();

        /// <summary>
        /// Loads stored past transactions from XML file
        /// </summary>
        public static void LoadTransactionLog()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Transaction>));
            XmlReader xmlReader = XmlReader.Create("transactionLog.xml");

            try
            {
                TransactionList = (List<Transaction>)serializer.Deserialize(xmlReader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading transaction log :\n" + e.Message);
            }
            finally
            {
                xmlReader.Close();
            }
        }


        /// <summary>
        /// Serializes transactions from current session into XML file
        /// </summary>
        public static void SaveTransactionLog()
        {
            var xmlWriter = XmlWriter.Create("transactionLog.xml");
            var serializer = new XmlSerializer(typeof(List<Transaction>));

            if (!File.Exists("transactionLog.xml"))
                File.Create("transactionLog.xml");

            try
            {
                serializer.Serialize(xmlWriter, TransactionList);

            }
            catch (Exception e)
            {
                MessageBox.Show("Error writing transactionLog.xml : \n" + e.Message);
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }
        }
    }
}
