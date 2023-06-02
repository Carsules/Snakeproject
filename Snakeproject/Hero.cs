using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakeproject
{
    public class Hero
    {
        public static Hero left = new Hero(0, -1);
        public static Hero right = new Hero(0, -1);
        public static Hero up = new Hero(-1, 0);
        public static Hero down = new Hero(1, 0);
        public int RowOffset { get; }
        public int ColOffset { get; }
        Hero(int rowOffset, int colOffset)
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }
        public Hero Opposite()
        {
            return new Hero(-RowOffset, -ColOffset);
        }
    }
}
