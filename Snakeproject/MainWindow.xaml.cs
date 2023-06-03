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

namespace Snakeproject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<items, ImageSource> gridtoimg = new()
        {
            {items.empty, Graphics.empty },
            {items.snake, Graphics.body },
            {items.food, Graphics.food }

        };
        public engine state;
        private readonly int rows = 25, cols = 25;
        private readonly Image[,] gridimg;
        public bool gamerun;
        public MainWindow()
        {
            InitializeComponent();
            gridimg = SetupGrid();
            state = new engine(rows, cols);
        }
        public void Restart()
        {
            state = new engine(rows, cols);
        }
        public async Task RunGame()
        {
            Draw();
            Overlay.Visibility = Visibility.Hidden;
            await Loop();
        }
        public async void Window_PreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if (!gamerun)
            {
                gamerun = true;
                await RunGame();
                gamerun = false;
            }
        }
        public void Window_KeyDown(Object sender, KeyEventArgs e)
        {
            if(e.Key == Key.R)
            {
                Restart();
            }
            if (state.Gameover)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    state.sidemove(Hero.left);
                    break;
                case Key.Right:
                    state.sidemove(Hero.right);
                    break;
                case Key.Up:
                    state.sidemove(Hero.up);
                    break;
                case Key.Down:
                    state.sidemove(Hero.down);
                    break;
            }
        }
        private async Task Loop()
        {
            int time = 100;
            while (!state.Gameover)
            {
                
                if(state.Score < 29) {
                    await Task.Delay(time - state.Score * 5);
                }
                else await Task.Delay(time);
                state.move();
                Draw();
            }
        }
        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Graphics.empty
                    };
                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }
        public void Draw()
        {
            drwgrd();
            ScoreT.Text = $"Очки {state.Score}";
        }

        public void drwgrd()
        {
            for(int r =0; r < rows; r++)
            {
                for (int c=0; c < cols; c++)
                {
                    items gridval = state.Grid[r, c];
                    gridimg[r, c].Source = gridtoimg[gridval];
                }
            }
        }
    }
}
