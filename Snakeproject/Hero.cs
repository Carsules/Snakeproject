using System;
using System.Collections.Generic;

namespace Snakeproject
{
    public class Hero
    {
        public readonly static Hero left = new Hero(0, -1);
        public readonly static Hero right = new Hero(0, 1);
        public readonly static Hero up = new Hero(-1, 0);
        public readonly static Hero down = new Hero(1, 0);
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

        public override bool Equals(object obj)
        {
            return obj is Hero hero &&
                   RowOffset == hero.RowOffset &&
                   ColOffset == hero.ColOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        public static bool operator ==(Hero left, Hero right)
        {
            return EqualityComparer<Hero>.Default.Equals(left, right);
        }

        public static bool operator !=(Hero left, Hero right)
        {
            return !(left == right);
        }
    }

}
