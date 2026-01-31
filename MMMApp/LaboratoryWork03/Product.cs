using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMMApp.LaboratoryWork03
{
    public partial class Product : Form
    {
        private ProductTable productstable;
        public Product()
        {
            InitializeComponent();
            this.Text = "Товары";

        }

        private void Products_Load(object sender, EventArgs e)
        {
            this.pRODUCTSBindingSource.DataSource =
                new COMPUTER_SHOP().PRODUCTS.ToList();
        }

        private void BindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.pRODUCTSBindingSource.EndEdit();

            using (var db = new COMPUTER_SHOP())
            {
                db.SaveChanges();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pRODUCTSBindingSource.MoveFirst();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pRODUCTSBindingSource.MovePrevious();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            pRODUCTSBindingSource.MoveNext();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            pRODUCTSBindingSource.MoveLast();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            pRODUCTSBindingSource.AddNew();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            pRODUCTSBindingSource.RemoveCurrent();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            //проверяет введённые в поля данные на соответствие типам данных полей 
            this.Validate();
            //закрывает подключение с сервером 
            this.pRODUCTSBindingSource.EndEdit();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            productstable = new ProductTable();
            productstable.Visible = true;
        }
    }
}
