using System;
using System.Collections.Generic;

namespace Snakeproject
{
    public class movement
    {
        public readonly static movement left = new movement(0, -1);
        public readonly static movement right = new movement(0, 1);
        public readonly static movement up = new movement(-1, 0);
        public readonly static movement down = new movement(1, 0);
        public int RowOffset { get; }
        public int ColOffset { get; }
        private movement(int rowOffset, int colOffset)
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }
        public movement Opposite()
        {
            return new movement(-RowOffset, -ColOffset);
        }

        public override bool Equals(object obj)
        {
            return obj is movement movement &&
                   RowOffset == movement.RowOffset &&
                   ColOffset == movement.ColOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        public static bool operator ==(movement left, movement right)
        {
            return EqualityComparer<movement>.Default.Equals(left, right);
        }

        public static bool operator !=(movement left, movement right)
        {
            return !(left == right);
        }
    }

}
