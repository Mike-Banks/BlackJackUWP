using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//CLASS AUTHOR: Michael Banks
namespace BlackJackApp.DataTypes
{

    /// <summary>
    /// Class that inherits from the User Class - Used for creating a Blackjack Player
    /// </summary>
    class BlackJackUser : User
    {

        /// <summary>
        /// Field Variable used to store the amount of chips the player has
        /// </summary>
        private int _gameMoney;

        /// <summary>
        /// Field Variable used to store the player's score (hand value)
        /// </summary>
        private int _score;

        /// <summary>
        /// Field Variable used to store the player's soft-hand score (if they have a soft-ace)
        /// </summary>
        private int _softHandValue;

        /// <summary>
        /// Field Variable used to keep track of the amount of aces the player has drawn (in current hand)
        /// </summary>
        private int _aceCount;

        /// <summary>
        /// Constructor that initializes the base class field variables as well as the new ones
        /// </summary>
        /// <param name="name">the name of the player</param>
        /// <param name="money">the amount of cash the player has</param>
        /// <param name="loses">the amount of rounds the player has lost</param>
        /// <param name="wins">the amount of rounds the player has won</param>
        /// <param name="gameMoney">the amount of chips the player has</param>
        /// <param name="bet">the amount of chips the player has bet</param>
        public BlackJackUser(string name, int money, int loses, int wins, int gameMoney, int bet) : base(name, money, loses, wins)
        {
            _gameMoney = gameMoney;
            _score = 0;
            _softHandValue = 0;
            _aceCount = 0;
        }

        /// <summary>
        /// Property used to access and modify the amount of chips the player has
        /// </summary>
        public int GameMoney
        {
            get { return _gameMoney; }
            set { _gameMoney = value; }
        }

        /// <summary>
        /// Property used to access and modify the player's score
        /// </summary>
        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }

        /// <summary>
        /// Property used to access and modify the soft-hand value of the player
        /// </summary>
        public int SoftHandValue
        {
            get { return _softHandValue; }
            set { _softHandValue = value; }
        }

        /// <summary>
        /// Property used to access and modify the ace counter of the player
        /// </summary>
        public int AceCount
        {
            get { return _aceCount; }
            set { _aceCount = value; }
        }

        /// <summary>
        /// Method used to read (load) from a file
        /// </summary>
        /// <param name="reader">reader object that reads from the file</param>
        public void Load(StreamReader reader)
        {
            //set the values of the field variables to the files contents, line by line
            _name = reader.ReadLine();
            _money = int.Parse(reader.ReadLine());
            _gameMoney = int.Parse(reader.ReadLine());
            _numWins = int.Parse(reader.ReadLine());
            _numLoses = int.Parse(reader.ReadLine());
        }

        /// <summary>
        /// Method used to write (save) to a file
        /// </summary>
        /// <param name="writer">writer object that writes to the file</param>
        public void Save(StreamWriter writer)
        {
            //write the values of the field variables to the file, line by line
            writer.WriteLine(_name);
            writer.WriteLine(_money);
            writer.WriteLine(_gameMoney);
            writer.WriteLine(_numWins);
            writer.WriteLine(_numLoses);
        }
    }
}
