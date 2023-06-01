using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakeproject
{
    class Field
    {
        byte[,] zone = new byte[25, 25];

        enum stateofsquare
        {
            empty=0,
            snake=1,
            food=2,
            edge=3
        };

        void Edgezone()
        {
            for(byte i = 0; i < 25; i++)
            {
                zone[i,0]=(byte)stateofsquare.edge;
                zone[i, 24] = (byte)stateofsquare.edge;
                zone[0, i] = (byte)stateofsquare.edge;
                zone[24, i] = (byte)stateofsquare.edge;
            }
        }
        void shootfood()
        {
            bool same = true;
            while (same)
            {
                Random x = new Random();
                Random y = new Random();
                if (zone[x.Next(25), y.Next(25)] != (byte)stateofsquare.snake)
                {
                    same = false;
                }
            }

        }
    }
}
