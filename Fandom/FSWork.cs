using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fandom
{
    internal class FSWork //класс для работы с файлами и загрузкой изображений
    {
        //Получение путей к разным системным директориям
        static public string Path(string location = "myDocs") 
        {
            switch (location) //выбор типа папки (доки, раб.стол, каталог)
            {
                case "myDocs":
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                case "Desktop":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                case "Current":
                    return Environment.CurrentDirectory;
                default:
                    return string.Empty; //если не определен, то пустая строка
                    break;
            }
             
        }

        //читаем SQL-файл разделяя на команды
        static public List<string> ReadSQLFile(string filename, string start = "CREATE TABLE") 
        {
            List<string> result = new List<string>(); //создаем список
            using (StreamReader sr = new StreamReader(filename)) //открываем для чтения
            {
                string tmp =  sr.ReadToEnd(); //читаем
                result =  tmp.Split(';').ToList<string>(); //разделяем содержимое на команды по ";"
            }
            for (int i = 0; i < result.Count; i++) //вставляем ";"
            {
                result[i] += ";";
            }
            return result; //результат с командами
        }

        //проверка существования файла
        static public bool IsFileExist(string path)
        {
            bool result = false; //хранение
            if (File.Exists(path)) //если файл существует
            {
                result = true;
            }
            return result; //результат проверки
        }

        //получение изображений в виде массива байтов
        static public byte[] GetImage()
        {
            byte[] result = null; //инициализация переменной для результата
            string filename = string.Empty; //хранение имени файла
            OpenFileDialog ofd = new OpenFileDialog(); //диалоговое окно для выбора файла
            ofd.Filter = "JPG files(*.JPG)|*.jpg|All files(*.*)|*.*"; //фильтр для отображения только JPT-файлов
            if (ofd.ShowDialog() == DialogResult.OK) //если пользователь выбрал файл
            {
                filename = ofd.FileName; //получаем имя выбранного файла
            }
            else return result; //если не выбран, то результат в котором нал

            using (FileStream fs = new FileStream(filename, FileMode.Open)) //открытие выбранного файла для чтения
            {
                result = new byte[fs.Length]; //инициализация массива байтов размером с файл
                fs.Read(result, 0, result.Length); //чтение содержимого файла в массив
            }
            return result; //массив байтов
        }
    }
}
