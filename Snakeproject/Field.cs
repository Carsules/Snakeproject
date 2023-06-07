using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakeproject 
{
    public class Field //Класс смещающий змейку в указаном направлении
    {
        public int Row { get; } //Переменная, обозначающая значение строки
        public int Col { get; } //Переменная, обозначающая значение столбца
        public Field(int row, int col) //Метод, который берёт значение строки и столбца
        {
            Row = row;
            Col = col;
        }
        public Field Trans(movement snk) //Метод, который смещает змейку в указаном направлении
        {
            return new Field(Row + snk.RowOffset, Col + snk.ColOffset); //Прибавляем к строке и столбцу направление движения змейки
        }
    }
}
