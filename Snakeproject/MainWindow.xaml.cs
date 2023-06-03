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
using static Snakeproject.engine;

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
        public int rows = 10, cols = 10;
        public Image[,] gridimg;
        public bool gamerun;
        public MainWindow()
        {
            InitializeComponent();
            gridimg = SetupGrid();
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
        public async Task Loop()
        {
            while (!state.Gameover)
            {
                await Task.Delay(100);
                state.move();
                Draw();
            }
        }
        public Image[,] SetupGrid()
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
