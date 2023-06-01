using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakeproject
{
    class Hero
    {
        byte headposcol;
        byte headposrow;
        byte tailposcol;
        byte tailposrow;
        
        void sethead(byte headcol, byte headrow)
        {
            headposcol=headcol;
            headposrow=headrow;
        }
        void settail(byte tailcol, byte tailrow)
        {
            tailposcol=tailcol;
            tailposrow=tailrow;
        }
        void dirup()
        {
            headposcol--;
        }
        void dirdown()
        {
            headposcol++;
        }
        void dirright()
        {
            headposrow++;
        }
        void dirleft()
        {
            headposrow--;
        }

    }
}
