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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer gameTimer = new System.Windows.Threading.DispatcherTimer();
        Player player;
        Random random = new Random();
        Zapper zappers;
        Coin[] coins = new Coin[4];
        int lastCollected;
        int score = 0;
        int oldScore = 0;
        bool paused = false;
        bool setHighScore = false;
        string[] highScores = new string[5];
        string[] highScoreData;
        string name;
        Button btnStartGame = new Button();
        Button btnHighScores = new Button();
        Button btnBack = new Button();
        Button btnSubmitName = new Button();
        Label lblHighScores = new Label();
        TextBox txtName = new TextBox();
        public MainWindow()
        {
            InitializeComponent();


            //Conner's stuff

            btnStartGame.Click += btnStartGame_Click;
            btnStartGame.Content = "Start Game";
            btnStartGame.FontSize = 50;
            btnStartGame.Background = Brushes.Yellow;
            Canvas.SetLeft(btnStartGame, 265);
            Canvas.SetTop(btnStartGame, 50);
            btnStartGame.BorderThickness = new Thickness(1);
            btnStartGame.BorderBrush = Brushes.Black;
            canvas.Children.Add(btnStartGame);

            btnHighScores.Click += btnHighScores_Click;
            btnHighScores.Content = "High Scores";
            btnHighScores.FontSize = 40;
            btnHighScores.Background = Brushes.Yellow;
            Canvas.SetLeft(btnHighScores, 285);
            Canvas.SetTop(btnHighScores, 125);
            btnHighScores.BorderThickness = new Thickness(1);
            btnHighScores.BorderBrush = Brushes.Black;
            canvas.Children.Add(btnHighScores);

            lblHighScores.FontSize = 15;
            //lblHighScores.Background.Opacity = .5;
            //lblHighScores.Background = Brushes.White;
            lblHighScores.Background = new SolidColorBrush(Color.FromArgb(125, 255, 255, 255));
            lblHighScores.Foreground = Brushes.Black;
            Canvas.SetLeft(lblHighScores, 350 - lblHighScores.ActualWidth);
            Canvas.SetTop(lblHighScores, 120 - lblHighScores.ActualHeight);

            btnBack.Click += btnBack_Click;
            btnBack.Content = "Back";
            btnBack.FontSize = 40;
            btnBack.Background = Brushes.Yellow;
            Canvas.SetLeft(btnBack, 358);
            Canvas.SetTop(btnBack, 400);
            btnBack.BorderThickness = new Thickness(1);
            btnBack.BorderBrush = Brushes.Black;

            txtName.FontSize = 30;
            txtName.Background = Brushes.White;
            Canvas.SetLeft(txtName, 285);
            Canvas.SetTop(txtName, 125);
            txtName.MaxLength = 6;

            btnSubmitName.Click += btnSubmitName_Click;
            btnSubmitName.Content = "Submit";
            btnSubmitName.FontSize = 40;
            btnSubmitName.Background = Brushes.Yellow;
            Canvas.SetLeft(btnSubmitName, 358);
            Canvas.SetTop(btnSubmitName, 400);
            btnSubmitName.BorderThickness = new Thickness(1);
            btnSubmitName.BorderBrush = Brushes.Black;

            System.IO.StreamReader sr = new System.IO.StreamReader("HighScores.txt");
            for (int i = 0; i < 5; i++)
            {
                highScores[i] = sr.ReadLine();
            }
            sr.Close();





            player = new Player(0, 300, 6, 0, playerCanvas);
            zappers = new Zapper(canvas, random);
            zappers.generate();

            for (int i = 0; i < 4; i++)
            {
                coins[i] = new Coin(i, canvas, random);
                coins[i].generate();
            }

            SoundPlayer sp = new SoundPlayer("Rocket Man Soundtrack.wav");
            sp.PlayLooping();

            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
        }
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            player.move();
            player.animate();
            if (player.pastScreen())
            {
                zappers.generate();
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        coins[i].remove();
                    }
                    catch
                    {

                    }
                    coins[i] = new Coin(i, canvas, random);
                    coins[i].generate();
                    lastCollected = -1;
                }
            }
            if (player.intersectWith(zappers.locations()))
            {
                MessageBox.Show("You lose");
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        highScoreData = highScores[i].Split(new char[] { ',' });
                        Int32.TryParse(highScoreData[1], out oldScore);
                    }
                    catch
                    {
                        oldScore = 0;
                    }
                    if (score > oldScore)
                    {
                        setHighScore = true;
                    }
                }

                if (setHighScore == true)
                {
                    updateHighScore();
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (player.intersectWith(coins[i].locations()))
                {
                    coins[i].remove();
                    if (lastCollected != i)
                    {
                        score++;
                        txtScore.Text = "Score: " + score;
                        lastCollected = i;
                    }
                }
            }
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (paused)
            {
                gameTimer.Start();
                btnPause.Content = "||";
                paused = false;
            }
            else
            {
                gameTimer.Stop();
                btnPause.Content = ">";
                paused = true;
            }
        }
        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {

            gameTimer.Start();

            canvas.Children.Remove(btnStartGame);
            canvas.Children.Remove(btnHighScores);

        }
        private void btnHighScores_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(btnStartGame);
            canvas.Children.Remove(btnHighScores);

            canvas.Children.Add(btnBack);
            canvas.Children.Add(lblHighScores);

            lblHighScores.Content = "";
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    highScoreData = highScores[i].Split(',');
                    lblHighScores.Content += (i + 1) + ". " + highScoreData[0] + " " + highScoreData[1] + Environment.NewLine;
                }
                catch
                {
                    lblHighScores.Content += (i + 1) +". " + "No Score Found" + Environment.NewLine;
                }
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(btnBack);
            canvas.Children.Remove(lblHighScores);
            canvas.Children.Add(btnHighScores);
            canvas.Children.Add(btnStartGame);
        }

        public void updateHighScore()
        {
            zappers.remove();
            for (int i = 0; i < 4; i++)
            {
                coins[i].remove();
            }
            canvas.Children.Add(txtName);
            canvas.Children.Add(btnSubmitName);
        }

        private void btnSubmitName_Click(object sender, RoutedEventArgs e)
        {
            name = txtName.Text;
            System.IO.StreamWriter sw = new System.IO.StreamWriter("highScores.txt");
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    highScoreData = highScores[i].Split(new char[] { ',' });
                    
                }
                catch
                {
                    highScoreData[0] = "";
                    highScoreData[1] = "0";
                }
                Int32.TryParse(highScoreData[1], out oldScore);
                if (score > oldScore)
                {
                    highScores[i] = name + "," + score.ToString();
                    score = oldScore;
                    name = highScoreData[0];
                }

                sw.WriteLine(highScores[i]);
            }
            sw.Close();
        }
    }
}
