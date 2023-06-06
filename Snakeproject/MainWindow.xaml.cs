using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            {items.food, Graphics.food },
            {items.edge, Graphics.edge }
        };
        public engine state;
        public engine bot;
        private readonly int rows = 17, cols = 17;
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
            if (state.Gameover && bot.Gameover)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.A:
                    state.sidemove(movement.left);
                    break;
                case Key.D:
                    state.sidemove(movement.right);
                    break;
                case Key.W:
                    state.sidemove(movement.up);
                    break;
                case Key.S:
                    state.sidemove(movement.down);
                    break;
            }
        }
        private async Task Loop()
        {
            int time = 150;
            while (!state.Gameover || !bot.Gameover)
            {
                
                if(state.Score * 2 + bot.Score * 2 < 145) {
                    await Task.Delay(time - state.Score * 2 - bot.Score*2);
                }
                else await Task.Delay(5);
                if (!state.Gameover) 
                {
                    state.move();
                }
                if (!bot.Gameover)
                {
                    bot.move();
                }
                brain();
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
        public void brain()
        {

                // получаем координаты еды на карте
                int foodRow = -1;
                int foodCol = -1;

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        if (bot.Grid[r, c] == items.food)
                        {
                            foodRow = r;
                            foodCol = c;
                            break;
                        }
                    }

                    if (foodRow != -1 && foodCol != -1)
                    {
                        break;
                    }
                }
                int currentDirection;
                int fooddir = GetDirection(bot.headpos().Row, bot.headpos().Col, foodRow,foodCol);
                // выбираем новое направление движения
                    if (CanMoveInDirection(fooddir))
                    {
                        currentDirection = fooddir;
                    }
                    else
                    {
                        currentDirection = GetRandomDirection();
                    }
                    if((bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] ==items.snake && bot.Grid[bot.headpos().Row +1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row + 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake))
                    {
                        for(int c=bot.headpos().Col; c<cols-1; c++)
                        {
                            if (bot.Grid[bot.headpos().Row, c+1] != items.snake)
                            {
                                if(bot.Grid[bot.headpos().Row+1, c] == items.snake)
                                {
                                    if (CanMoveInDirection(1))
                                        {
                                            currentDirection = 1;
                                        }
                                }
                                else if(bot.Grid[bot.headpos().Row - 1, c] == items.snake)
                                {
                                    if (CanMoveInDirection(2))
                                    {
                                        currentDirection = 2;
                                    }
                                }
                            }
                        }
                    }
                    else if ((bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col + 1] == items.snake) && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake)
                    {
                        for (int c = bot.headpos().Col; c > 1; c--)
                        {
                            if (bot.Grid[bot.headpos().Row, c - 1] != items.snake)
                            {
                                if (bot.Grid[bot.headpos().Row + 1, c] == items.snake)
                                {
                                    if (CanMoveInDirection(1))
                                    {
                                        currentDirection = 1;
                                    }
                                }
                                else if (bot.Grid[bot.headpos().Row - 1, c] == items.snake)
                                {
                                    if (CanMoveInDirection(2))
                                    {
                                        currentDirection = 2;
                                    }
                                }
                            }
                        }
                    }
                    else if((bot.Grid[bot.headpos().Row+1, bot.headpos().Col] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake)
                        || (bot.Grid[bot.headpos().Row+1, bot.headpos().Col] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake)
                        || (bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake)
                        || (bot.Grid[bot.headpos().Row+1, bot.headpos().Col] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake))
                    {
                        for (int r = bot.headpos().Row; r > 1; r--)
                        {
                            if (bot.Grid[r+1, bot.headpos().Col] != items.snake)
                            {
                                if (bot.Grid[r, bot.headpos().Col+1] == items.snake)
                                {
                                    if (CanMoveInDirection(3))
                                    {
                                        currentDirection = 3;
                                    }
                                }
                                else if (bot.Grid[r, bot.headpos().Col -1] == items.snake)
                                {
                                    if (CanMoveInDirection(4))
                                    {
                                        currentDirection = 4;
                                    }
                                }
                            }
                        }
                    }
                    else if ((bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col] == items.snake)
                        || (bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col] == items.snake)
                        || (bot.Grid[bot.headpos().Row - 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col] == items.snake)
                        || (bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col] == items.snake))
                    {
                        for (int r = bot.headpos().Row; r < rows-1; r++)
                        {
                            if (bot.Grid[r - 1, bot.headpos().Col] != items.snake)
                            {
                                if (bot.Grid[r, bot.headpos().Col + 1] == items.snake)
                                {
                                    if (CanMoveInDirection(3))
                                    {
                                        currentDirection = 3;
                                    }
                                }
                                else if (bot.Grid[r, bot.headpos().Col - 1] == items.snake)
                                {
                                    if (CanMoveInDirection(4))
                                    {
                                        currentDirection = 4;
                                    }
                                }
                            }
                        }
                    }
            // выполняем шаг в выбранном направлении
            switch (currentDirection)
                {
                    case 1://up
                        bot.sidemove(movement.up);
                        break;
                    case 2://down
                        bot.sidemove(movement.down);
                        break;
                    case 3://left
                        bot.sidemove(movement.left);
                        break;
                    case 4://right
                        bot.sidemove(movement.right);
                        break;
                default:
                    return;
                }

                // ждем некоторое время перед следующим шагом
        }
            // функция для определения направления движения до указанной точки
            public int GetDirection(int fromRow, int fromCol, int toRow, int toCol)
            {
            int dir;
                if (fromRow > toRow)
                {
                    dir = 1;
                }
                else if (fromRow < toRow)
                {
                    dir = 2;
                }
                else if (fromCol > toCol)
                {
                    dir = 3;
                }
                else if (fromCol < toCol)
                {
                    dir = 4;
                }
                else
                {
                dir = 0;
                }
            return dir;
            }
        public bool CanMoveInDirection(int direction)
        {
            switch (direction)
            {
                case 1: //up
                    return ((bot.headpos().Row > 0 && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == 0) || (bot.headpos().Row > 0 && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.food));
                case 2://down
                    return ((bot.headpos().Row < bot.Grid.GetLength(0) - 1 && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col] == 0) || (bot.headpos().Row < bot.Grid.GetLength(0) - 1 && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col] == items.food));
                case 3://left
                    return ((bot.headpos().Col > 0 && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == 0) || (bot.headpos().Col > 0 && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.food)); 
                case 4://right
                    return ((bot.headpos().Col < bot.Grid.GetLength(1) - 1 && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == 0) || (bot.headpos().Col < bot.Grid.GetLength(1) - 1 && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.food));
                default:
                    return false;
            }
        }
        // функция для выбора случайного направления движения из текущей позиции головы змейки
        public int GetRandomDirection()
        {
            int[] rnddir = new int[] { 0, 0, 0, 0 };
            if ((bot.headpos().Row > 0 && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == 0) || (bot.headpos().Row > 0 && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.food))
            {
                return 1; //up
            }

            else if ((bot.headpos().Row < bot.Grid.GetLength(0) - 1 && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col] == 0) || (bot.headpos().Row < bot.Grid.GetLength(0) - 1 && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col] == items.food))
            {
                return 2;//down
            }

            else if ((bot.headpos().Col > 0 && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == 0) || (bot.headpos().Col > 0 && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.food))
            {
                return 3;//left
            }

            else
            {
                return 4;//right
            }
        }

        }
    }
