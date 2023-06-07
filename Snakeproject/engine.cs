using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Snakeproject
{
    public class engine //Основной класс отвечающий за процесс игры
    {
        public enum items //Тип перечисления в котором содержатся все объекты игры
        {
            empty = 0, // 0 - это пустая клетка
            snake = 1, // 1 - это клетка змеи
            food = 2, // 2 - это клетка еды
            edge = 3 // 0 - это клетка границ
        }
        public int Rows { get; } //Переменная отвечающая за кол-во строк
        public int Cols { get; } //Переменная отвечающая за кол-во столбцов

        public items[,] Grid { get; } //Массив, который является полем игры
        public movement Snk { get; set; } //Переменная отвечающая за напрвления змейки
        public int Score { get; set; } //Переменная отвечающая за кол-во очков
        public bool Gameover { get; set; } //Переменная отвечающая за проигрыш
        public LinkedList<movement> dirchng = new LinkedList<movement>(); //Лист содержаший направления движений
        private readonly LinkedList<Field> snkpos = new LinkedList<Field>(); //Лист содержаший координаты сегменты змейки
        private readonly Random rnd = new Random(); //Переменная отвечающая за случайность
        public engine(int rows, int cols) //Конструктор, который создаёт поле, змейку, движение, еду
        {
            Rows = rows; //Определяет кол-во строк
            Cols = cols;//Определяет кол-во столбцов
            Grid = new items[rows, cols]; //Создаёт игравое поле
            Snk = movement.right; //задаёт змейке движение вправо
            create(Rows);
            dinner();
        }
        public movement Opposite(movement type) //Метод, который возвращает противоположно движению
        {
            if (type == movement.up) //Если движение вверх, вернёт движение вниз
            {
                return movement.down;
            }
            else if (type == movement.down)
            {
                return movement.up;
            }
            else if (type == movement.left)
            {
                return movement.right;
            }
            else return movement.left;
        }
        private void create(int rows) //Метод создающий на поле границу и змейку 
        {
            for(int r=0; r<rows; r++) //Цикл создаюший границы
            {
                Grid[r, 0] = items.edge; //Создаёт границу в первой строке
                Grid[r, rows-1] = items.edge; //Создаёт границу в последней строке
                Grid[0, r] = items.edge; //Создаёт границу в первом столбце
                Grid[rows-1, r] = items.edge; //Создаёт границу в последнем столбце
            }
            int ro = Rows / 2; //Переменная обозначающая среднюю строку
            for(int i = 2; i < 5; i++) //Цикл, который создаёт змейку
            {
                Grid[ro, i] = items.snake; //Создаёт клетку змейку змейки на поле
                snkpos.AddFirst(new Field(ro, i)); //Заносит положение сигмента змейки в лист
            }

        }
        private IEnumerable<Field> EmptyPositions() //Метод, который возвращает пустые клетки на поле 
        {
            for(int r = 1; r< Rows-1; r++) //Проходит по полю
            {
                for (int c=1; c < Cols-1; c++)
                {
                    if (Grid[r,c] == items.empty) //Если элемент массива пустой,
                    {
                        yield return new Field(r, c); //,то он возвращает пустую клетку
                    }
                }
            }
        }
        private void dinner() //Метод создающий еду на случайной пустой клетке в поле
        {
            List<Field> empty = new List<Field>(EmptyPositions()); //Лист содержащий все пустые клетки поля
            if (empty.Count == 0) //Если пустых клеток нет, то метод ничего не делает
            {
                return;
            }
            Field pos = empty[rnd.Next(empty.Count)]; //Переменная, которая случайно выбирает пустую клетку для еды
            Grid[pos.Row, pos.Col] = items.food; //Создаёт в этой клетке еду
        }
        public Field headpos() //Метод возвращающий позицию головы змейки
        {
            return snkpos.First.Value;
        }
        public Field tailpos() //Метод возвращающий позицию хвоста змейки
        {
            return snkpos.Last.Value;
        }
        public IEnumerable<Field> snkposi() //Метод возвращающий позицию змейки
        {
            return snkpos;
        }
        private void addhead(Field pos) //Метод, который добавляет сегмент змейки относительно головы в заданом направлении
        {
            snkpos.AddFirst(pos); //Добавляем новую позицию головы в лист с позицией змейки
            Grid[pos.Row, pos.Col] = items.snake; //Выводит новую позицию головы на поле
        }
        private void remtail() //Метод, который удаляет последний сегмент змейки
        {
            Field tail = snkpos.Last.Value; //Получаем позицию хвоста змейки
            Grid[tail.Row, tail.Col] = items.empty; //Удаляем хвост из поля
            snkpos.RemoveLast(); //Удаляем хвост из листа позиции змейки
        }

        public movement getlastdir() //Метод возвращающий последнее направление змейки
        {
            if (dirchng.Count == 0) //Если в листе нет направлений, то возвращаем текущее направление
            {
                return Snk; 
            }
            return dirchng.Last.Value; //Возвращаем последнее направление в листе
        }

        public bool CanChangeDirection(movement newdir) //Метод определяющий возможность изменения направлений
        {
            if(dirchng.Count == 2) //Если в листе два направления, то возвращаем false
            {
                return false;
            }

            movement lastdir = getlastdir(); //Получаем значение последнего направления в листе направлений
            return newdir != lastdir && newdir != Opposite(lastdir); //Проверяем чтобы текущее направление не было противоположно предыдущему
        }
        public void sidemove(movement dir) //Метод меняющий направление змейки
        {
            if (CanChangeDirection(dir)) //Если направление змейки может поменяться, то змейка меняет направление
            {
                dirchng.AddLast(dir); //Добавляем в лист новое направление змеки 
            }
        }
        public bool edgerunner(Field pos) //Метод, проверяющий столкновение со стеной
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }
        public items Strike(Field headp) //Метод определяющий столкновения
        {
            if (edgerunner(headp)) //Если позиция головы находится на краю, то метод возвращает границу
            {
                return items.edge;
            }
            return Grid[headp.Row, headp.Col]; //Иначе возвращает змейку
        }
        public void move() //Метод отвечающий за события происходящие во время движения
        {
            if(dirchng.Count > 0) //Если в листе с движениями присутствуют движения
            {
                Snk = dirchng.First.Value; //То получаем из листа последнее движение  
                dirchng.RemoveFirst(); //Убераем первое движение из листа 
            }
            Field headp = headpos().Trans(Snk); //Получаем текущее движение
            items crash = Strike(headp); //Получаем текущее столкновение
            if(crash == items.edge || crash == items.snake) //Если змека сталкивается с границей или с собою, то игра заканчивается
            {
                Gameover = true;
            }
            else if (crash == items.empty) //Если змейка сталкивается с пустой клеткой
            { 
                remtail(); //Удаляем хвост
                addhead(headp); //Добавляем сегмент
            }
            else if (crash == items.food) //Если змейка сталкивается с едой
            {
                addhead(headp); //Добавляем сегмент
                Score++; //Добавляем очки
                dinner(); //Создаём новую еду
            }
        }
    }
}
