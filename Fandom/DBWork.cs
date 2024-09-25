using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fandom
{
    internal class DBWork //класс для рбаоты с базой данных
    {
        static private string dbname = "Fandom.db"; //имя бд
        static private string path = $"Data Source={dbname};"; //подключение к бд
        static private List<string> queryes = new List<string>(); //хранение SQL-запросов
        static private List<SQLiteCommand> commands = new List<SQLiteCommand>(); //хранение команд SQLite

        //заполнение списка запросов из SQL файла
        static public void FillQueryes(string filename = @"sql\CreateDB.sql")
        {
            queryes = FSWork.ReadSQLFile(filename); //чтение файла и сохранение запросов в список
        }

        //выполнение всех запросов
        static public bool MakeQuery()
        {
            bool result = false; //для хранения результата выполнения запросов
            using (SQLiteConnection conn = new SQLiteConnection(path)) //создание соединения с бд
            {
                conn.Open(); //открытие
                for (int i = 0; i < queryes.Count; i++) //проход по списку SQL запросов
                {
                    commands.Add(conn.CreateCommand()); //добавление команды для текущего запроса
                    commands[i].CommandText = queryes[i]; //установка текста команды
                    //commands[i].ExecuteNonQuery(); //выполнение команды
                }
            }
            result = true; //успех выполнения
            return result; //возвращение результата
        }

        //получение списка механиков из бд
        static public List<string> GetMechanics() 
        {
            List<string> result = new List<string>(); //список для хранения механиков
            string get_mechanisc = "SELECT name From Mechanics;"; //SQL запрос для получения имен механиков

            using (SQLiteConnection conn = new SQLiteConnection(path)) //соединение с бд
            {
                SQLiteCommand cmd = conn.CreateCommand(); //создание команды для выполнения SQL-запроса
                cmd.CommandText = get_mechanisc; //установка текста команды
                conn.Open(); //открытие соединения
                var reader = cmd.ExecuteReader(); //выполнение запроса и чтение результата
                if (reader.HasRows) //если результат содержит строки
                {
                    while (reader.Read()) //чтение каждой строки
                    {
                        result.Add(reader.GetString(0)); //добавление имени в список
                    }
                }
            }
            return result; //возврат списка
        }

        //обновление записи в таблице
        static public void AddAvatar(string _name, byte[] _image) 
        {
            using (SQLiteConnection conn = new SQLiteConnection(path)) //соединение с бд
            {
                SQLiteCommand command = new SQLiteCommand(conn); //создание команды для выполнения SQLзапроса
                command.CommandText = @"UPDATE Mechanics SET Avatar=@Avatar " +
                    $"WHERE Name LIKE '{_name}%';"; //запрос для обновления колонки аватар в таблице механиков, для механики с именем
                command.Parameters.Add(new SQLiteParameter("@Avatar", _image)); //передаем изображение в виде массива байтов в аватар
                conn.Open(); //открываем соединение
                command.ExecuteNonQuery(); //выполнение запроса (обновление данных в таблице)
            }
        }

        //возвращение аватара механиков
        static public MemoryStream GetAvatar(string _name)
        {
            MemoryStream result = null; //инициализация переменной для аватара в виде потока
            byte[] _image = null; //хранение изображения в виде массива байтов
            using (SQLiteConnection conn =
                new SQLiteConnection(path)) //открытие соединения с бд
            {
                SQLiteCommand cmd = new SQLiteCommand(conn); //команда для выполнения SQLзапроса
                string get_image = $"SELECT Avatar FROM Mechanics WHERE Name LIKE '{_name}%';"; //запрос для получения изображения
                cmd.CommandText = get_image; //установка текста команды
                conn.Open(); //открытие соединения
                SQLiteDataReader reader = cmd.ExecuteReader(); //выполнение запроса и получение результата в виде райдера
                if (reader.HasRows) //если результат содержит строки
                {
                    while (reader.Read()) //то чтение всех строк результата
                    {
                        if (!reader.IsDBNull(0)) //если колонка не содержит нал
                        {
                            _image =
                            (byte[])reader.GetValue(0); //получаем изображение в виде массива байтов
                        }

                    }
                }
            }
            if (_image != null) //если изображение было получено
            {
                result = new MemoryStream(_image); //преобразуем массив байтов в поток мемористрим
            }
            return result; //возврат потока с изображением
        }

        //SQL запрос дляв ставки новых данных в бд
        static public void AddData(string _newCategoryInsert,
            string _dbname = "test02") 
        {
            string path = $"Data Source={_dbname};"; //строка подключения к бд на основе имени файл в бд
            using (SQLiteConnection conn = new SQLiteConnection(path)) //открытие соединения
            {
                SQLiteCommand cmd = new SQLiteCommand(conn); //создание команды для выполнения SQLзапроса
                cmd.CommandText = _newCategoryInsert; //устанавливаем SQLзапроса, который передается в качестве аргумента
                conn.Open(); //открытие соединения
                cmd.ExecuteNonQuery(); //выполнение запроса (вставка данных)
            }
        }

        //обновление данных в таблице и получение их в нужном формате
        static public DataSet Refresh(string _dbname = "test02")
        {
            DataSet result = new DataSet(); //пустой dataset, кот будет содержать данныз из бд
            string path = $"Data Source={_dbname};"; //строка подключения к бд
            string show_all_data = "SELECT * FROM Category;"; //получения данных и таблицы
            using (SQLiteConnection conn = new SQLiteConnection(path)) //открытие соединения с бд
            {
                conn.Open(); 
                SQLiteDataAdapter adapter =
                    new SQLiteDataAdapter(show_all_data, conn); //адаптер для выполнения запрос и заполнение данных
                adapter.Fill(result); //Заполнение DataSet результатами запроса
            }
            return result;
        }


        static public void Save(DataTable dt,
            out string _query,
            string _dbname = "test02")
        {
            string path = $"Data Source={_dbname};";
            string show_all_data = "SELECT * FROM Category;";
            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                conn.Open();
                SQLiteDataAdapter adapter =
                    new SQLiteDataAdapter(show_all_data, conn);
                SQLiteCommandBuilder commandBuilder =
                    new SQLiteCommandBuilder(adapter);
                adapter.Update(dt);
                _query = commandBuilder.GetUpdateCommand().CommandText;
            }
        }
    }
}
