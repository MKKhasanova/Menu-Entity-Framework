using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Menu
{
    public partial class Form2 : Form
    {
        Database1Entities db = new Database1Entities();
        public Form2()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
            Hide();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
            var korzinas = from korzina in db.Kors
                           join book in db.Menus on korzina.Id equals book.Id
                           join genre in db.Kitchens on book.Kitchen_ID equals genre.Kitchen_ID
                           join food in db.FoodTypes on book.FoodType_ID equals food.FoodType_ID
                           select new
                           {
                              // Id = book.Id,
                               Kitchen = genre.Kitchen1,
                               Type = food.FoodType1,
                               Food_Name = book.Name,
                               Price = book.Price,
                               Kol = korzina.Kol
                           };
            dataGridView1.DataSource = korzinas.ToList();
            decimal totalCost = (decimal)korzinas.Sum(item => item.Price * item.Kol);
            textBox1.Text = totalCost.ToString();
            DataGridViewColumn ColumnKorzina = new DataGridViewTextBoxColumn();
            dataGridView1.Columns.Insert(0, ColumnKorzina);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = i + 1;
            }
            
            dataGridView3.DataSource = db.Historys.ToList<History>();
            dataGridView3.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView3.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataGridViewRow curRow = dataGridView2.CurrentRow;
            string Names = curRow.Cells["FIO"].Value.ToString();
            Oformlenie selectedReader = db.Oformlenies.Where(x => x.FIO == Names).FirstOrDefault();
            int IDs = selectedReader.Id;
            DateTime currentDate = DateTime.Today;
            string dateString = currentDate.ToString("dd-MM-yyyy");
            decimal Summ = Convert.ToDecimal(textBox1.Text);
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string Menu_Name = row.Cells["Food_Name"].Value.ToString();
                Kor selectedBook = db.Kors.Where(x => x.Name == Menu_Name).FirstOrDefault();
                int Book_ID = selectedBook.Id;
                decimal bonusPercentage = 0.1m;
                decimal bonusValue = Summ * bonusPercentage;
                int bonusIntValue = Convert.ToInt32(bonusValue);
                History newLoan = new History();

                newLoan.FIO = Names;
                newLoan.Data = dateString;
                newLoan.AllSum = Summ;
                newLoan.Bonus = bonusIntValue;

                // Генерация нового идентификатора для новой записи
                int newId = db.Historys.Max(x => x.Id) + 1;
                newLoan.Id = newId;

                db.Historys.Add(newLoan);
                db.SaveChanges();
            }
            MessageBox.Show($"Заказ успешно оформлен на пользователя {Names}");
            //очищение корзины
            List<Kor> korzinaToDelete = db.Kors.ToList();
            db.Kors.RemoveRange(korzinaToDelete);
            db.SaveChanges();
            var korzinas = from korzina in db.Kors
                           join book in db.Menus on korzina.Id equals book.Id
                           join genre in db.Kitchens on book.Kitchen_ID equals genre.Kitchen_ID
                           join food in db.FoodTypes on book.FoodType_ID equals food.FoodType_ID
                           select new
                           {
                               // Id = book.Id,
                               Kitchen = genre.Kitchen1,
                               Type = food.FoodType1,
                               Food_Name = book.Name,
                               Price = book.Price,
                               Kol = korzina.Kol
                           };
            dataGridView1.DataSource = korzinas.ToList();
            //обновление списка выдач

            var loans = from loan in db.Historys
                        select new
                        {
                            Loan_Id = loan.Id,
                            FIO = Names,
                            Data = loan.Data,
                            AllSum = loan.AllSum,
                            Bonus = loan.Bonus
                        };

            dataGridView3.DataSource = loans.ToList();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.SaveChanges();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();

            DialogResult result = form3.ShowDialog();
            if (result == DialogResult.OK)
            {
                // Обновите DataGridView или другой элемент управления для отображения добавленных данных
                dataGridView2.DataSource = db.Oformlenies.ToList();
                dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.RowCount - 1;
            }
            dataGridView2.Refresh();
            
        }
        public void AddClient(Oformlenie client)
        {
            db.Oformlenies.Add(client);
            db.SaveChanges();

            // Обновление отображения данных в DataGridView
            dataGridView2.DataSource = db.Oformlenies.ToList();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                string client = row.Cells["FIO"].Value.ToString();

                // Выбор данных из таблицы Istory по Client_ID
                var loans = from loan in db.Historys
                            where loan.FIO == client
                            select new
                            {
                                Loan_Id = loan.Id,
                                FIO = loan.FIO,
                                Data = loan.Data,
                                AllSum = loan.AllSum,
                                Bonus = loan.Bonus
                            };

                dataGridView3.DataSource = loans.ToList();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
           
            // Создать экземпляр Form2 и передать выбранный клиент для отображения и редактирования
            Form4 form4 = new Form4();
            //form2.ShowDialog();

            form4.ShowDialog();
            dataGridView2.Refresh();
        }
    }
    
}
