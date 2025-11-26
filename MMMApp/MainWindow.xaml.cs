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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Навигация по умолчанию
            MainFrame.Navigate(new Start());
            Manager.MainFrame = MainFrame;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.MainFrame.CanGoBack)
                Manager.MainFrame.GoBack();
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            BtnBack.Visibility = Manager.MainFrame.CanGoBack ? Visibility.Visible : Visibility.Hidden;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new PageAddUser());
        }

        private void AddPriv_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new PageAddPrivilege());
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new PageBackup());
        }
    }
}
