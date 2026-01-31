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
    public partial class Users : Form
    {
        public Users()
        {
            InitializeComponent();
            this.Text = "Пользователи";
        }
        private void Users_Load(object sender, EventArgs e)
        {
            this.usersTableAdapter.Fill(this.mMM1DataSet.Users);
        }

        private void usersBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.usersBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.mMM1DataSet);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
