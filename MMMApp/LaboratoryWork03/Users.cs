using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace MMMApp.LaboratoryWork03
{
    public partial class Users : Form
    {
       
        public Users()
        {
            InitializeComponent();
            this.Text = "Пользователи";

        }
        private void Users_Load(object sender, EventArgs e)
        {
            this.uSERSBindingSource.DataSource =
                new COMPUTER_SHOP().USERS.ToList();
        }

        private void BindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.uSERSBindingSource.EndEdit();

            using (var db = new COMPUTER_SHOP())
            {
                db.SaveChanges();
            }
        }
        private void label1_Click(object sender, EventArgs e) { }

    }
}
