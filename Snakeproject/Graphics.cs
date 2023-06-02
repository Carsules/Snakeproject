using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snakeproject
{
    public static class Graphics
    {
        public static ImageSource empty = LoadImage("empty.png");
        public static ImageSource body = LoadImage("body.png");
        public static ImageSource head = LoadImage("head.png");
        public static ImageSource food = LoadImage("food.png");
        public static ImageSource LoadImage(string fileName)
        {
            return new BitmapImage(new Uri($"visual/{fileName}", UriKind.Relative));
        }
    }
}
