using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;

namespace MMMApp
{
    /// <summary>
    /// Логика взаимодействия для PageAddPrivilege.xaml
    /// </summary>
    public partial class PageAddPrivilege : Page
    {
        public PageAddPrivilege()
        {
            InitializeComponent();
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string user = txtUser.Text.Trim();
                string priv = (cmbPriv.SelectedItem as ComboBoxItem)?.Content.ToString();
                string table = (cmbTable.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (user == "" || priv == null || table == null)
                {
                    MessageBox.Show("Заполните все поля.");
                    return;
                }

                var cs = ConfigurationManager.ConnectionStrings["ServerConnection"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(cs))
                {
                    conn.Open();

                    string sql = $"GRANT {priv} ON dbo.{table} TO [{user}]";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Привилегия успешно назначена.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
