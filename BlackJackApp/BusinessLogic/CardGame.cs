using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackApp.DataTypes;

namespace BlackJackApp.BusinessLogic
{
    abstract class CardGame
    {
        protected int _minBet;
        private CardDeck _cardDeck;
        protected Player _roundWinner;
       
        

        public CardGame(int mBet)
        {
            _cardDeck = new CardDeck();
            _minBet = mBet;
            
            

        }

        public CardDeck CardDeck
        {
            get
            {
                return _cardDeck;
            }

            set
            {
                _cardDeck = value;
            }
        }



        public abstract void Restart();

    }

    
}
