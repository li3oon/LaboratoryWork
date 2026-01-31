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
    public partial class ProductGroups : Form
    {
        public ProductGroups()
        {
            InitializeComponent();
            this.Text = "Товарные группы";
        }
        private void Catalogs_Load(object sender, EventArgs e)
        {
            this.cATALOGSBindingSource.DataSource =
                new COMPUTER_SHOP().CATALOGS.ToList();
        }

        private void BindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.cATALOGSBindingSource.EndEdit();

            using (var db = new COMPUTER_SHOP())
            {
                db.SaveChanges();
            }
        }
    }
}
