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
using System.Windows.Threading;
namespace HCIGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Ellipse> removeThis = new List<Ellipse>();

        int spawnRate = 60;
        int currentRate;
        int lastScore = 0;
        int x;
        int y;
        int health = 350;
        int score = 0;
        double growthRate = 0.5;
        Random rand = new Random();
        Brush brush;

        MediaPlayer playClickSound = new MediaPlayer();
        MediaPlayer playPopSound = new MediaPlayer();

        Uri ClickedSound;
        Uri PoppedSound;
        public MainWindow(Window parent)
        {
            Owner = parent;
            InitializeComponent();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            currentRate = spawnRate;

            ClickedSound = new Uri("pack://siteoforigin:,,,/sound/clickedpop.mp3");
            PoppedSound = new Uri("pack://siteoforigin:,,,/sound/pop.mp3");
        }

        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;
            txtLastScore.Content = "Last score: " + lastScore;
            currentRate -= 2;
            if (currentRate < 1) {
                currentRate = spawnRate;
                x = rand.Next(15, 700);
                y = rand.Next(50, 350);
                brush = new SolidColorBrush(Color.FromRgb((byte)rand.Next(1, 255), (byte)rand.Next(1, 255), (byte)rand.Next(1, 255)));

                Ellipse circle = new Ellipse
                {
                    Tag = "circle",
                    Height = 10,
                    Width = 10,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = brush
                };
                Canvas.SetLeft(circle, x);
                Canvas.SetTop(circle, y);

                MyCanvas.Children.Add(circle);
            }

            foreach (var x in MyCanvas.Children.OfType<Ellipse>()) {
                x.Height += growthRate;
                x.Width += growthRate;
                x.RenderTransformOrigin = new Point(0.5, 0.5);

                if (x.Width > 70) {
                    removeThis.Add(x);
                    health -= 15;
                    playPopSound.Open(PoppedSound);
                    playPopSound.Play();

                }
            }

            if (health > 1)
            {
                healthBar.Width = health;
            }
            else {
                GameOver();
            }
            foreach (Ellipse i in removeThis)
            {
                MyCanvas.Children.Remove(i);
            }

            if (score > 5){
                spawnRate = 25;
            }
            if (score > 20)
            {
                spawnRate = 15;
                growthRate = 1.5;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Ellipse) {
                Ellipse circle = (Ellipse)e.OriginalSource;
                MyCanvas.Children.Remove(circle);
                score++;
                playClickSound.Open(ClickedSound);
                playClickSound.Play();
            }
        }
    

        private void GameOver() {
            gameTimer.Stop();
            MessageBoxResult result = MessageBox.Show("Game over! " + Environment.NewLine + "Your score: " + score +
                Environment.NewLine + "Click OK to play again! ", "Agreement", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
