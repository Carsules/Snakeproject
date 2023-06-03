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
            empty=0,
            snake=1,
            food=2,
            edge=3
        }
       public int Rows { get; }
       public int Cols { get; }
       
       public items[,] Grid { get; }
        public Hero snk { get; set; }
        public int Score { get; set; }
        public bool Gameover { get; set; }

        private LinkedList<Field> snkpos = new LinkedList<Field>();
        private Random rnd = new Random();
        public engine(int rows, int cols) {
            Rows = rows;
            Cols = cols;
            Grid = new items[rows, cols];
            snk = Hero.right;
            create();
            dinner();
                }
        private void create()
        {
            int r = Rows / 2;
            for(int i = 1; i < 4; i++)
            {
                Grid[r, i] = items.snake;
                snkpos.AddFirst(new Field(r, i));

            }
        }
        private IEnumerable<Field> EmptyPositions()
        {
            for(int r = 0; r< Rows; r++)
            {
                for (int c=0; c < Cols; c++)
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
        public void addhead(Field pos)
        {
            snkpos.AddFirst(pos);
            Grid[pos.Row, pos.Col] = items.snake;
        }
        public void remtail()
        {
            Field tail = snkpos.Last.Value;
            Grid[tail.Row, tail.Col] = items.empty;
            snkpos.RemoveLast();
        }
        public void sidemove(Hero dir)
        {
            snk = dir;
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
            Field headp = headpos().Trans(snk);
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
