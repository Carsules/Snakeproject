﻿using System;
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
                state.move();
                brain();
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
                    if((bot.headpos().Row+1 ==foodRow) || (bot.headpos().Row -1 == foodRow) || (bot.headpos().Col + 1 == foodCol) || (bot.headpos().Col -1 == foodCol))
                    {
                        currentDirection = GetDirection(bot.headpos().Row, bot.headpos().Col, foodRow, foodCol);
            }
                // выполняем шаг в выбранном направлении
                switch (currentDirection)
                {
                    case 1://up
                        bot.sidemove(Hero.up);
                        break;
                    case 2://down
                        bot.sidemove(Hero.down);
                        break;
                    case 3://left
                        bot.sidemove(Hero.left);
                        break;
                    case 4://right
                        bot.sidemove(Hero.right);
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
                    return bot.headpos().Row > 0 && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == 0;
                case 2://down
                    return bot.headpos().Row < bot.Grid.GetLength(0) - 1 && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col] == 0;
                case 3://left
                    return bot.headpos().Col > 0 && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == 0;
                case 4://right
                    return bot.headpos().Col < bot.Grid.GetLength(1) - 1 && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == 0;
                default:
                    return false;
            }
        }
        // функция для выбора случайного направления движения из текущей позиции головы змейки
        public int GetRandomDirection()
        {
            int[] rnddir = new int[] { 0, 0, 0, 0 };
            if (bot.headpos().Row > 0 && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == 0)
            {
                rnddir[0] = 1; //up
            }

            if (bot.headpos().Row < bot.Grid.GetLength(0) - 1 && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col] == 0)
            {
                rnddir[1] = 2;//down
            }

            if (bot.headpos().Col > 0 && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == 0)
            {
                rnddir[2] = 3;//left
            }

            if (bot.headpos().Col < bot.Grid.GetLength(1) - 1 && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == 0)
            {
                rnddir[3] = 4;//right
            }
            Random rnd = new Random();
            int a;
            while (true)
            {
                a = rnd.Next(3);
                if(rnddir[a] != 0)
                {
                    return rnddir[a];
                }
            }
        }

        }
    }
