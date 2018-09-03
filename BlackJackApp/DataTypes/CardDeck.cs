using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp.DataTypes
{
    /// <summary>
    /// Represents a card deck which contains a list of cards that are to be played
    /// in a card game. The Card Deck allows access to the cards and implements basic
    /// operations of extracting, adding, shuffling, exchanging cards.
    /// </summary>
    class CardDeck
    {
        /// <summary>
        /// The list of cards that are in the deck to be played
        /// (read as "List of Cards")
        /// </summary>
        protected List<Card> _cardList;

        /// <summary>
        /// Randomizer used to extract cards from the deck
        /// </summary>
        private static Random s_randomizer;

        protected const int MAX_SUIT_COUNT = 4;
        protected const int MAX_CARD_VALUE = 13;

        public Random Randomizer
        {
            get { return s_randomizer; }
        }

        public List<Card> CardList
        {
            get { return _cardList; }
            set { _cardList = value; }
        }
        /// <summary>
        /// Constructor for CardDeck objects, creates a full card deck of
        /// 52 cards shuffled
        /// </summary>
        /// 
        public CardDeck()
        {
            _cardList = new List<Card>();



            //fill the card list with card objects
            CreateCards();
            //Shuffle the Cards
            // ShuffleCards();
        }

        static CardDeck()
        {
            s_randomizer = new Random();
        }

        /// <summary>
        /// The number of cards left in the deck.
        /// </summary>
        public int CardCount
        {
            get { return _cardList.Count; }
        }

        /// <summary>
        /// Creates a complete deck with all the cards of every suit
        /// </summary>
        protected virtual void CreateCards()
        {
            //go through every suit to create cards of that suit
            for (byte value = 1; value <= MAX_CARD_VALUE; value++)
            {


                //create the cards for the current suit. How?
                //go through every card value and create cards
                for (int iSuit = 1; iSuit <= MAX_SUIT_COUNT; iSuit++)
                {
                    //obtain the suit for the current index
                    CardSuit suit = (CardSuit)iSuit;

                    //create the card
                    Card card = new Card(value, suit);

                    //add the card to the list of the deck
                    _cardList.Add(card);
                }
            }
        }

        /// <summary>
        /// Shuffles the card deck using a Fisher-Yates shuffle algorithm
        /// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        /// </summary>
        public void ShuffleCards()
        {


            for (int cardIndex = 0; cardIndex < CardCount; cardIndex++)
            {
                int swapIndex = Randomizer.Next(cardIndex, CardCount);
                Card card = _cardList[cardIndex];
                _cardList[cardIndex] = _cardList[swapIndex];
                _cardList[swapIndex] = card;

            }
        }

        /// <summary>
        /// Prints the deck of cards in the order the cards are found
        /// </summary>
        public string PrintCards()
        {
            //TODO: use a regular for loop to go through all the cards
            //in the list of cards N(_cardList) and create a string
            //output and return it
            string output = "";

            foreach (Card card in _cardList)
            {
                //accumulate the cards in the output variable
                output += $"{card}\n";
            }

            return output;
        }

        public Card DrawCard(Boolean faceUp)
        {
            Card card = _cardList[0];
            _cardList.Remove(card);
            if (faceUp)
                card.FaceUp = true;
            return card;
        }

        public Card[] DrawCards(int amount, Boolean faceup)
        {
            Card[] cards = _cardList.Take(amount).ToArray();
            _cardList.RemoveRange(0, amount);
            if (faceup)
            {
                foreach (Card card in cards)
                {
                    card.FaceUp = true;
                }
            }
            return cards;
        }

        //Implement ExchangeCards that receives two cards and exchanges them with each other
        public void ExchangeCards(ref Card cardOne, ref Card cardTwo)
        {
            Card temp = cardOne;
            cardOne = cardTwo;
            cardTwo = temp;

        }
    }
}
