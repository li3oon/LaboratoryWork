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

namespace MMMApp
{
    /// <summary>
    /// Логика взаимодействия для PageShop.xaml
    /// </summary>
    public partial class PageShop : Page
    {
        private Store _store;
        public PageShop()
        {
            InitializeComponent();
            // создаём новый магазин
            _store = new Store();

            // назначаем DataContext
            DataContext = _store;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // простая валидация
                if (string.IsNullOrWhiteSpace(_store.name))
                {
                    MessageBox.Show("Введите название магазина");
                    return;
                }

                DataBase.GetContext().Store.Add(_store);
                DataBase.GetContext().SaveChanges();

                MessageBox.Show("Магазин добавлен");

                // создаём новый объект для следующего ввода
                _store = new Store();
                DataContext = _store;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _store = new Store();
            DataContext = _store;
        }
    }
}
