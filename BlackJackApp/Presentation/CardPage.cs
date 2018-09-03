using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using BlackJackApp.DataTypes;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;

namespace BlackJackApp.Presentation
{

    

   public enum Alignment
    {
        Horizontal,
        Vertical,
        Left,
        Right,
        Center

       
    }

    public enum Action
    {
        Take,
        Give,
        None
    }

    public abstract class CardPage:Page
    {
       
        public abstract Grid MainGrid { get; }

        public CardPage()
        {
           SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawScreen();
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawScreen();
        }

        protected abstract void DrawScreen();
        
        protected void DrawCardsBE(Canvas canvas, List<Card> cards, Alignment alignment)
        {
            canvas.Children.Clear();
            double center = canvas.ActualWidth / 2;
            //sets the width of the cards (can fit 10 cards in this case)
            double width = MainGrid.RenderSize.Width / 12;

            //looping through all of the list of cards that have been passed as a parameter
            for (int imageIndex = 0; imageIndex < cards.Count(); imageIndex++)
            {

                //creating an image
                Image image = new Image();
                //setting the width (see above)
                image.Width = width;
                //setting theheight to be the height of the canvas that was passed as a parameter
                image.Height = canvas.ActualHeight;
                
                //if the current card is facing up           
                if (cards[imageIndex].FaceUp)
                    //sets the source of the image with the front face
                    image.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{cards[imageIndex].GetFileName()}"));
                //if the curren tcard is not facing up
                else
                    //sets the source of the image with the back face
                    image.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/cardBack_blue1.png"));

                //sets the offset for where the cards can be drawn
                double offset = imageIndex * width;
                //adds the image to the canvas
                canvas.Children.Add(image);
                //ensures the card is drawn at the top of the canvas
                Canvas.SetTop(image, 0);

                //if the alignment specified in the parameter is 'center'
                if (alignment == Alignment.Center)
                {

                    offset = Math.Ceiling(Convert.ToDouble(imageIndex) / 2) * width;
                    if (imageIndex % 2 == 1)
                        offset *= -1;
                    Canvas.SetLeft(image, center + offset);

                }
                
                else if (alignment == Alignment.Left)
                {
                    //draw the card from left to right (add the offset)
                    Canvas.SetLeft(image, offset);
                }
                else if (alignment == Alignment.Right)
                {
                    //draw the card from right to left (subtract the offset and width of the card
                    Canvas.SetLeft(image, canvas.ActualWidth - offset - width);
                }

            }
        }

    }
}
