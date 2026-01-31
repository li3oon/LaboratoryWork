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
    public partial class ProductTable : Form
    {
        public ProductTable()
        {
            InitializeComponent();
        }

        private void Products_Load(object sender, EventArgs e)
        {
            // Загружаем данные в DataTable для поддержки сортировки
            LoadProductsToDataTable();

            // Изначально блокируем кнопку сортировки
            button1.Enabled = false;

            // Заполняем ComboBox
            LoadProductNamesToComboBox();
        }

        private void LoadProductsToDataTable()
        {
            try
            {
                using (var db = new COMPUTER_SHOP())
                {
                    var products = db.PRODUCTS.ToList();

                    // Создаем DataTable
                    DataTable table = new DataTable();

                    // Добавляем колонки
                    table.Columns.Add("ID_PRODUCT", typeof(int));
                    table.Columns.Add("NAME", typeof(string));
                    table.Columns.Add("PRICE", typeof(decimal));
                    table.Columns.Add("QUANTITY", typeof(int));
                    table.Columns.Add("RATING", typeof(decimal));
                    table.Columns.Add("DESCRIPTION", typeof(string));

                    // Заполняем данными
                    foreach (var product in products)
                    {
                        table.Rows.Add(
                            product.ID_PRODUCT,
                            product.NAME,
                            product.PRICE,
                            product.QUANTITY,
                            product.RATING,
                            product.DESCRIPTION
                        );
                    }

                    // Привязываем DataTable к BindingSource
                    pRODUCTSBindingSource.DataSource = table;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductNamesToComboBox()
        {
            try
            {
                using (var db = new COMPUTER_SHOP())
                {
                    // Получаем уникальные названия товаров
                    var productNames = db.PRODUCTS
                        .Select(p => p.NAME)
                        .Distinct()
                        .OrderBy(n => n)
                        .ToList();

                    // Заполняем ComboBox
                    comboBox1.DataSource = productNames;
                    comboBox1.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки названий товаров: {ex.Message}",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                this.pRODUCTSBindingSource.EndEdit();

                // Получаем DataTable из BindingSource
                DataTable table = pRODUCTSBindingSource.DataSource as DataTable;

                if (table != null)
                {
                    using (var db = new COMPUTER_SHOP())
                    {
                        // Сохраняем изменения в БД
                        foreach (DataRow row in table.Rows)
                        {
                            if (row.RowState == DataRowState.Modified)
                            {
                                int id = (int)row["ID_PRODUCT"];
                                var product = db.PRODUCTS.Find(id);

                                if (product != null)
                                {
                                    product.NAME = row["NAME"] as string;
                                    product.PRICE = (decimal)row["PRICE"];
                                    product.QUANTITY = (int)row["QUANTITY"];
                                    product.RATING = (decimal)row["RATING"];
                                    product.DESCRIPTION = row["DESCRIPTION"] as string;
                                }
                            }
                        }

                        db.SaveChanges();
                        MessageBox.Show("Изменения сохранены", "Сохранение",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Разблокируем кнопку сортировки при выборе пункта
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверяем, что выбран пункт в списке
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите поле для сортировки", "Внимание",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Определяем имя поля для сортировки
            string sortColumn = "";

            switch (listBox1.SelectedIndex)
            {
                case 0: // Наименование
                    sortColumn = "NAME";
                    break;
                case 1: // Цена
                    sortColumn = "PRICE";
                    break;
                case 2: // Количество
                    sortColumn = "QUANTITY";
                    break;
                case 3: // Оценка
                    sortColumn = "RATING";
                    break;
                case 4: // Описание
                    sortColumn = "DESCRIPTION";
                    break;
                default:
                    return;
            }

            // Получаем DataTable из BindingSource
            DataTable table = pRODUCTSBindingSource.DataSource as DataTable;

            if (table != null)
            {
                // Определяем направление сортировки
                string sortDirection = radioButton1.Checked ? "ASC" : "DESC";

                // Сортируем DataTable
                table.DefaultView.Sort = $"{sortColumn} {sortDirection}";

                // Обновляем DataGridView
                pRODUCTSDataGridView.DataSource = table.DefaultView;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Фильтрация по названию товара
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("Выберите товар для фильтрации", "Внимание",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string filterValue = comboBox1.Text.Replace("'", "''");
            pRODUCTSBindingSource.Filter = $"NAME = '{filterValue}'";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Сброс фильтрации
            pRODUCTSBindingSource.Filter = "";
            comboBox1.SelectedIndex = -1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Проверяем, что введен текст для поиска
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Введите текст для поиска", "Внимание",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string searchText = textBox1.Text.ToLower();

            // Сбрасываем предыдущее выделение
            ResetCellColors();

            // Поиск и выделение ячеек
            for (int i = 0; i < pRODUCTSDataGridView.RowCount; i++)
            {
                for (int j = 0; j < pRODUCTSDataGridView.ColumnCount; j++)
                {
                    if (pRODUCTSDataGridView[j, i].Value != null)
                    {
                        string cellValue = pRODUCTSDataGridView[j, i].Value.ToString().ToLower();

                        if (cellValue.Contains(searchText))
                        {
                            pRODUCTSDataGridView[j, i].Style.BackColor = Color.AliceBlue;
                            pRODUCTSDataGridView[j, i].Style.ForeColor = Color.Blue;
                            pRODUCTSDataGridView[j, i].Style.Font =
                                new Font(pRODUCTSDataGridView.Font, FontStyle.Bold);
                        }
                    }
                }
            }
        }

        private void ResetCellColors()
        {
            for (int i = 0; i < pRODUCTSDataGridView.RowCount; i++)
            {
                for (int j = 0; j < pRODUCTSDataGridView.ColumnCount; j++)
                {
                    pRODUCTSDataGridView[j, i].Style.BackColor = Color.White;
                    pRODUCTSDataGridView[j, i].Style.ForeColor = Color.Black;
                    pRODUCTSDataGridView[j, i].Style.Font =
                        new Font(pRODUCTSDataGridView.Font, FontStyle.Regular);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Опционально: Если нужно сохранять оригинальный подход с DataGridViewColumn
        // но использовать DataTable для поддержки сортировки
        private void AlternativeSortMethod()
        {
            // Альтернативный метод сортировки с использованием DataGridViewColumn
            if (listBox1.SelectedIndex < 0) return;

            string sortColumn = "";

            switch (listBox1.SelectedIndex)
            {
                case 0: sortColumn = "NAME"; break;
                case 1: sortColumn = "PRICE"; break;
                case 2: sortColumn = "QUANTITY"; break;
                case 3: sortColumn = "RATING"; break;
                case 4: sortColumn = "DESCRIPTION"; break;
            }

            // Используем DataView для сортировки
            DataTable table = pRODUCTSBindingSource.DataSource as DataTable;
            if (table != null)
            {
                DataView dv = table.DefaultView;
                dv.Sort = radioButton1.Checked ? $"{sortColumn} ASC" : $"{sortColumn} DESC";
                pRODUCTSDataGridView.DataSource = dv;
            }
        }
    }
}