using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//CLASS AUTHOR: Michael Banks
namespace BlackJackApp.DataTypes
{

    /// <summary>
    /// Class that inherits from the Player class - Used for creating a Blackjack Dealer
    /// </summary>
    class BlackJackDealer : Player
    {

        /// <summary>
        /// Field Variable used to store the dealer's score (hand value)
        /// </summary>
        private int _score;

        /// <summary>
        /// Field Variable used to store the dealer's soft-hand score (if they have a soft-ace)
        /// </summary>
        private int _softHandValue;

        /// <summary>
        /// Field Variable used to keep track of the amount of aces the dealer has drawn (in current hand)
        /// </summary>
        private int _aceCount;

        /// <summary>
        /// Constructor that initializes the base class field variables as well as the new ones
        /// </summary>
        /// <param name="name">the name of the dealer</param>
        public BlackJackDealer(string name) : base(name)
        {
            _score = 0;
            _softHandValue = 0;
            _aceCount = 0;
        }

        /// <summary>
        /// Property used to access and modify the dealer's score
        /// </summary>
        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }

        /// <summary>
        /// Property used to access and modify the soft-hand value of the dealer
        /// </summary>
        public int SoftHandValue
        {
            get { return _softHandValue; }
            set { _softHandValue = value; }
        }

        /// <summary>
        /// Property used to access and modify the ace counter of the dealer
        /// </summary>
        public int AceCount
        {
            get { return _aceCount; }
            set { _aceCount = value; }
        }
    }
}
