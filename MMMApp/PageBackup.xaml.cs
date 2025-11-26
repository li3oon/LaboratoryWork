using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Data.SqlClient;

namespace MMMApp
{
    /// <summary>
    /// Логика взаимодействия для PageBackup.xaml
    /// </summary>
    public partial class PageBackup : Page
    {
        public PageBackup()
        {
            InitializeComponent();
        }

        private void AppendLog(string s)
        {
            // потокобезопасный вывод в текстовое поле
            if (Dispatcher.CheckAccess())
            {
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {s}{Environment.NewLine}");
                txtLog.ScrollToEnd();
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {s}{Environment.NewLine}");
                    txtLog.ScrollToEnd();
                });
            }
        }

        private string GetMasterConnectionString()
        {
            var cs = ConfigurationManager.ConnectionStrings["ServerConnection"];
            return cs?.ConnectionString ?? throw new InvalidOperationException("Нет строки подключения 'ServerConnection' в App.config");
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = Path.GetFileName(txtPath.Text);
            dlg.InitialDirectory = Path.GetDirectoryName(txtPath.Text) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.Filter = "Backup files (*.bak;*.trn)|*.bak;*.trn|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                txtPath.Text = dlg.FileName;
            }
        }

        private async void BtnBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnBackup.IsEnabled = false;
                AppendLog("Запуск операции бэкапа...");

                string backupType = (cmbType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Full";
                string filePath = txtPath.Text.Trim();
                if (string.IsNullOrEmpty(filePath))
                {
                    AppendLog("Ошибка: укажите путь к файлу бэкапа.");
                    return;
                }

                AppendLog("ВНИМАНИЕ: указанный путь относится к файловой системе SQL Server. Убедитесь, что путь существует на сервере или это UNC-путь, доступный службе SQL Server.");

                string masterConn = GetMasterConnectionString();

                // Все тяжёлые операции выполняем в фоновом потоке
                await Task.Run(() =>
                {
                    try
                    {
                        using (var conn = new SqlConnection(masterConn))
                        {
                            conn.Open();

                            // Проверка существования БД
                            if (!DatabaseExists(conn, "MMM"))
                            {
                                AppendLog("Ошибка: база данных 'MMM' не найдена на сервере.");
                                return;
                            }

                            // Если Log backup выбран, проверяем модель восстановления
                            if (backupType.Equals("Log", StringComparison.OrdinalIgnoreCase))
                            {
                                var recovery = GetRecoveryModel(conn, "MMM");
                                AppendLog($"Текущая модель восстановления: {recovery}");
                                if (string.Equals(recovery, "SIMPLE", StringComparison.OrdinalIgnoreCase))
                                {
                                    AppendLog("Модель восстановления SIMPLE — BACKUP LOG невозможен. Меняю модель на FULL и выполняю полный бэкап перед логом.");
                                    string alter = $"ALTER DATABASE [{EscapeSqlIdentifier("MMM")}] SET RECOVERY FULL WITH ROLLBACK IMMEDIATE;";
                                    ExecuteNonQuery(conn, alter);

                                    AppendLog("Модель восстановления установлена в FULL.");

                                    string fullPath = Path.Combine(Path.GetDirectoryName(filePath) ?? @"D:\Backup", "MMM_AUTO_FULL.BAK");
                                    AppendLog($"Выполняю автоматический полный бэкап: {fullPath}");
                                    string escFullPath = EscapeSqlLiteral(fullPath);
                                    string fullCmd = $"BACKUP DATABASE [{EscapeSqlIdentifier("MMM")}] TO DISK = N'{escFullPath}' WITH INIT, CHECKSUM;";
                                    ExecuteNonQuery(conn, fullCmd);
                                    AppendLog("Автоматический полный бэкап завершён.");
                                }
                            }

                            // Основные типы бэкапа
                            if (backupType.Equals("Full", StringComparison.OrdinalIgnoreCase))
                            {
                                AppendLog($"Выполняю FULL backup -> {filePath}");
                                string cmd = $"BACKUP DATABASE [{EscapeSqlIdentifier("MMM")}] TO DISK = N'{EscapeSqlLiteral(filePath)}' WITH INIT, CHECKSUM;";
                                ExecuteNonQuery(conn, cmd);
                                AppendLog("FULL backup успешно выполнен.");
                            }
                            else if (backupType.Equals("Differential", StringComparison.OrdinalIgnoreCase))
                            {
                                AppendLog($"Выполняю DIFFERENTIAL backup -> {filePath}");
                                string cmd = $"BACKUP DATABASE [{EscapeSqlIdentifier("MMM")}] TO DISK = N'{EscapeSqlLiteral(filePath)}' WITH DIFFERENTIAL, CHECKSUM;";
                                ExecuteNonQuery(conn, cmd);
                                AppendLog("DIFFERENTIAL backup успешно выполнен.");
                            }
                            else if (backupType.Equals("Log", StringComparison.OrdinalIgnoreCase))
                            {
                                AppendLog($"Выполняю LOG backup -> {filePath}");
                                string cmd = $"BACKUP LOG [{EscapeSqlIdentifier("MMM")}] TO DISK = N'{EscapeSqlLiteral(filePath)}' WITH CHECKSUM;";
                                ExecuteNonQuery(conn, cmd);
                                AppendLog("LOG backup успешно выполнен.");
                            }
                            else
                            {
                                AppendLog("Неизвестный тип бэкапа.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Используем AppendLog — он сам маршалит вызов на UI-поток
                        AppendLog("Ошибка: " + ex.Message);
                        if (ex.InnerException != null) AppendLog("Inner: " + ex.InnerException.Message);
                    }
                });
            }
            finally
            {
                btnBackup.IsEnabled = true;
            }
        }

        private void BtnCheckRecovery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppendLog("Проверка/установка recovery model...");
                string masterConn = GetMasterConnectionString();

                using (var conn = new SqlConnection(masterConn))
                {
                    conn.Open();

                    if (!DatabaseExists(conn, "MMM"))
                    {
                        AppendLog("БД MMM не найдена.");
                        return;
                    }

                    var recovery = GetRecoveryModel(conn, "MMM");
                    AppendLog($"Текущая модель восстановления: {recovery}");
                    if (string.Equals(recovery, "SIMPLE", StringComparison.OrdinalIgnoreCase))
                    {
                        string alter = $"ALTER DATABASE [{EscapeSqlIdentifier("MMM")}] SET RECOVERY FULL WITH ROLLBACK IMMEDIATE;";
                        ExecuteNonQuery(conn, alter);
                        AppendLog("Модель восстановления изменена на FULL.");
                    }
                    else
                    {
                        AppendLog("Модель восстановления уже не SIMPLE.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog("Ошибка: " + ex.Message);
            }
        }

        // Вспомогательные методы для работы через T-SQL

        private static bool DatabaseExists(SqlConnection conn, string dbName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT 1 FROM sys.databases WHERE name = @name";
                cmd.Parameters.AddWithValue("@name", dbName);
                var r = cmd.ExecuteScalar();
                return r != null;
            }
        }

        private static string GetRecoveryModel(SqlConnection conn, string dbName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT recovery_model_desc FROM sys.databases WHERE name = @name";
                cmd.Parameters.AddWithValue("@name", dbName);
                var r = cmd.ExecuteScalar();
                return r?.ToString() ?? string.Empty;
            }
        }

        private static void ExecuteNonQuery(SqlConnection conn, string sql)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandTimeout = 0; // без таймаута для больших бэкапов
                cmd.ExecuteNonQuery();
            }
        }

        private static string EscapeSqlLiteral(string input)
        {
            // для использования внутри N'...'
            return input.Replace("'", "''");
        }

        private static string EscapeSqlIdentifier(string identifier)
        {
            // простая экранировка для имен объектов в квадратных скобках
            return identifier.Replace("]", "]]");
        }
    }
}
