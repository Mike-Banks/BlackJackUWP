using BlackJackApp.BusinessLogic;
using BlackJackApp.DataPersistence;
using BlackJackApp.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//CLASS/PAGE AUTHOR: Michael Banks
namespace BlackJackApp.Presentation
{

    /// <summary>
    /// Class used with the Blackjack Page
    /// </summary>
    public sealed partial class BlackJack : CardPage
    {

        #region Field Variables / Constructor / Properties

        /// <summary>
        /// Field Variable used as an instantiation of a blackjack game
        /// </summary>
        private BlackJackGame _game;

        /// <summary>
        /// Field Variable used as a timer for drawing dealer cards
        /// </summary>
        private DispatcherTimer _tmDealerCardTimer;

        /// <summary>
        /// Field Variable used to store a boolean for whether the game has started or not
        /// </summary>
        private bool _started;

        /// <summary>
        /// Field Variable used to store a boolean for whether the timer has ticked for the first time
        /// </summary>
        private bool _firstTimerTick;

        /// <summary>
        /// Field Variable used to store a blackjack serializer instantiation - used when saving and loading the game
        /// </summary>
        private BlackJackTextSerializer _serializer;

        /// <summary>
        /// Constructor used to initialize field variables and load the game
        /// </summary>
        public BlackJack()
        {
            this.InitializeComponent();

            _game = new BlackJackGame();
            _serializer = new BlackJackTextSerializer();

            LoadGame();

            //initializes the timer settings
            _tmDealerCardTimer = new DispatcherTimer();
            _tmDealerCardTimer.Interval = TimeSpan.FromMilliseconds(1500);
            _tmDealerCardTimer.Tick += OnDealerCardTimerTick;

            _uiDealerScoreBorder.Visibility = Visibility.Collapsed;
            
            //disables all of the buttons by default
            _btnStart.IsEnabled = false;
            _btnHold.IsEnabled = false;
            _btnHit.IsEnabled = false;
            _btnDouble.IsEnabled = false;

            _started = false;
        }

        /// <summary>
        /// Property used for scaling with the grid
        /// </summary>
        public override Grid MainGrid
        {
            get
            {
                return _grid;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method used to perform actions when the round starts - called by the start button on click method
        /// </summary>
        private async void StartRound()
        {
            //start the round
            _game.StartRound();

            _firstTimerTick = true;
            _started = true;

            //disable and close neccessary controls
            _btnOpenPane.IsEnabled = false;
            _btnStart.IsEnabled = false;
            _uiPlayerChip.IsEnabled = false;

            //adjust visibilities and text
            _btnOpenPane.Visibility = Visibility.Collapsed;
            _uiDealerScoreBorder.Visibility = Visibility.Collapsed;
            _txtDealerTotal.Text = "";

            //use delays and draw the hands of the player and dealer
            await Task.Delay(TimeSpan.FromMilliseconds(1500));
            DrawCardsBE(_canvasPlayerHand, _game.Player.Hand, Alignment.Center);
            await Task.Delay(TimeSpan.FromMilliseconds(1500));
            DrawCardsBE(_canvasDealerHand, _game.Dealer.Hand, Alignment.Center);

            //if the player has a soft-hand value
            if (_game.Player.SoftHandValue != 0)
            {
                //if the player has a soft-hand score of 21
                if (_game.Player.SoftHandValue == 21)
                {
                    //show the score of the player on the screen
                    _txtPlayerTotal.Text = _game.Player.SoftHandValue.ToString();
                }

                //if the player has a soft-hand value that is not 21, show their score and soft-hand score beside each othe
                else
                {
                    _txtPlayerTotal.Text = $"{_game.Player.Score.ToString()}/{_game.Player.SoftHandValue.ToString()}";
                }
            }

            //if the player does not have a soft-hand value
            else
            {
                //show their score
                _txtPlayerTotal.Text = _game.Player.Score.ToString();
            }

            //the cards can now scale and be dynamically drawn
            DrawScreen();

            //check if the player or dealer got blackjack
            GameBlackJack();

            //if nobody got blackjack
            if (_game.DetermineBlackJack() == "none")
            {
                //enable the game buttons
                _btnHold.IsEnabled = true;
                _btnHit.IsEnabled = true;

                //if the player has enough money to double down
                if (_game.Player.GameMoney >= _game.Player.Bet)
                {
                    //enable the double down button
                    _btnDouble.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Method used to perform actions when the round ends
        /// </summary>
        /// <param name="endReason">taakes a string value for the reason as to why the round has ended</param>
        private async void EndRound(string endReason)
        {
            //stop the dealer card timer
            _tmDealerCardTimer.Stop();

            //disable the game buttons
            _btnHold.IsEnabled = false;
            _btnHit.IsEnabled = false;
            _btnDouble.IsEnabled = false;

            _uiFinalTextBorder.Visibility = Visibility.Visible;

            //set the displayed message to be the string passed as a parameter
            _txtDisplayOutcome.Text = endReason;

            //delay the program
            await Task.Delay(TimeSpan.FromMilliseconds(3000));

            _started = false;

            //clear the canvases
            _canvasDealerHand.Children.Clear();
            _canvasPlayerHand.Children.Clear();

            _btnOpenPane.IsEnabled = true;
            _btnOpenPane.Visibility = Visibility.Visible;

            //update controls
            _txtGameMoney.Text = (_game.Payout().ToString());
            _txtChipCount.Text = $"CHIP TOTAL: {_game.Player.GameMoney.ToString()}";

            //if the player has chips remaining
            if (_game.Player.GameMoney != 0)
            {
                //draw the player chip stack and allow them to click on them to bet
                _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                _uiPlayerChips.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                _uiPlayerChip.IsEnabled = true;

                //ask the user to bet chips to start the round
                _txtDisplayOutcome.Text = "CLICK ON A CHIP TO MAKE A BET";
            }
            //if the player does not have chips remaining
            else
            {
                //ask them to go to the menu to buy chips
                _txtDisplayOutcome.Text = "CLICK 'USER MENU' TO BUY CHIPS";

                _txtGameMoney.Text = "";
            }

            //save the game at the end of each round
            SaveGame();

            //make the bet area look empty
            _uiBetArea.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            _uiBetArea.Fill = new SolidColorBrush();

            //reset controls and variables
            _uiDealerScoreBorder.Visibility = Visibility.Collapsed;

            _game.Player.Bet = 0;
            _txtBet.Text = "";

            _txtPlayerTotal.Text = "";

        }

        /// <summary>
        /// Method that used the virtual variant from the CardPage class - scales the cards and redraws them
        /// </summary>
        protected override void DrawScreen()
        {
            //if the round has started - dont want to draw the cards before the round has started
            if (_started)
            {
                //scale the cards
                DrawCardsBE(_canvasPlayerHand, _game.Player.Hand, Alignment.Center);
                DrawCardsBE(_canvasDealerHand, _game.Dealer.Hand, Alignment.Center);
            }
        }

        /// <summary>
        /// Method used to perform actions based on whether there is a blackjack in the round
        /// </summary>
        private async void GameBlackJack()
        {
            //if there is a blackjack in the game
            if (_game.DetermineBlackJack() != "none")
            {
                //delay
                await Task.Delay(TimeSpan.FromMilliseconds(1500));

                //turn the dealer's cards face up and show their score
                _game.Dealer.TurnCardsFaceUp();
                DrawCardsBE(_canvasDealerHand, _game.Dealer.Hand, Alignment.Center);
                _uiDealerScoreBorder.Visibility = Visibility.Visible;

                //if the player has a soft-hand value
                if (_game.Player.SoftHandValue != 0)
                {
                    //set the score of the player to be their soft-hand score
                    _game.Player.Score = _game.Player.SoftHandValue;
                }

                //if the dealer has a soft hand value
                if (_game.Dealer.SoftHandValue != 0)
                {
                    //set the score of the dealer to be their soft-hand score
                    _game.Dealer.Score = _game.Dealer.SoftHandValue;
                }

                //update the score controls to display the scores of the player and dealer
                _txtPlayerTotal.Text = _game.Player.Score.ToString();
                _txtDealerTotal.Text = _game.Dealer.Score.ToString();

                //delay
                await Task.Delay(TimeSpan.FromMilliseconds(1500));

                _uiFinalTextBorder.Visibility = Visibility.Visible;

                //depending on who got the blackjack, call the end round method with the appropriate reason for ending the game
                if (_game.DetermineBlackJack() == "player")
                {
                    EndRound("PLAYER BLACKJACK");
                }
                else if (_game.DetermineBlackJack() == "dealer")
                {
                    EndRound("DEALER BLACKJACK");
                }
                else if (_game.DetermineBlackJack() == "draw")
                {
                    //in this blackjack implementation, if both the dealer and player get blackjack, the round ends in a draw
                    EndRound("DRAW");
                }
            }
        }

        /// <summary>
        /// Method used to save the game - called after changes have been made to the user menu and after each round
        /// </summary>
        private void SaveGame()
        {
            //saves the game
            _game.Save(_serializer);
        }

        /// <summary>
        /// Method used to load the game - called only once (when the user loads the blackjack page for the first time)
        /// </summary>
        private async void LoadGame()
        {
            //boolean variable used to determine if there should be a delay or not
            bool wait = false;

            //if there is a data file
            if (File.Exists($"{_serializer.DirectoryPath}\\{_serializer.FilePath}"))
            {
                //load the game from the data file
                _game.Load(_serializer);

                _uiFinalTextBorder.Visibility = Visibility.Visible;

                //if the player has a name saved
                if (_game.Player.Name != "")
                {
                    //address them by their name
                    _txtDisplayOutcome.Text = $"WELCOME BACK, {_game.Player.Name.ToUpper()}";
                }
                //if the user does not have a name saved
                else
                {
                    //address them and welcome them back, but since there is no name that is all that can be done
                    _txtDisplayOutcome.Text = $"WELCOME BACK";
                }

                //update controls to display loaded information
                _txtNameInput.Text = _game.Player.Name;

                _uiEditProfilePane.IsPaneOpen = false;

                _txtChipCount.Text = $"CHIP TOTAL: {_game.Player.GameMoney.ToString()}";
                _txtTotalMoney.Text = $"${_game.Player.Money.ToString()}";

                //the program will need to have a delay before the next message to the user
                wait = true;

            }
            //if there is no data file
            else
            {
                //delay
                await Task.Delay(TimeSpan.FromMilliseconds(10));

                //automatically open the pane for the user (they have to get money and buy chips since this is their first time playing
                _uiEditProfilePane.IsPaneOpen = true;

                //edit functionality of certain controls
                _btnAddFunds.IsEnabled = true;
                _btnAddGameMoney.IsEnabled = false;
            }

            //update the labels (whether the game has been loaded or not) to show the money (cash) and game money (chips) of the user
            _txtGameMoney.Text = _game.Player.GameMoney.ToString();
            _txtTotalMoney.Text = $"${_game.Player.Money.ToString()}";

            //if the user does not have a name
            if (_game.Player.Name == "")
            {
                //show placeholder text for an example name
                _txtNameInput.PlaceholderText = "Ex: User123";
            }

            //if the player does not have any money
            if (_game.Player.Money == 0)
            {
                //do not let them add any chips
                _btnAddGameMoney.IsEnabled = false;
            }

            //if the player does not have any chips
            if (_game.Player.GameMoney == 0)
            {
                //update the chip stack to make it look empty
                _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                _uiPlayerChips.Fill = new SolidColorBrush();
                _txtGameMoney.Text = "";
            }

            //if a file was loaded from
            if (wait == true)
            {
                //wait five seconds before the next message is shown
                await Task.Delay(TimeSpan.FromMilliseconds(5000));

                //if the player has no chips
                if (_game.Player.GameMoney == 0)
                {
                    //inform them to buy chips from the user menu
                    _txtDisplayOutcome.Text = "CLICK 'USER MENU' TO BUY CHIPS";
                }
                
                //if the player has chips
                else
                {
                    //inform them to place a bet
                    _txtDisplayOutcome.Text = "CLICK ON A CHIP TO MAKE A BET";
                }

            }

            //if a file was not loaded from
            else
            {
                //ask them to buy chips - if no file was loaded from, a default player is made, meaning they have no chips
                _txtDisplayOutcome.Text = "CLICK 'USER MENU' TO BUY CHIPS";
            }

        }

        #endregion

        #region Event Handlers

        #region Game Event Handlers

        /// <summary>
        /// Event Handler that runs when the user clicks on their chip stack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerChipClick(object sender, PointerRoutedEventArgs e)
        {
            //if the player has chips to bet
            if (_game.Player.GameMoney != 0)
            {
                //update controls
                _txtDisplayOutcome.Text = "";
                _uiFinalTextBorder.Visibility = Visibility.Collapsed;

                //add 10 to their bet, subtract 10 from their game money, show bet on the screen
                _game.Player.Bet += 10;
                _txtBet.Text = _game.Player.Bet.ToString();
                _game.Player.GameMoney -= 10;

                //update text labels to show updated amounts
                _txtGameMoney.Text = _game.Player.GameMoney.ToString();
                _txtChipCount.Text = $"CHIP TOTAL: {_game.Player.GameMoney.ToString()}";

                //make the bet area look like a chip stack
                _uiBetArea.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                _uiBetArea.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

                _btnStart.IsEnabled = true;

                //if the player has no money (runs after they have made a bet)
                if (_game.Player.GameMoney == 0)
                {
                    //make the chip stack look empty
                    _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    _uiPlayerChips.Fill = new SolidColorBrush();
                    _txtGameMoney.Text = "";

                    //if the player has no cash (on top of having no chips)
                    if (_game.Player.Money == 0)
                    {
                        //do not let the add chips (they must get more cash first
                        _btnAddGameMoney.IsEnabled = false;
                    }
                    
                    //do not let the player place bets
                    _uiPlayerChip.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Event Handler that runs when the user clicks on the bet area chips
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBetAreaClick(object sender, PointerRoutedEventArgs e)
        {
            //if the player has made a bet but they havn't started the game
            if (_game.Player.Bet != 0 && _started == false)
            {
                //subtract 10 from the bet total, then update the label
                _game.Player.Bet -= 10;
                _txtBet.Text = _game.Player.Bet.ToString();
                
                //if the player has no chips left
                if (_game.Player.GameMoney == 0)
                {
                    //make the chip stack look like a chip stack - it currently looks empty 
                    _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    _uiPlayerChips.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    _uiPlayerChip.IsEnabled = true;
                }

                // add 10 to the chip total of the player
                _game.Player.GameMoney += 10;

                //update the controls
                _txtGameMoney.Text = _game.Player.GameMoney.ToString();
                _txtChipCount.Text = $"CHIP TOTAL: {_game.Player.GameMoney.ToString()}";

                //if the player bet is 0 
                if (_game.Player.Bet == 0)
                {
                    //make the bet area look empty
                    _txtBet.Text = "";

                    _uiBetArea.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    _uiBetArea.Fill = new SolidColorBrush();

                    _txtDisplayOutcome.Text = "CLICK ON A CHIP TO MAKE A BET";
                    _uiFinalTextBorder.Visibility = Visibility.Visible;

                    _btnStart.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Event Handler that runs when the user clicks on the start button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            //start the round
            StartRound();
        }

        /// <summary>
        /// Event Handler that runs when the user clicks on the hit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHitClick(object sender, RoutedEventArgs e)
        {
            //if the player is able to hit
            if (_game.Hit() == true)
            {
                //don't let them double - following blackjack rules
                _btnDouble.IsEnabled = false;

                //redraw the player's hand (now with an additonal card than before)
                DrawCardsBE(_canvasPlayerHand, _game.Player.Hand, Alignment.Center);

                //if the player has a soft-hand value
                if (_game.Player.SoftHandValue != 0)
                {
                    //update the label to show the player's score and soft-hand score
                    _txtPlayerTotal.Text = $"{_game.Player.Score.ToString()}/{_game.Player.SoftHandValue.ToString()}";
                }

                //if the player does not have a soft-hand value
                else
                {
                    //don't show the soft-hand score, only show the score
                    _txtPlayerTotal.Text = _game.Player.Score.ToString();
                }

                //if the score of the player is 21
                if (_game.Player.Score == 21)
                {
                    //disable the buttons
                    _btnHold.IsEnabled = false;
                    _btnHit.IsEnabled = false;

                    //start the dealer card timer to add dealer cards - this is done automatically as the user has hit 21 (the best score possible)
                    _tmDealerCardTimer.Start();
                }

                //if the player score is greater than 21
                if (_game.Player.Score > 21)
                {
                    //end the round, pasisng the method the appropriate message
                    EndRound("PLAYER BUST, DEALER WINS");
                }


            }
        }

        /// <summary>
        /// Event Handler that runs when the user clicks on the hold button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHoldClick(object sender, RoutedEventArgs e)
        {
            //start the dealer timer - deals the cards
            _tmDealerCardTimer.Start();

            //disable the buttons
            _btnHold.IsEnabled = false;
            _btnHit.IsEnabled = false;
            _btnDouble.IsEnabled = false;

            //if the soft-hand value of the dealer is greater than the score (if the soft-hand score is not 0
            if (_game.Player.SoftHandValue > _game.Player.Score)
            {
                //set the score of the dealer to the soft-hand value
                _txtPlayerTotal.Text = _game.Player.SoftHandValue.ToString();
            }

        }

        /// <summary>
        /// Event Handler that runs on each tick of the dealer timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDealerCardTimerTick(object sender, object e)
        {
            //if this is the first time the timer has ticked
            if (_firstTimerTick == true)
            {
                //turn the cards face up and re-draw them
                _game.Dealer.TurnCardsFaceUp();
                DrawCardsBE(_canvasDealerHand, _game.Dealer.Hand, Alignment.Center);

                //show the dealer's score
                _uiDealerScoreBorder.Visibility = Visibility.Visible;

                //if the dealer has a soft-hand value
                if (_game.Dealer.SoftHandValue != 0)
                {
                    //show the score and soft-hand value
                    _txtDealerTotal.Text = $"{_game.Dealer.Score.ToString()}/{_game.Dealer.SoftHandValue.ToString()}";
                }

                //if the dealer does not have a soft-hand value
                else
                {
                    //show the score of the player
                    _txtDealerTotal.Text = _game.Dealer.Score.ToString();
                }

                //ensure the above code only runs the first tick by setting the boolean to false
                _firstTimerTick = false;


            }

            //if this is not the first time the timer has ticked
            else
            {
                //if the dealer's soft-hand value is 17 or above
                if (_game.Dealer.SoftHandValue >= 17)
                {
                    //set the dealer score to the soft-hand value and update the label
                    _game.Dealer.Score = _game.Dealer.SoftHandValue;
                    _txtDealerTotal.Text = _game.Dealer.Score.ToString();
                }

                //if the dealer's score is between 17 and 21
                if (_game.Dealer.Score >= 17 && _game.Dealer.Score <= 21)
                {
                    //end the round - blackjack rules - dealer cannot hit after it reaches a score (or soft-hand score) of 17 or above
                    EndRound(_game.DetermineWinner());
                }

                //if the dealer's score is less than 17
                else
                {
                    if (_game.Hold() == true)
                    {
                        //if the dealer has a soft-hand value
                        if (_game.Dealer.SoftHandValue != 0)
                        {
                            //show the score and soft-hand core of the dealer
                            _txtDealerTotal.Text = $"{_game.Dealer.Score.ToString()}/{_game.Dealer.SoftHandValue.ToString()}";
                        }

                        //if the dealer does not have a soft-hand score
                        else
                        {
                            //show the score
                            _txtDealerTotal.Text = _game.Dealer.Score.ToString();
                        }
                        
                        //draw the cards again
                        DrawCardsBE(_canvasDealerHand, _game.Dealer.Hand, Alignment.Center);

                        //if the dealer score is greater than 21
                        if (_game.Dealer.Score > 21)
                        {
                            //end the round - dealer busted
                            EndRound("DEALER BUST, PLAYER WINS");
                        }

                    }

                    //if the dealer has reached a score of above 17
                    else
                    {
                        //stop the card timer
                        _tmDealerCardTimer.Stop();

                        //if their score is less than or equal to 17
                        if (_game.Dealer.Score <= 21)
                        {
                            //end the round
                            EndRound(_game.DetermineWinner());
                        }
                    }
                }
            }



        }

        /// <summary>
        /// Event Handler that runs when the user clicks on the double button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDoubleClick(object sender, RoutedEventArgs e)
        {
            //if the player has the funs to double
            if (_game.Double() == true)
            {
                //update the bet
                _txtBet.Text = _game.Player.Bet.ToString();

                //if the player has chips remaining
                if (_game.Player.GameMoney != 0)
                {
                    //update the chip count
                    _txtGameMoney.Text = _game.Player.GameMoney.ToString();
                }

                //if the player does not have money left
                else
                {
                    //make the chipstack look empty
                    _txtGameMoney.Text = "";
                    _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    _uiPlayerChips.Fill = new SolidColorBrush();
                    _uiPlayerChip.IsEnabled = false;

                }

                //update the chip total
                _txtChipCount.Text = $"CHIP TOTAL: {_game.Player.GameMoney.ToString()}";

                //disable the game buttons
                _btnHold.IsEnabled = false;
                _btnHit.IsEnabled = false;
                _btnDouble.IsEnabled = false;

                //draw the cards again
                DrawCardsBE(_canvasPlayerHand, _game.Player.Hand, Alignment.Center);

                //if the soft-hand value of the player has a value
                if (_game.Player.SoftHandValue != 0)
                {
                    //show the plaer;s score and soft-hand score
                    _txtPlayerTotal.Text = $"{_game.Player.Score.ToString()}/{_game.Player.SoftHandValue.ToString()}";
                    _game.Player.Score = _game.Player.SoftHandValue;
                }

                //if there is no soft-hand value
                else
                {
                    //show the player's score
                    _txtPlayerTotal.Text = _game.Player.Score.ToString();
                }

                //delay
                await Task.Delay(TimeSpan.FromMilliseconds(1500));

                //if the player's score is above 21
                if (_game.Player.Score > 21)
                {
                    //end the round - the player busted
                    EndRound("PLAYER BUST, DEALER WINS");
                }

                //if the player's score is not above 21
                else
                {
                    //start dealing the dealer cards
                    _tmDealerCardTimer.Start();
                }

                //update the socre label
                _txtPlayerTotal.Text = _game.Player.Score.ToString();
            }
        }

        #endregion

        #region Pane Event Handlers

        /// <summary>
        /// Event Handler that runs when the user clicks on the user menu button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditUserClick(object sender, RoutedEventArgs e)
        {
            //set the splitview pane properties
            _uiEditProfilePane.IsPaneOpen = true;
            _uiEditProfilePane.DisplayMode = SplitViewDisplayMode.Overlay;
        }

        /// <summary>
        /// Event Handler that runs when the user clicks to add funs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddFundsClick(object sender, RoutedEventArgs e)
        {
            //add 10 to the player's cash total
            _game.Player.Money += 10;

            //update the cash text display
            _txtTotalMoney.Text = $"${(_game.Player.Money).ToString()}";

            //if the button to add chips is currently disabled
            if (_btnAddGameMoney.IsEnabled == false)
            {
                //enable it
                _btnAddGameMoney.IsEnabled = true;
            }
        }

        /// <summary>
        /// Event Handler that runs when the user clicks to add chips
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddChips(object sender, RoutedEventArgs e)
        {
            //if the current display text is asking the user to buy chips
            if (_txtDisplayOutcome.Text == "CLICK 'USER MENU' TO BUY CHIPS")
            {
                //change the text to ask the user to click on the chips to make a bet
                _txtDisplayOutcome.Text = "CLICK ON A CHIP TO MAKE A BET";
            }

            //if the chip count of the player is 0
            if (_game.Player.GameMoney == 0)
            {
                //set the beat area to look like a chip stack
                _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                _uiPlayerChips.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

                _uiPlayerChip.IsEnabled = true;


            }

            //add 10 to the chip stack
            _game.Player.GameMoney += 10;

            //subtract 10 from the player's cash total
            _game.Player.Money -= 10;

            //update the cash total label
            _txtTotalMoney.Text = $"${(_game.Player.Money).ToString()}";

            //update the chip count label (the one in the pane)
            _txtChipCount.Text = $"CHIP TOTAL: {_game.Player.GameMoney.ToString()}";

            //update the chip count label (the one that appears over the chips when playing the game)
            _txtGameMoney.Text = _game.Player.GameMoney.ToString();

            //if the player has no money left
            if (_game.Player.Money == 0)
            {
                //don't let them add any chips
                _btnAddGameMoney.IsEnabled = false;

            }

        }

        /// <summary>
        /// If the user clicks on the done button in the pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDoneClick(object sender, RoutedEventArgs e)
        {
            //close the pane
            _uiEditProfilePane.IsPaneOpen = false;
        }

        /// <summary>
        /// If the user changes their name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNameChanged(object sender, TextChangedEventArgs e)
        {
            //set the name of the player to the text in the input box
            _game.Player.Name = _txtNameInput.Text;
        }

        /// <summary>
        /// When the user menu pane closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPaneClosed(SplitView sender, object args)
        {
            //save the game
            SaveGame();
        }


        #endregion

        #endregion

    }
}
