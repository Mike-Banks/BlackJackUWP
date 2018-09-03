using BlackJackApp.DataPersistence;
using BlackJackApp.DataTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

//CLASS AUTHOR: Michael Banks
namespace BlackJackApp.BusinessLogic
{
    class BlackJackGame : CardGame
    {

        #region Field Variables / Constructor / Properties

        /// <summary>
        /// Field Variable used for an instantiation of a blackjack player
        /// </summary>
        private BlackJackUser _player;

        /// <summary>
        /// Field Variable used for an instantiation of a blackjack dealer
        /// </summary>
        private BlackJackDealer _dealer;

        /// <summary>
        /// Constructor used to initialize the player and dealer field variables
        /// </summary>
        public BlackJackGame() : base(10)
        {
            //default player and dealer
            _player = new BlackJackUser("", 0, 0, 0, 0, 0);
            _dealer = new BlackJackDealer("Dealer Dan");
        }

        /// <summary>
        /// Property used to access the player field from outside of the class
        /// </summary>
        public BlackJackUser Player
        {
            get { return _player; }
            set { _player = value; }
        }

        /// <summary>
        /// Property used to access the dealer field from outside of the class
        /// </summary>
        public BlackJackDealer Dealer
        {
            get { return _dealer; }
            set { _dealer = value; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method used to set the card value for cards (namely aces, jacks, queens and kings)
        /// </summary>
        /// <param name="card">usess a card object to change the value (if need be)</param>
        /// <returns>modified (or original in most cases) value of the card</returns>
        private int SetCardValue(Card card)
        {
            //initialize the value variable to 0
            int cardValue = 0;

            //if the card being passed is a jack, queen, or king
            if (card.CardName == "Jack" || card.CardName == "Queen" || card.CardName == "King")
            {
                //set the value of the card to be 10 (blackjack rules)
                cardValue = 10;
            }

            //otherwise, the value of the card is its default value (as defined in the card class)
            else
            {
                cardValue = card.Value;
            }

            //return the value of the card
            return cardValue;
        }

        /// <summary>
        /// Method used to determine if there is an ace in the initial hands of the dealer or player
        /// </summary>
        /// <param name="firstCard">the first card from the dealer or user's hand</param>
        /// <param name="secondCard">the second card from the dealer or user's hand</param>
        /// <returns>string value stating if there is an ace, and if there is it states which card in the hand has it</returns>
        private string InitialAceCheck(Card firstCard, Card secondCard)
        {
            //if the first card is an ace and the second card isn't
            if (SetCardValue(firstCard) == 1 && SetCardValue(secondCard) != 1)
            {
                return "first";
            }

            //if the second card is an ace and the first card isn't
            else if (SetCardValue(firstCard) != 1 && SetCardValue(secondCard) == 1)
            {
                return "second";
            }

            //if both cards are aces
            else if (SetCardValue(firstCard) == 1 && SetCardValue(secondCard) == 1)
            {
                return "both";
            }

            //if there is no ace in the hand
            return "neither";
        }

        /// <summary>
        /// Method used to perform actions based on the outcome of the 'InitialAceCheck' method
        /// </summary>
        private void DetermineInitialAce()
        {
            //if the player has an ace as their first card only
            if (InitialAceCheck(_player.Hand[0], _player.Hand[1]) == "first")
            {
                // add the soft ace value (11) to the value of the second card to get the soft-hand value
                _player.SoftHandValue = SetCardValue(_player.Hand[1]) + 11;
                _player.AceCount++;
            }

            //if the player has an ace as their second card only
            else if (InitialAceCheck(_player.Hand[0], _player.Hand[1]) == "second")
            {
                //add the soft ace value (11) to the value of the first card to get the soft-hand value
                _player.SoftHandValue = SetCardValue(_player.Hand[0]) + 11;
                _player.AceCount++;
            }

            //if the player has two aces
            else if (InitialAceCheck(_player.Hand[0], _player.Hand[1]) == "both")
            {
                //treat one of the aces as a soft ace, the other as a hard ace
                _player.SoftHandValue = 1 + 11;
                _player.AceCount++;
            }

            //if the player has no aces
            else
            {
                //they have no soft-hand value
                _player.SoftHandValue = 0;
            }

            //if the dealer has an ace as their first card only
            if (InitialAceCheck(_dealer.Hand[0], _dealer.Hand[1]) == "first")
            {
                //add the soft ace value (11) to the value of their second card to get the soft-hand value
                _dealer.SoftHandValue = SetCardValue(_dealer.Hand[1]) + 11;
                _dealer.AceCount++;
            }

            //if the dealer has an ace as their second card only
            else if (InitialAceCheck(_dealer.Hand[0], _dealer.Hand[1]) == "second")
            {
                //add the soft ace value (11) to the value of the first card to get the soft-hand value
                _dealer.SoftHandValue = SetCardValue(_dealer.Hand[0]) + 11;
                _dealer.AceCount++;
            }

            //if the dealer has two aces
            else if (InitialAceCheck(_dealer.Hand[0], _dealer.Hand[1]) == "both")
            {
                //treat one of the aces as a soft ace, the other as a hard ace
                _dealer.SoftHandValue = 1 + 11;
                _dealer.AceCount++;
            }

            //if the dealer has no aces
            else
            {
                //they have no soft-hand value
                _dealer.SoftHandValue = 0;
            }
        }

        /// <summary>
        /// Method used to determine if the dealer or player have blackjack
        /// </summary>
        /// <returns>string value stating whether the player, dealer, both or neither have blackjack</returns>
        public string DetermineBlackJack()
        {
            //if the player has blackjack
            if (_player.SoftHandValue == 21 && _player.Hand.Count == 2)
            {
                //set the player score to 21 (this value is previously stored in the soft-hand value
                _player.Score = 21;

                //if the dealer also has blackjack
                if (_dealer.SoftHandValue == 21 && _dealer.Hand.Count == 2)
                {
                    //set the dealer score to 21
                    _dealer.Score = 21;

                    //both the player and dealer got blackjack
                    return "draw";
                }

                //only the player got blackjack
                return "player";
            }

            //if the dealer has blackjack
            else if ((_dealer.SoftHandValue == 21 && _dealer.Hand.Count == 2) && _player.SoftHandValue != 21)
            {
                //set the dealer score to 21
                _dealer.Score = 21;

                //only the dealer has blackjack
                return "dealer";
            }

            //neither the player or dealer got blackjack
            return "none";
        }

        /// <summary>
        /// Method used to set the initial hands of the player and dealer
        /// </summary>
        private void InitialHands()
        {

            //gives the player their first two cards
            _player.Hand.AddRange(CardDeck.DrawCards(2, true));

            //loop through the player's hand
            for (int iHand = 0; iHand < _player.Hand.Count; iHand++)
            {
                //add the value of the cards to the player's score
                _player.Score += SetCardValue(_player.Hand[iHand]);
            }
            
            //give the dealer their two cards, one of them being face-down
            _dealer.Hand.Add(CardDeck.DrawCard(true));
            _dealer.Hand.Add(CardDeck.DrawCard(false));

            //loop through the dealer's hand
            for (int iHand = 0; iHand < _dealer.Hand.Count; iHand++)
            {
                //add the value of the cards to the dealer's score
                _dealer.Score += SetCardValue(_dealer.Hand[iHand]);
            }

            // Call methods to determine if there is an ace is either of the hands, or if the player or dealer got blackjack
            DetermineInitialAce();
            DetermineBlackJack();
        }

        /// <summary>
        /// Method used to add player cards
        /// </summary>
        private void AddPlayerCards()
        {
            //adds a new card from the deck to the player's hand
            _player.Hand.Add(CardDeck.DrawCard(true));

            //set the card drawn from the deck to a new card object
            Card card = _player.Hand[_player.Hand.Count - 1];

            //if the card is an ace
            if (SetCardValue(card) == 1)
            {
                //if an ace is already being used as an 11 (soft ace)
                if (_player.AceCount != 0 && _player.SoftHandValue != 0)
                {
                    //use the hard value of the ace (add 1)
                    _player.SoftHandValue += 1;
                }

                //if this is the first ace that has been pulled
                else
                {
                    _player.AceCount++;

                    //if the current player score is less than or equal to 10
                    if (_player.Score <= 10)
                    {
                        //add 11 
                        _player.SoftHandValue = _player.Score + 11;
                    }

                    //if the current player score is greater than 10
                    else
                    {
                        _player.SoftHandValue = 0;
                    }
                }
            }

            // if the card is not an ace
            else
            {

                //if the player already has a soft-hand value 
                if (_player.AceCount != 0 && _player.SoftHandValue != 0)
                {
                    //add the value of the card to the soft score
                    _player.SoftHandValue += SetCardValue(card);

                    //if the soft score of the player is over 21
                    if (_player.SoftHandValue > 21)
                    {
                        //they no longer have a soft score, they must use the ace they had as a hard ace
                        _player.SoftHandValue = 0;
                    }
                }
            }

            // add the value of the card to the player's score
            _player.Score += SetCardValue(card);
        }

        /// <summary>
        /// Method used to add dealer cards
        /// </summary>
        private void AddDealerCards()
        {
            //add a card from the deck to the dealer's hand 
            _dealer.Hand.Add(CardDeck.DrawCard(true));

            //set the card drawn as the value of a new card object
            Card card = _dealer.Hand[_dealer.Hand.Count - 1];
            
            //if the card drawn is an ace
            if (SetCardValue(card) == 1)
            {
                //if the dealer already has a soft-hand value
                if (_dealer.AceCount != 0 && _dealer.SoftHandValue != 0)
                {
                    //use the ace drawn as a hard ace (use value of 1)
                    _dealer.SoftHandValue += 1;
                }

                //if the dealer does not have an ace already
                else
                {
                    _dealer.AceCount++;

                    //if the dealer score is less than or equal to 10
                    if (_dealer.Score <= 10)
                    {
                        //make the soft-hand value of the dealer to be the score of the player plus the soft value of the ace
                        _dealer.SoftHandValue = _dealer.Score + 11;
                    }

                    //if the dealer score is above 10
                    else
                    {
                        //the ace must be treated as a hard ace, there is no soft-hand value
                        _dealer.SoftHandValue = 0;
                    }
                }
            }

            //if the dealer did not draw an ace
            else
            {
                //if the dealer has a soft-hand value
                if (_dealer.AceCount != 0 && _dealer.SoftHandValue != 0)
                {
                    //add the value of the card to the soft-hand value
                    _dealer.SoftHandValue += SetCardValue(card);

                    //if the soft-hand value is over 21
                    if (_dealer.SoftHandValue > 21)
                    {
                        //there is no soft-hand value, the ace in the hand must be treated as a hard ace
                        _dealer.SoftHandValue = 0;
                    }
                }
               
            }

            //add the value of the card to the score of the dealer
            _dealer.Score += SetCardValue(card);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method that runs when the player starts the game
        /// </summary>
        public void StartRound()
        {
            //call the Restart method to reset the game 
            Restart();

            //call the method to get the initial hands of the player and dealer
            InitialHands();
        }
        
        /// <summary>
        /// Method that resets variables to prepare for a new round
        /// </summary>
        public override void Restart()
        {
            //create a new card deck and shuffle the cards
            CardDeck = new CardDeck();
            CardDeck.ShuffleCards();

            //reset the hand, score, soft-hand value and ace count of the player
            _player.Hand.Clear();
            _player.Score = 0;
            _player.SoftHandValue = 0;
            _player.AceCount = 0;

            //reset the hand, score, soft-hand value and ace count of the player
            _dealer.Hand.Clear();
            _dealer.Score = 0;
            _dealer.SoftHandValue = 0;
            _dealer.AceCount = 0;
        }

        /// <summary>
        /// Method that determiness the winner of the round
        /// </summary>
        /// <returns>string value stating if the player won, the dealer won, or it was a draw</returns>
        public string DetermineWinner()
        {
            //if the soft-hand value of the player is less than or equal to 21
            if (_player.SoftHandValue > _player.Score && _player.SoftHandValue <= 21)
            {
                //set the score of the player to be the soft-hand value
                _player.Score = _player.SoftHandValue;
            }

            //if the soft-hand value of the dealer is less than or equal to 21
            if (_dealer.SoftHandValue > _dealer.Score && _dealer.SoftHandValue <= 21)
            {
                //set the score of the dealer to be the soft-hand value
                _dealer.Score = _dealer.SoftHandValue;
            }

            //if the player score is greater than the dealer's score or the dealer busted
            if (_player.Score > _dealer.Score || _dealer.Score > 21)
            {
                //if the player busted
                if (_player.Score > 21)
                {
                    //the dealer wins the round - this is because if the player busts, they automatically lose
                    return "DEALER WINS";
                }

                //the player wins the round
                return "PLAYER WINS";
            }

            //if the dealer score is greater than the player's score or the player busted
            else if (_dealer.Score > _player.Score || _player.Score > 21)
            {
                //if the dealer busted
                if (Dealer.Score > 21)
                {
                    //the player wins the round
                    return "PLAYER WINS";
                }

                //the dealer wins the round
                return "DEALER WINS";
            }

            //if the hand values are equal, the round ends in a draw
            return "DRAW";
        }

        /// <summary>
        /// Method used to determine the payout of the player
        /// </summary>
        /// <returns>an integer value that stores the winnings of the player</returns>
        public int Payout()
        {
            //if the player won the round
            if (DetermineWinner() == "PLAYER WINS")
            {
                //if the player got blackjack
                if (DetermineBlackJack() == "player")
                {
                    //the player gets triple what they bet
                     return _player.GameMoney += _player.Bet * 3;
                }

                //they player gets double what they bet
                return _player.GameMoney += _player.Bet * 2;
            }

            //if the dealer won
            else if (DetermineWinner() == "DEALER WINS")
            {
                //the player does not receive any winnings (they lost their bet)
                return _player.GameMoney;
            }

            //if the round ends in a draw
            else
            {
                //the player receives their money back
                return _player.GameMoney += _player.Bet;
            }
        }

        /// <summary>
        /// Method that runs when the user holds
        /// </summary>
        /// <returns>true if the dealer's score is below 17 (are able to hit)</returns>
        public bool Hold()
        {
            //if the soft hand value of the player is greater than the score
            if (_player.SoftHandValue > _player.Score)
            {
                //set the socre of the player to be the soft-hand score - this will be the highest score the player has on their hand
                _player.Score = _player.SoftHandValue;
            }            
            
            //if the dealer's score is less than 17
            if (_dealer.Score < 17)
            {
                //they are able to hit, add dealer cards
                AddDealerCards();

                return true;
            }

            //this code runs if the dealers score is already above 17
            return false;
        }

        /// <summary>
        /// Method that runs when the user hits
        /// </summary>
        /// <returns>true if the player is able to hit</returns>
        public bool Hit()
        {
            //if the player has not reached a score of 21
            if (_player.Score <= 21)
            {
                //deal the player another card
                AddPlayerCards();

                return true;
            }

            //if the player is above a score of 21
            return false;
        }

        /// <summary>
        /// Method that runs when the usee doubles
        /// </summary>
        /// <returns>true if the player is able to double down</returns>
        public bool Double()
        {
            //if the player is able to hit and they have chips to bet
            if (Hit() == true && _player.GameMoney > 0)
            {
                //remove the chips from the player, add the chips to their bet pile
                _player.GameMoney -= _player.Bet;
                _player.Bet *= 2;
               
                return true;
            }

            //returns false if the player cannot double down
            return false;
        }

        /// <summary>
        /// Method that saves the game's player
        /// </summary>
        /// <param name="serializer">serializer object</param>
        public void Save(BlackJackTextSerializer serializer)
        {
            //set the serialier's player values to be the player's values from the game
            serializer.Player = _player;

            //save the values
            serializer.Save();
        }

        /// <summary>
        /// Method that loads the game's player
        /// </summary>
        /// <param name="_serializer">serializer object</param>
        public void Load(BlackJackTextSerializer _serializer)
        {
            //set the serializer's player values to be the player from the game (should be default values)
            _serializer.Player = _player;

            //load the values
            _serializer.Load();

            //set the values of the player to be the loaded values of the serializer's player
            _player = _serializer.Player;
        }

        #endregion

    }
}
