using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для PageCatalog.xaml
    /// </summary>
    public partial class PageCatalog : Page
    {
        private List<Model> _models;
        private int _currentIndex;
        public event PropertyChangedEventHandler PropertyChanged;
        public int CurrentIndex => _models.Count == 0 ? 0 : _currentIndex + 1;
        public int TotalCount => _models.Count;
        public PageCatalog()
        {
            InitializeComponent();
            _models = DataBase.GetContext().Model.ToList();

            if (_models.Any())
                DataContext = _models[_currentIndex];

            UpdateCounters();
        }

        private void Btn_Start(object sender, RoutedEventArgs e)
        {
            if (!_models.Any()) return;
            _currentIndex = 0;
            Refresh();
        }

        private void Btn_Back(object sender, RoutedEventArgs e)
        {
            if (_currentIndex <= 0) return;
            _currentIndex--;
            Refresh();
        }

        private void Btn_Forward(object sender, RoutedEventArgs e)
        {
            if (_currentIndex >= _models.Count - 1) return;
            _currentIndex++;
            Refresh();
        }

        private void Btn_End(object sender, RoutedEventArgs e)
        {
            if (!_models.Any()) return;
            _currentIndex = _models.Count - 1;
            Refresh();
        }

        private void Btn_Add(object sender, RoutedEventArgs e)
        {
            var model = new Model
            {
                name = "Новый код модели",
                price = 0
            };

            DataBase.GetContext().Model.Add(model);
            _models.Add(model);

            _currentIndex = _models.Count - 1;
            DataContext = model;

            UpdateCounters();
        }

        private void Btn_Delete(object sender, RoutedEventArgs e)
        {
            if (!_models.Any()) return;

            if (MessageBox.Show(
                "Удалить запись?",
                "Внимание",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            var current = _models[_currentIndex];

            DataBase.GetContext().Model.Remove(current);
            _models.RemoveAt(_currentIndex);

            if (_currentIndex >= _models.Count)
                _currentIndex--;

            DataBase.GetContext().SaveChanges();

            DataContext = _models.Any() ? _models[_currentIndex] : null;
            UpdateCounters();
        }

        private void Btn_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                DataBase.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }
        private void Refresh()
        {
            DataContext = _models[_currentIndex];
            UpdateCounters();
        }

        private void UpdateCounters()
        {
            OnPropertyChanged(nameof(CurrentIndex));
            OnPropertyChanged(nameof(TotalCount));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
