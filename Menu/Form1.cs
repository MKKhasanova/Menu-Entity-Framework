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
    public partial class Form1 : Form
    {

        Database1Entities db = new Database1Entities();
        string selectedKitchen, selectedFoodType;
        int index1, index2;
        
        int kitchen_ID, foodtype_ID;
        int Sum;
       

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndexChanged += comboBox_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += comboBox_SelectedIndexChanged;

        }

       

        private void button1_Click(object sender, EventArgs e)//выбор
        {
            DataGridViewRow curRow = dataGridView1.CurrentRow;
            int Menu_ID = (int)curRow.Cells["Menu_ID"].Value;
            string Kitc = curRow.Cells["Kitchen"].Value.ToString();
            string FoodTp = curRow.Cells["FoodType"].Value.ToString();
            string Foodname = curRow.Cells["Menu_Name"].Value.ToString();
            decimal Pric = Convert.ToDecimal(curRow.Cells["Price"].Value);
            int Koli = Convert.ToInt32(textBox1.Text);

            Kor bookToKorzina = new Kor();
            bookToKorzina.Id = Menu_ID;
            bookToKorzina.Kitchen = Kitc;
            bookToKorzina.Type = FoodTp;
            bookToKorzina.Name = Foodname;
            bookToKorzina.Price = Pric;
            bookToKorzina.Kol = Koli;

            db.Kors.Add(bookToKorzina);

            Menu book1 = db.Menus.FirstOrDefault(x => x.Id == Menu_ID);

            // Уменьшаем значение в столбце "Kol" на 1 в DataGridView1
            Menu kolli = db.Menus.Where(x => x.Id == Menu_ID).FirstOrDefault();
            kolli.Koll -= Koli;

            db.SaveChanges();
            UpdateDataGridView();
            var korzinas = from korzina in db.Kors
                           join book in db.Menus on korzina.Id equals book.Id
                           join genre in db.Kitchens on book.Kitchen_ID equals genre.Kitchen_ID
                           join food in db.FoodTypes on book.FoodType_ID equals food.FoodType_ID
                           select new
                           {
                               Id = book.Id,
                               Kitchen = genre.Kitchen1,
                               Type = food.FoodType1,
                               Name = book.Name,
                               Price = book.Price,
                               Kol = korzina.Kol
                           };

            dataGridView2.DataSource = korzinas.ToList();

            decimal totalCost = (decimal)korzinas.Sum(item => item.Price * item.Kol);
            textBox2.Text = totalCost.ToString();
            textBox1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)// отказ
        {
            DataGridViewRow curRow = dataGridView2.CurrentRow;
            int Menu_ID = (int)curRow.Cells["Id"].Value;
            Kor menuToDelete = db.Kors.Where(x => x.Id == Menu_ID).FirstOrDefault();
            Menu book1 = db.Menus.Where(x => x.Id == Menu_ID).FirstOrDefault();

            if (menuToDelete.Kol == 1)
            {
                if (menuToDelete != null)
                {
                    db.Kors.Remove(menuToDelete);
                }
            }
            else if (menuToDelete.Kol > 1)
            {
                if (menuToDelete != null)
                {
                    menuToDelete.Kol -= 1;
                }
            }
            Menu kolli = db.Menus.Where(x => x.Id == Menu_ID).FirstOrDefault();
            kolli.Koll += 1;
            db.SaveChanges();
            UpdateDataGridView();
            // Обновляем dataGridView2
            var korzinas = from korzina in db.Kors
                           join book in db.Menus on korzina.Id equals book.Id
                           join genre in db.Kitchens on book.Kitchen_ID equals genre.Kitchen_ID
                           join food in db.FoodTypes on book.FoodType_ID equals food.FoodType_ID
                           select new
                           {
                               Id = book.Id,
                               Kitchen = genre.Kitchen1,
                               Type = food.FoodType1,
                               Name = book.Name,
                               Price = book.Price,
                               Kol = korzina.Kol
                           };

            dataGridView2.DataSource = korzinas.ToList();

            // Вычисляем новое значение totalCost
            decimal totalCost = (decimal)korzinas.Sum(item => item.Price * item.Kol);
            textBox2.Text = totalCost.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
            Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                int menuId = Convert.ToInt32(row.Cells["Menu_ID"].Value);

                var selectedMenu = db.Menus.FirstOrDefault(m => m.Id == menuId);
                if (selectedMenu != null)
                {
                    string imgPath = selectedMenu.Img;
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        Image originalImage = Image.FromFile(imgPath);
                        Image scaledImage = ScaleImage(originalImage, pictureBox1.Width, pictureBox1.Height);
                        pictureBox1.Image = scaledImage;
                    }
                    string inf = selectedMenu.Inf;
                    label3.Text = inf;
                }
            }
        }

        // Метод для масштабирования изображения под заданный размер
        private Image ScaleImage(Image image, int width, int height)
        {
            Bitmap scaledImage = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(scaledImage))
            {
                graphics.DrawImage(image, 0, 0, width, height);
            }
            return scaledImage;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            UpdateDataGridView();

            List<Kor> Kors = db.Kors.ToList<Kor>();
            dataGridView2.DataSource = Kors.ToList();
            List<Kitchen> Kitchens = db.Kitchens.ToList<Kitchen>();
            Kitchen allKitchens = new Kitchen();
            allKitchens.Kitchen_ID = 0;
            allKitchens.Kitchen1 = "Кухня";
            Kitchens.Insert(0, allKitchens);

            comboBox1.DataSource = Kitchens;
            comboBox1.ValueMember = "Kitchen_ID";
            comboBox1.DisplayMember = "Kitchen1";
            // Заполнение comboBox2
            List<FoodType> FoodTypes = db.FoodTypes.ToList<FoodType>();
            FoodType allFoodType = new FoodType();
            allFoodType.FoodType_ID = 0;
            allFoodType.FoodType1 = "Меню";
            FoodTypes.Insert(0, allFoodType);

            comboBox2.DataSource = FoodTypes;
            comboBox2.ValueMember = "FoodType_ID";
            comboBox2.DisplayMember = "FoodType1";
            
        }

        private void UpdateDataGridView()//обновление dataGridView1
        {
            var menu_ = from menu in db.Menus
                        join kitchen in db.Kitchens on menu.Kitchen_ID equals kitchen.Kitchen_ID
                        join foodtype in db.FoodTypes on menu.FoodType_ID equals foodtype.FoodType_ID
                        select new
                        {
                            Menu_ID = menu.Id,
                            Kitchen = kitchen.Kitchen1,
                            FoodType = foodtype.FoodType1,
                            foodtype_ID=foodtype.FoodType_ID,
                            Menu_Name = menu.Name,
                            Price = menu.Price,
                            Weight = menu.Weight,
                            Koll = menu.Koll,
                            inf = menu.Inf,
                            KOLIC_iD=kitchen_ID
                        };
            dataGridView1.DataSource = menu_.ToList();
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)//combobox выбор
        {
            Console.WriteLine("ComboBox SelectedIndexChanged event triggered.");
            IQueryable<Menu> query = db.Menus;
            index1 = comboBox1.SelectedIndex;
            index2 = comboBox2.SelectedIndex;

            if (index1 > 0)
            {
                Kitchen kitchen = (Kitchen)comboBox1.SelectedItem;
                selectedKitchen = kitchen.Kitchen1;
                kitchen_ID = db.Kitchens
                        .Where(k => k.Kitchen1 == selectedKitchen)
                        .Select(k => k.Kitchen_ID)
                        .FirstOrDefault();

                query = query.Where(menu => menu.Kitchen_ID == kitchen_ID);
            }

            if (index2 > 0)
            {
                FoodType foodType = (FoodType)comboBox2.SelectedItem;
                selectedFoodType = foodType.FoodType1;
                foodtype_ID = db.FoodTypes
                        .Where(f => f.FoodType1 == selectedFoodType)
                        .Select(f => f.FoodType_ID)
                        .FirstOrDefault();

                query = query.Where(menu => menu.FoodType_ID == foodtype_ID);
            }

            List<Menu> filteredMenus = query.ToList();

            var men = from menu in filteredMenus
                        join kitchen in db.Kitchens on menu.Kitchen_ID equals kitchen.Kitchen_ID
                        join foodType in db.FoodTypes on menu.FoodType_ID equals foodType.FoodType_ID
                        select new
                        {
                            Menu_ID = menu.Id,
                            Kitchen = kitchen.Kitchen1,
                            FoodType = foodType.FoodType1,
                            Menu_Name = menu.Name,
                            Price = menu.Price,
                            Weight = menu.Weight,
                            Koll = menu.Koll
                        };

            dataGridView1.DataSource = men.ToList();
        }
    }
}
