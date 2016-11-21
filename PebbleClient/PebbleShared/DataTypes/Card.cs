using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PebbleShared.DataTypes
{
    [Serializable]
    public class Card
    {
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string CardholderName { get; set; }
        public string CardType { get; set; }
        public string CardBrand { get; set; }
        public string CardNumber { get; set; }

        protected string password = "";
        /// <summary>
        /// Hashed Password (SHA-256)
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }


        protected bool hasPassword;
        /// <summary>
        /// Only magnetic strip cards can have passwords. Used for server-side verification
        /// </summary>
        public bool HasPassword
        {
            get { return CardType == "Magnetic Strip" && hasPassword; }
            set { hasPassword = value; }
        }

        //Return holder name and card number with only the first and last 3 digits
        public override string ToString()
        {

            string finalNumber = CardholderName + " - ";
            finalNumber += CardNumber.Substring(0, 3);
            finalNumber += "********";
            finalNumber += CardNumber.Substring(CardNumber.Length - 3, 3);



            return finalNumber;

        }
    }
}
