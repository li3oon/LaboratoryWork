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
            this.users1BindingSource.DataSource =
                new DataBase().Users1.ToList();
        }

        private void usersBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.users1BindingSource.EndEdit();

            using (var db = new DataBase())
            {
                db.SaveChanges();
            }
        }
        private void label1_Click(object sender, EventArgs e) { }

    }
}
