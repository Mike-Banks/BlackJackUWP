using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BlackJackApp.Presentation;

//CLASS/PAGE AUTHOR: Michael Banks
namespace BlackJackApp
{
    
    /// <summary>
    /// Class used with the Main Page
    /// </summary>
    public sealed partial class MainPage : Page
    {

        /// <summary>
        /// Constructor that loads the page and navigates to the instructions page by default
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            _frmContent.Navigate(typeof(SplashPage));
        }

        /// <summary>
        /// Event Handler that runs when the user navigates to a new page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContentFrameNavigated(object sender, NavigationEventArgs e)
        {
            //if the page the user is navigating to is the instructions page
            if (e.SourcePageType == typeof(InstructionsPage))
            {
                //change the title of the page / change the selected item in the nav menu
                _txtPageTitle.Text = "Information";
                _lstAppNavigation.SelectedItem = _uiNavInstructions;
                _navSplitView.DisplayMode = SplitViewDisplayMode.Inline;
            }

            //if the page the user is navigating to is the blackjack page
            else if (e.SourcePageType == typeof(BlackJack))
            {
                //change the title of the page / change the selected item in the nav menu
                _txtPageTitle.Text = "Blackjack";
                _lstAppNavigation.SelectedItem = _uiNavBlackJack;
                _navSplitView.DisplayMode = SplitViewDisplayMode.Inline;
                _navSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// Event Handler that runs when a nav menu item is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNavigationItemClicked(object sender, ItemClickEventArgs e)
        {
            //sets variable equal to the clicked control
            NavMenuItem navMenuItem = e.ClickedItem as NavMenuItem;

            //if the control clicked is the instructions page button
            if (navMenuItem == _uiNavInstructions)
            {
                //navigate to the instructions page
                _frmContent.Navigate(typeof(InstructionsPage));
            }

            //if the control clicked is the blackjack page button
            else if (navMenuItem == _uiNavBlackJack)
            {
                //navigate to the blackjack page
                _frmContent.Navigate(typeof(BlackJack));
            }

        }
    }
}