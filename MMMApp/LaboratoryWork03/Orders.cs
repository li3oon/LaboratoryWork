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
    public partial class Orders : Form
    {
        public Orders()
        {
            InitializeComponent();
            this.Text = "Заказы";

        }

        private void Order_Load(object sender, EventArgs e)
        {
            this.oRDERSBindingSource.DataSource =
                new COMPUTER_SHOP().ViewORDER.ToList();
        }

        private void BindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.oRDERSBindingSource.EndEdit();

            using (var db = new COMPUTER_SHOP())
            {
                db.SaveChanges();
            }
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {

        }

        private void pRICELabel_Click(object sender, EventArgs e)
        {

        }

        private void pRICETextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void eMPLOYEESURNAMELabel_Click(object sender, EventArgs e)
        {

        }

        private void eMPLOYEESURNAMETextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //класс System.Convert позволяет преобразовывать несовместимые в C# типы данных 
            textBox1.Text = Convert.ToString(Convert.ToDouble(pRICETextBox.Text) *
            Convert.ToDouble(nUMBERTextBox.Text));
        }
    }
}
