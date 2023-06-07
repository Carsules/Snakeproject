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
    public partial class MainWindow : Window //Класс, который выводит змейку на экран 
    {
        public Dictionary<items, ImageSource> gridtoimg = new() //Словарь привязывающий игровой объект к изображению
        {
            {items.empty, Graphics.empty },
            {items.snake, Graphics.body },
            {items.food, Graphics.food },
            {items.edge, Graphics.edge }
        };
        public engine state; //Переменная игрового процесса игрока
        public engine bot; //Переменная игрового процесса бота
        private readonly int rows = 17, cols = 17; //Переменные размера поля
        private readonly Image[,] gridimg; //переменная изображения объектов игрока
        private readonly Image[,] botimg; //переменная изображения объектов игрока
        public bool gamerun; //Переменная отвечающая за запуск игры
        public MainWindow() //Конструктор запускающий программу
        {
            InitializeComponent(); //
            gridimg = SetupGrid(); //Накладывает изображения на поле игрока
            botimg = SetupGridb(); //Накладывает изображения на поле бота
            state = new engine(rows, cols); //Создаёт поле игрока
            bot = new engine(rows, cols); //Создаёт поле бота
        }
        public void Restart() //Метод перезапускающий игровой процесс
        {
            state = new engine(rows, cols); //Пересоздаёт поле игрока
            bot = new engine(rows, cols); //Пересоздаёт поле бота
        }
        public async Task RunGame() //Метод отвечающий за запуск игры
        {
            Draw();
            Overlay.Visibility = Visibility.Hidden; //Прячем стартовый слой
            await Loop(); //Запуск игрового процесса
        }
        public async void Window_PreviewKeyDown(Object sender, KeyEventArgs e) //Метод скрывающий стартовое окно при нажатии любой кнопки на клавиатуре
        {
            if (Overlay.Visibility == Visibility.Visible) // Если стартовое окно видно, то программа приостановится
            {
                e.Handled = true; 
            }
            if (!gamerun) //Если игра не запущена 
            {
                gamerun = true; //Игра запускается 
                await RunGame(); 
                gamerun = false; //Игра останавливается
            }
        }
        public void Window_KeyDown(Object sender, KeyEventArgs e) //Метод привязывающий кнопки к игровому процессу
        {
            if(e.Key == Key.R) //Если нажата R, то игра рестартится
            {
                Restart();
            }
            if (state.Gameover && bot.Gameover) //При проигрыше нельзя нажимать кнопки
            {
                return;
            }
            switch (e.Key)
            {
                case Key.A:
                    state.sidemove(movement.left); //При нажатии А змейка меняет направление влево
                    break;
                case Key.D:
                    state.sidemove(movement.right); //При нажатии D змейка меняет направление вправо
                    break;
                case Key.W:
                    state.sidemove(movement.up); //При нажатии W змейка меняет направление вверх
                    break;
                case Key.S:
                    state.sidemove(movement.down); //При нажатии S змейка меняет направление вниз
                    break;
            }
        }
        private async Task Loop() //Метод обновляющий поле, меняющий обновления кадров, определяющий смерть игроков и внедряющий алгаритм бота в игровой процесс
        {
            int time = 150; //Переменная отвечающая за задержку обновления кадров
            while (!state.Gameover || !bot.Gameover) //Пока игрок и бот не умерли
            {
                
                if(state.Score * 2 + bot.Score * 2 < 145) //Если количество очков игрока и бота умноженные на 2 меньше 145
                { 
                    await Task.Delay(time - state.Score * 2 - bot.Score*2); //То задержка равняется заданной задержке минус количество очков игрока и бота умноженные на 2
                }
                else await Task.Delay(5); //Иначе задержка равна 5
                if (!state.Gameover) //Если игрок проигрывает, то игра останавливается
                {
                    state.move();
                }
                if (!bot.Gameover) //Если бот проигрывает, то игра останавливается
                {
                    bot.move();
                }
                brain(); //Алгаритм змейки бота
                Draw(); // Прорисовка полей
            }
        }
        private Image[,] SetupGrid() //Метод прорисовывающий поле игрока
        {
            Image[,] images = new Image[rows, cols]; //Массив изображений поля
            GameGrid.Rows = rows; //Задаём значение количества строк поле игрока
            GameGrid.Columns = cols; //Задаём значение количества столбцов поле игрока
            for (int r = 0; r < rows; r++) //Проходимся по полю и заполняем изображниями
            {
                for(int c = 0; c < cols; c++)
                {
                    Image image = new Image //Создаём переменную изображения
                    {
                        Source = Graphics.empty //Задаём изначальным источником изоброжение пустой клетки
                    };
                    images[r, c] = image; //Переносим изображения в масссив
                    GameGrid.Children.Add(image); //Добавляем изображения на поле
                }
            }
            return images; //Возвращаем массив изображений
        }
        private Image[,] SetupGridb() //Метод прорисовывающий поле бота
        {
            Image[,] images = new Image[rows, cols]; //Массив изображений поля
            botGrid.Rows = rows; //Задаём значение количества строк поле бота
            botGrid.Columns = cols; //Задаём значение количества столбцов поле бота
            for (int r = 0; r < rows; r++) //Проходимся по полю и заполняем изображниями
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image //Создаём переменную изображения
                    {
                        Source = Graphics.empty //Задаём изначальным источником изоброжение пустой клетки
                    };
                    images[r, c] = image; //Переносим изображения в масссив
                    botGrid.Children.Add(image); //Добавляем изображения на поле
                }
            }
            return images; //Возвращаем массив изображений
        }
        public void Draw() //Метод отвечающий за отображение поля и очков
        {
            drwgrd(); 
            drwgrdb(); 
            ScoreT.Text = $"Очки {state.Score}"; //Отображает очки игрока
            ScoreB.Text = $"Очки {bot.Score}"; //Отображает очки бота
        }

        public void drwgrd() //Метод прорисовывающий объект на поле игрока
        {
            for(int r =0; r < rows; r++) //Проходимся по полю
            {
                for (int c=0; c < cols; c++)
                {
                    items gridval = state.Grid[r, c]; //Считываем объект с поля
                    gridimg[r, c].Source = gridtoimg[gridval]; //Вставляем изображение данного объекта
                }
            }
        }
        public void drwgrdb() //Метод прорисовывающий объект на поле бота
        {
            for (int r = 0; r < rows; r++) //Проходимся по полю
            {
                for (int c = 0; c < cols; c++)
                {
                    items gridval = bot.Grid[r, c]; //Считываем объект с поля
                    botimg[r, c].Source = gridtoimg[gridval]; //Вставляем изображение данного объекта
                }
            }
        }
        public void brain()
        {

                // получаем координаты еды на карте
                int foodRow = -1;
                int foodCol = -1;

                for (int r = 0; r < rows; r++) //Проходимся по массиву
                {
                    for (int c = 0; c < cols; c++)
                    {
                        if (bot.Grid[r, c] == items.food) //Если в массиме находится еда, то записываем координаты
                        {
                            foodRow = r;
                            foodCol = c;
                            break;
                        }
                    }

                    if (foodRow != -1 && foodCol != -1) //Если еды нет, то прерываем поиск
                    {
                        break;
                    }
                }
                int currentDirection; //Переменная отвечающая за текущее направления движения
                int fooddir = GetDirection(bot.headpos().Row, bot.headpos().Col, foodRow,foodCol); //Переменная отвечающая за направление движение с сторону еды
                // выбираем новое направление движения
                    if (CanMoveInDirection(fooddir)) //Если направление в сторону еды возможно
                    {
                        currentDirection = fooddir; //Текущее направление = направление к еде
                    }
                    else
                    {
                        currentDirection = GetRandomDirection(); //Иначе текущее направление = другое доступное направление
            }
                    if((bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] ==items.snake && bot.Grid[bot.headpos().Row +1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row + 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake)) //Если слева головы змейки возникает ее тело
                    {
                        for(int c=bot.headpos().Col; c<cols-1; c++) //Проходимся по столбцам от головы змейки до конца поля
                        {
                            if (bot.Grid[bot.headpos().Row, c+1] != items.snake) //Если след клетка не равняется змейке
                            {
                                if(bot.Grid[bot.headpos().Row+1, c] == items.snake) //Если снизу клетки есть змейка
                                {
                                    if (CanMoveInDirection(1)) //Если движение вверх допустимо
                                        {
                                            currentDirection = 1; //Текущее направления равняется вверх
                                        }
                                }
                                else if(bot.Grid[bot.headpos().Row - 1, c] == items.snake) //Иначе если сверху клетки есть змейка
                                {
                                    if (CanMoveInDirection(2)) //Если движение вниз допустимо
                                    {
                                        currentDirection = 2; //Текущее направления равняется вниз
                            }
                                }
                            }
                        }
                    }
                    else if ((bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake)
                        || (bot.Grid[bot.headpos().Row, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col + 1] == items.snake) && bot.Grid[bot.headpos().Row, bot.headpos().Col - 1] == items.snake) //Если справо головы змейки возникает ее тело
            {
                        for (int c = bot.headpos().Col; c > 1; c--) //Проходимся по столбцам от головы змейки до начала поля
                {
                            if (bot.Grid[bot.headpos().Row, c - 1] != items.snake) //Если след клетка не равняется змейке
                    {
                                if (bot.Grid[bot.headpos().Row + 1, c] == items.snake) //Если снизу клетки есть змейка
                        {
                                    if (CanMoveInDirection(1)) //Если движение вверх допустимо
                            {
                                        currentDirection = 1; //Текущее направления равняется вверх
                            }
                                }
                                else if (bot.Grid[bot.headpos().Row - 1, c] == items.snake) //Иначе если сверху клетки есть змейка
                        {
                                    if (CanMoveInDirection(2)) //Если движение вниз допустимо
                            {
                                        currentDirection = 2; //Текущее направления равняется вниз
                            }
                                }
                            }
                        }
                    }
                    else if((bot.Grid[bot.headpos().Row+1, bot.headpos().Col] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake)
                        || (bot.Grid[bot.headpos().Row+1, bot.headpos().Col] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake)
                        || (bot.Grid[bot.headpos().Row + 1, bot.headpos().Col + 1] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake)
                        || (bot.Grid[bot.headpos().Row+1, bot.headpos().Col] == items.snake && bot.Grid[bot.headpos().Row + 1, bot.headpos().Col - 1] == items.snake && bot.Grid[bot.headpos().Row - 1, bot.headpos().Col] == items.snake)) //Если снизу головы змейки возникает ее тело
            {
                        for (int r = bot.headpos().Row; r > 1; r--) //Проходимся по строкам от головы змейки до начала поля
                {
                            if (bot.Grid[r+1, bot.headpos().Col] != items.snake) //Если след клетка не равняется змейке
                    {
                                if (bot.Grid[r, bot.headpos().Col+1] == items.snake) //Если снизу клетки есть змейка
                        {
                                    if (CanMoveInDirection(3)) //Если движение вверх допустимо
                            {
                                        currentDirection = 3; //Текущее направления равняется вверх
                            }
                                }
                                else if (bot.Grid[r, bot.headpos().Col -1] == items.snake) //Иначе если сверху клетки есть змейка
                        {
                                    if (CanMoveInDirection(4)) //Если движение вниз допустимо
                            {
                                        currentDirection = 4; //Текущее направления равняется вниз
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
