using System;
using System.Diagnostics;

namespace BlackJackApp.DataTypes
{
    /// <summary>
    /// Represents the card suit with the values diamonds, hearts etc.
    /// </summary>
    public enum CardSuit
    {
        Diamonds = 1,
        Clubs,
        Hearts,
        Spades
    }

    /// <summary>
    /// Represents a card in a card game with a value and a suit.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// The numeric value of the card
        /// </summary>
        private byte _value;

        /// <summary>
        /// The suit of the card
        /// </summary>
        CardSuit _suit;
        Boolean _faceUp;

        /// <summary>
        /// Constructor to create card objects given a numeric value and suit
        /// </summary>
        /// <param name="value">the numeric value for the created card</param>
        /// <param name="suit">the suit of the created card</param>
        public Card(byte value, CardSuit suit)
        {
            _value = value;
            _suit = suit;
            _faceUp = false;

        }

        /// <summary>
        /// The numeric value of the card (Ace is 1)
        /// </summary>
        public byte Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// The card name based on its value. 1 is Ace, 11 is Jack etc.
        /// </summary>
        /// <returns></returns>
        public string CardName
        {
            get
            {
                //determine the card name based on its value
                string cardName;
                switch (_value)
                {
                    case 1:
                        cardName = "Ace";
                        break;

                    case 11:
                        cardName = "Jack";
                        break;

                    case 12:
                        cardName = "Queen";
                        break;

                    case 13:
                        cardName = "King";
                        break;

                    default:
                        cardName = _value.ToString();
                        break;
                }

                return cardName;
            }
        }

        /// <summary>
        /// The name of the suit. This is transfered from Python which used integer
        /// to represent suit values. As we are using enums will it be necessary?
        /// </summary>
        /// <returns></returns>
        public string SuitName
        {
            get
            {
                //determine the name of the suit
                switch (_suit)
                {
                    case CardSuit.Diamonds:
                    case CardSuit.Clubs:
                    case CardSuit.Hearts:
                    case CardSuit.Spades:
                        return _suit.ToString();

                    default:
                        Debug.Assert(false, "Unexpected suit value");
                        return "N/A";
                }
            }
        }

        public bool FaceUp
        {
            get
            {
                return _faceUp;
            }

            set
            {
                _faceUp = value;
            }
        }

        public string GetFileName()
        {
            string value = _value.ToString();
            if(value.Length == 1)
            {
                value = $"0{value}";
            }
            return $"{_suit.ToString().ToLower()[0].ToString()}{value}.png";
        }
    }
}

