using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace Fandom
{
    internal class DataProcesing //класс для работы с бд
    {
        static private string dbname = "Fandom.db"; //имя бд
        static private string path = $"Data Source={dbname};"; //подключение к бд 
        static private string show_Persons_data = "SELECT * FROM Person;";
        static private string dbName = "Fandom";
        static private string tblPersonName = "Persons";
        static private string tblInfoName = "Info";
        static private string tblImageName = "Images";



        public static void AddCharacter(string name, string description, string link, byte[] avatar)
        {
            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                string query = "INSERT INTO Person (name, description, link, avatar) VALUES (@name, @description, @link, @avatar);";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@link", link);
                command.Parameters.AddWithValue("@avatar", avatar);

                conn.Open();
                //command.ExecuteNonQuery();
            }
        }

        //получение данныз из бд
        public static DataSet GetDBData()
        {
            DataSet result = new DataSet();
            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(show_Persons_data, conn);
                SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(adapter);
                adapter.Fill(result);
            }
            return result;
        }

        // удаление персонажа
        public static void DeleteCharacter(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                string query = "DELETE FROM Person WHERE id=@id";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                command.Parameters.AddWithValue("@id", id);

                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        //экспорт данных
        public static string ExportData()
        {
            DataSet result = GetDBData();
            StringWriter writer = new StringWriter();

            foreach (DataTable table in result.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    writer.WriteLine($"ID: {row["id"]}, Name: {row["name"]}, Description: {row["description"]}, Link: {row["link"]}");
                }
            }

            return writer.ToString();
        }

      

    }




}
