using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Snakeproject
{
    public class engine
    {
        public enum items
        {
            empty = 0,
            snake = 1,
            food = 2,
            edge = 3
        }
    public int Rows { get; }
       public int Cols { get; }
       
       public items[,] Grid { get; }
        public movement Snk { get; set; }
        public int Score { get; set; }
        public bool Gameover { get; set; }

        public LinkedList<movement> dirchng = new LinkedList<movement>();
        private readonly LinkedList<Field> snkpos = new LinkedList<Field>();
        private readonly Random rnd = new Random();
        public engine(int rows, int cols) {
            Rows = rows;
            Cols = cols;
            Grid = new items[rows, cols];
            Snk = movement.right;
            create(Rows);
            dinner();
                }
        public movement Opposite(movement type)
        {
            if (type == movement.up)
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
        private void create(int rows)
        {
            for(int r=0; r<rows; r++)
            {
                Grid[r, 0] = items.edge;
                Grid[r, rows-1] = items.edge;
                Grid[0, r] = items.edge;
                Grid[rows-1, r] = items.edge;
            }
            int ro = Rows / 2;
            for(int i = 2; i < 5; i++)
            {
                Grid[ro, i] = items.snake;
                snkpos.AddFirst(new Field(ro, i));

            }

        }
        private IEnumerable<Field> EmptyPositions()
        {
            for(int r = 1; r< Rows-1; r++)
            {
                for (int c=1; c < Cols-1; c++)
                {
                    if (Grid[r,c] == items.empty)
                    {
                        yield return new Field(r, c); 
                    }
                }
            }
        }
        private void dinner()
        {
            List<Field> empty = new List<Field>(EmptyPositions());
            if (empty.Count == 0)
            {
                return;
            }
            Field pos = empty[rnd.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = items.food;
        }
        public Field headpos()
        {
            return snkpos.First.Value;
        }
        public Field tailpos()
        {
            return snkpos.Last.Value;
        }
        public IEnumerable<Field> snkposi()
        {
            return snkpos;
        }
        private void addhead(Field pos)
        {
            snkpos.AddFirst(pos);
            Grid[pos.Row, pos.Col] = items.snake;
        }
        private void remtail()
        {
            Field tail = snkpos.Last.Value;
            Grid[tail.Row, tail.Col] = items.empty;
            snkpos.RemoveLast();
        }

        public movement getlastdir()
        {
            if (dirchng.Count == 0)
            {
                return Snk;
            }
            return dirchng.Last.Value;
        }

        public bool CanChangeDirection(movement newdir)
        {
            if(dirchng.Count == 2)
            {
                return false;
            }

            movement lastdir = getlastdir();
            return newdir != lastdir && newdir != Opposite(lastdir);
        }
        public void sidemove(movement dir)
        {
            if (CanChangeDirection(dir))
            {
                dirchng.AddLast(dir);
            }
        }
        public bool edgerunner(Field pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }
        public items Strike(Field headp)
        {
            if (edgerunner(headp))
            {
                return items.edge;
            }
            if (headp == tailpos())
            {
                return items.empty;
            }
            return Grid[headp.Row, headp.Col];
        }
        public void move()
        {
            if(dirchng.Count > 0)
            {
                Snk = dirchng.First.Value;
                dirchng.RemoveFirst();
            }
            Field headp = headpos().Trans(Snk);
            items crash = Strike(headp);
            if(crash == items.edge || crash == items.snake)
            {
                Gameover = true;
            }
            else if (crash == items.empty){
                remtail();
                addhead(headp);
            }
            else if (crash == items.food)
            {
                addhead(headp);
                Score++;
                dinner();
            }
        }
    }
}
