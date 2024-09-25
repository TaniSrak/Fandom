using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;


namespace Fandom
{
    public partial class Form1 : Form //класс для работы с формой
    {
        public Form1()
        {
            InitializeComponent();
            initDB();
        }
        private void initDB() //инициализация базы данных
        {
            DBWork.FillQueryes(); //заполнение sQL запросов для базы
            DBWork.MakeQuery(); //выполнение SQL запросов
           
            dgrTable.Refresh(); //обновляем
        }



        private void Form1_DoubleClick(object sender, EventArgs e)
        {
   
        }
        
        //обновить
        private void button1_Click(object sender, EventArgs e)
        {
          
            DataSet ds = DataProcesing.GetDBData();
            dgrTable.DataSource = ds.Tables[0];
            dgrTable.Refresh();
        }

        //добавить
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string description = txtDescription.Text;
            string link = txtLink.Text;
            byte[] avatar = null;

            if (!string.IsNullOrEmpty(txtImagePath.Text))
            {
                avatar = File.ReadAllBytes(txtImagePath.Text);
            }

            DataProcesing.AddCharacter(name, description, link, avatar);
            Refresh();
        }

        //удалить
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgrTable.SelectedRows.Count > 0)
            {
                int id = (int)dgrTable.SelectedRows[0].Cells["id"].Value;
                DataProcesing.DeleteCharacter(id);
                Refresh();
            }
        }

        //изображение
        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPG files(*.JPG)|*.jpg|All files(*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtImagePath.Text = ofd.FileName;
            }
        }

        private void dgrTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //Name
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //фотка
        private void txtImagePath_TextChanged(object sender, EventArgs e)
        {

        }

        //ссфлка
        private void txtLink_TextChanged(object sender, EventArgs e)
        {

        }

        //описание
        private void txtDescription_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string data = DataProcesing.ExportData();
            File.WriteAllText("export.txt", data);
            MessageBox.Show("Данные экспортированы в файл export.txt");
        }
    }
}
