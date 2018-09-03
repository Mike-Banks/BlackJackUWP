using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp.DataTypes
{
   
    class Player
    {
        private List<Card> _hand;
        protected String _name;

        internal List<Card> Hand
        {
            get
            {
                return _hand;
            }

            set
            {
                _hand = value;
            }
        }

        public Player(String name)
        {
            _name = name;
            _hand = new List<Card>();

        }
        public  string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public void TurnCardsFaceUp()
        {
            foreach(Card card in Hand)
            {
                card.FaceUp = true;
            }
        }
    }
}
