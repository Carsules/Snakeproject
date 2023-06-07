using System;
using System.Collections.Generic;

namespace Snakeproject
{
    public class movement // Класс, дающий напрвление змейке
    {
        public readonly static movement left = new movement(0, -1); //Переменная, задающее направление влево 
        public readonly static movement right = new movement(0, 1); //Переменная, задающее направление вправо 
        public readonly static movement up = new movement(-1, 0); //Переменная, задающее направление вверх
        public readonly static movement down = new movement(1, 0); //Переменная, задающее направление вниз
        public int RowOffset { get; } //Переменная, обозначающая значение строки
        public int ColOffset { get; } //Переменная, обозначающая значение столбца
        private movement(int rowOffset, int colOffset) //Метод, который берёт значение строки и столбца
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }
    }

}
