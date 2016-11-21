using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PebbleShared.DataTypes;

namespace PebbleClient.DataTypes
{
    [Serializable]
    public class Transaction
    {
        public double Amount { get; set; }
        public Card Card { get; set; }
        public string TransactionType { get; set; }
        public string TransactionStatus { get; set; }
        public int Number { get; set; }
        

        public Transaction()
        {
            Card = new Card();
        }
    }
}
