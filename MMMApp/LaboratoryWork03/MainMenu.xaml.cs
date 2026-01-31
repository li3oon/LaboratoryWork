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
using WinForms = System.Windows.Forms;

namespace MMMApp.LaboratoryWork03
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        private Users users;
        private ProductGroups productGroups;
        private Orders orders;
        private Product product;

        public MainMenu()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            productGroups = new ProductGroups();
            productGroups.Visible = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            product = new Product();
            product.Visible = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            orders = new Orders();
            orders.Visible = true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            users = new Users();
            users.Visible = true;
        }
    }
}
