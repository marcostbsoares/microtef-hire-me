using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using PebbleClient.DataTypes;

namespace PebbleClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        

        public App()
        {
            TransactionLog.LoadTransactionLog();   
            //SaveTransactionLog();
        }
        
    }
}
