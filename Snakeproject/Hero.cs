using System;
using System.Collections.Generic;

namespace Snakeproject
{
    public class Hero
    {
        public static readonly Hero left = new Hero(0, -1);
        public static readonly Hero right = new Hero(0, -1);
        public static readonly Hero up = new Hero(-1, 0);
        public static readonly Hero down = new Hero(1, 0);
        public int RowOffset { get; }
        public int ColOffset { get; }
        private Hero(int rowOffset, int colOffset)
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
