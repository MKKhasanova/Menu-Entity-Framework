using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Menu
{
    public partial class Form3 : Form
    {
        Database1Entities db = new Database1Entities();
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int maxId = db.Oformlenies.Max(c => c.Id);

            Oformlenie newClient = new Oformlenie()
            {
                Id = maxId + 1,
                FIO = textBox1.Text,
                Address = textBox2.Text, 
                Phone = int.Parse(textBox3.Text)
            };

            // Получить экземпляр Form1 из контекста приложения
            Form2 form2 = Application.OpenForms.OfType<Form2>().FirstOrDefault();
            if (form2 != null)
            {
                // Добавить нового клиента в таблицу на Form1
                form2.AddClient(newClient);
            }

            // Закрыть текущую форму (Form3)
            Close();
        }
    }
}
