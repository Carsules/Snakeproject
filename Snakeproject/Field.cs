using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakeproject
{
    public class Field
    {
        public int Row { get;}
        public int Col { get;}
        public Field(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public Field Trans(Hero snk)
        {
            return new Field(Row + snk.RowOffset, Col + snk.ColOffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Field field &&
                   Row == field.Row &&
                   Col == field.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Field left, Field right)
        {
            return EqualityComparer<Field>.Default.Equals(left, right);
        }

        public static bool operator !=(Field left, Field right)
        {
            return !(left == right);
        }
    }
}
