using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Media;
using System.IO;
namespace rocketRiotv2
{
    public class Player
    {
        double xPosition, yPosition, xSpeed, ySpeed;
        int coins;
        bool falling = true;
        Canvas canvas = new Canvas();
        Rectangle playerSprite = new Rectangle ();
        Polygon hitBox = new Polygon();
        ImageBrush spritefill;//Image for the player
        BitmapImage bitmapImage;//Image file to use
        Polygon test;
        int counter = 0;
        /// <summary>
        /// Create a player object
        /// </summary>
        /// <param name="xP"></param>
        /// <param name="yP"></param>
        /// <param name="xS"></param>
        /// <param name="yS"></param>
        public Player(double xP, double yP, double xS, double yS, Canvas c)
        {
            xPosition = xP;
            yPosition = yP;
            xSpeed = xS;
            ySpeed = yS;
            canvas = c;
            playerSprite.Height = 75;
            playerSprite.Width = 75;

            //bitmapImage = new BitmapImage(new Uri("spriteFill2.png", UriKind.Relative));
            //spritefill = new ImageBrush(bitmapImage);

            bitmapImage = new BitmapImage(new Uri("spriteFill2.png", UriKind.Relative));
            spritefill = new ImageBrush(bitmapImage);

            playerSprite.Fill = spritefill;
            canvas.Children.Add(playerSprite);
            Canvas.SetBottom(playerSprite, yPosition);
            Canvas.SetLeft(playerSprite, xPosition);

            //hitBox.Stroke = Brushes.Red;
            hitBox.StrokeThickness = 2;
            hitBox.FillRule = FillRule.EvenOdd;
            hitBox.Fill = Brushes.Transparent;
            StreamReader sr = new StreamReader("playerPoints.txt");
            List<Point> points = new List<Point>();
            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                double xPosition, yPosition;
                double.TryParse(currentLine.Split(',')[0], out xPosition);
                double.TryParse(currentLine.Split(',')[1], out yPosition);
                Point point = new Point(xPosition, yPosition);


                //solution to polygons, put polygon behind the player and make it transparent
                //move the polygon with the player


                points.Add(point);
            }
            PointCollection myPointCollection = new PointCollection();
            for (int i = 0; i < points.Count; i++)
            {
                myPointCollection.Add(points[i]);
            }
            hitBox.Points = myPointCollection;
            canvas.Children.Add(hitBox);
        }
        public void move()
        {       
            if (Keyboard.IsKeyDown(Key.Up))
            {
                if (falling)
                {
                    ySpeed = 0;
                    falling = false;
                }
                else
                {
                    ySpeed += 0.2;//change based on difficulty
                }
            }
            else
            {
                if (falling)
                {
                    ySpeed -= 0.4;
                }
                else
                {
                    ySpeed = 0;
                    falling = true;
                }
            }
            if (falling)
            {
                if (yPosition + ySpeed > 0 )
                {
                    yPosition += ySpeed;
                }
                else
                {
                    yPosition = 0;
                }
            }
            else
            {
                if (yPosition + ySpeed < 480)
                {
                    yPosition += ySpeed;
                }
                else
                {
                    yPosition = 480;
                }
            }
            if (xPosition + xSpeed < 800)
            {
                xPosition += xSpeed;
            }
            else
            {
                xPosition = 800;
            }
        }
        public void animate()
        {
            Canvas.SetBottom(playerSprite, yPosition);
            Canvas.SetLeft(playerSprite, xPosition);
            Canvas.SetBottom(hitBox, yPosition);
            Canvas.SetLeft(hitBox, xPosition);
        }
        public void update()
        {

        }
        public bool intersectWith(PointCollection hits)
        {
            bool hitTrue = false;
            for (int i = 0; i < hits.Count; i++)
            {
                if (canvas.InputHitTest(hits[i]) == hitBox)
                {
                    hitTrue = true;
                }
            }
            return hitTrue;
        }
        public bool pastScreen()
        {
            if (xPosition == 800)
            {
                xPosition = 0;
                xSpeed += 0.2;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void rotate()
        {
            counter++;
            RotateTransform rotate = new RotateTransform(counter * 5);
            playerSprite.RenderTransformOrigin = new Point (0.5, 0.5);
            playerSprite.RenderTransform = rotate;
            canvas.Children.Remove(playerSprite);
            canvas.Children.Add(playerSprite);
            hitBox.RenderTransformOrigin = new Point(0.5, 0.5);
            hitBox.RenderTransform = rotate;
            canvas.Children.Remove(hitBox);
            canvas.Children.Add(hitBox);
        }
        public bool collision(Point point)
        {
            return canvas.InputHitTest(point) == playerSprite;
        }
    }
}
