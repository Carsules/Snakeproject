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
        public engine bot;
        private readonly int rows = 25, cols = 25;
        private readonly Image[,] gridimg;
        private readonly Image[,] botimg;
        public bool gamerun;
        public MainWindow()
        {
            InitializeComponent();
            gridimg = SetupGrid();
            botimg = SetupGridb();
            state = new engine(rows, cols);
            bot = new engine(rows, cols);
        }
        public void Restart()
        {
            state = new engine(rows, cols);
            bot = new engine(rows, cols);
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
            int time = 150;
            while (!state.Gameover)
            {
                
                if(state.Score * 2 - bot.Score * 2 < 145) {
                    await Task.Delay(time - state.Score * 2 - bot.Score*2);
                }
                else await Task.Delay(5);
                Botbtain();
                state.move();
                bot.move();
                Draw();
            }
        }
        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            for (int r = 0; r < rows; r++)
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
        private Image[,] SetupGridb()
        {
            Image[,] images = new Image[rows, cols];
            botGrid.Rows = rows;
            botGrid.Columns = cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Graphics.empty
                    };
                    images[r, c] = image;
                    botGrid.Children.Add(image);
                }
            }
            return images;
        }
        public void Draw()
        {
            drwgrd();
            drwgrdb();
            ScoreT.Text = $"Очки {state.Score}";
            ScoreB.Text = $"Очки {bot.Score}";
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
        public void drwgrdb()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    items gridval = bot.Grid[r, c];
                    botimg[r, c].Source = gridtoimg[gridval];
                }
            }
        }
        public void Botbtain()
        {
            for (int r = 1; r < rows-1; r++)
            {
                for (int c = 1; c < cols-1; c++)
                {
                    if(items.food == bot.Grid[r, c])
                    {
                        if(bot.headpos().Row < r)
                        {
                            if (bot.Grid[bot.headpos().Row+1, bot.headpos().Col] ==items.snake)
                            {
                                for(int ch = 1; ch < cols - 1; ch++)
                                {
                                    if ((bot.Grid[bot.headpos().Row, ch] == items.snake))
                                    {
                                        if(ch < bot.headpos().Col)
                                        {
                                            bot.sidemove(Hero.down);
                                        }
                                        else bot.sidemove(Hero.up);

                                    }
                                }
                            }
                            else bot.sidemove(Hero.right);
                        }
                        else
                        {
                            if (bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake)
                            {
                                for (int ch = 1; ch < cols - 1; ch++)
                                {
                                    if ((bot.Grid[bot.headpos().Row, ch] == items.snake))
                                    {
                                        if (ch < bot.headpos().Col)
                                        {
                                            bot.sidemove(Hero.down);
                                        }
                                        else bot.sidemove(Hero.up);

                                    }
                                }
                            }
                            else bot.sidemove(Hero.left);
                        }
                        if (bot.headpos().Col < c)
                        {
                            if (bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake)
                            {
                                for (int ch = 1; ch < rows - 1; ch++)
                                {
                                    if ((bot.Grid[ch, bot.headpos().Col] == items.snake))
                                    {
                                        if (ch < bot.headpos().Row)
                                        {
                                            bot.sidemove(Hero.right);
                                        }
                                        else bot.sidemove(Hero.left);
                                    }
                                }
                            }
                            else bot.sidemove(Hero.down);
                        }
                        else
                        {
                            if (bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake)
                            {
                                for (int ch = 1; ch < cols - 1; ch++)
                                {
                                    if ((bot.Grid[ch, bot.headpos().Col] == items.snake))
                                    {
                                        if (ch < bot.headpos().Row)
                                        {
                                            bot.sidemove(Hero.left);
                                        }
                                        else bot.sidemove(Hero.right);

                                    }
                                }
                            }
                            else bot.sidemove(Hero.up);
                        }
                    }
                }
            }
        }
    }
}
