using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snakeproject
{
    public static class Graphics //Класс, отвечающий за загрузку картинок
    {
        public static ImageSource empty = LoadImage("empty.png"); //Переменная содержащая изображение пустой клетки
        public static ImageSource body = LoadImage("body.png"); //Переменная содержащая изображение тела змейки
        public static ImageSource food = LoadImage("food.png"); //Переменная содержащая изображение еды
        public static ImageSource edge = LoadImage("edge.png"); //Переменная содержащая изображение границ
        public static ImageSource LoadImage(string fileName) //Метод, который возвращает изображения через проописаный путь
        {
            return new BitmapImage(new Uri($"visual/{fileName}", UriKind.Relative));
        }
    }
}
