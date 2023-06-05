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
        public Field Trans(movement snk)
        {
            return new Field(Row + snk.RowOffset, Col + snk.ColOffset);
        }
    }
}
