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
    /// Логика взаимодействия для PageAddUser.xaml
    /// </summary>
    public partial class PageAddUser : Page
    {
        public PageAddUser()
        {
            InitializeComponent();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (login == "" || pass == "")
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            try
            {
                // Подключаемся строкой ServerConnection (как в PageBackup)
                var cs = ConfigurationManager.ConnectionStrings["ServerConnection"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(cs))
                {
                    conn.Open();

                    // 1. Создать логин
                    SqlCommand cmd1 = new SqlCommand(
                        $"CREATE LOGIN [{login}] WITH PASSWORD='{pass}'",
                        conn);
                    cmd1.ExecuteNonQuery();

                    // 2. Создать пользователя в базе MMM
                    SqlCommand cmd2 = new SqlCommand(
                        $"CREATE USER [{login}] FOR LOGIN [{login}]",
                        conn);
                    cmd2.ExecuteNonQuery();
                }

                MessageBox.Show("Пользователь успешно создан.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
