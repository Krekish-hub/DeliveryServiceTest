using DeliveryServiceTest.DataFiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DeliveryServiceTest
{
    public partial class MainWindow : Window
    {
        private const int ChunkSize = 50;
        private ApplicationDbContext _context;
        public ObservableCollection<Order> OrdersCollection { get; set; } = new ObservableCollection<Order>();
        private string logFilePath;

        public MainWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            OrdersDataGrid.ItemsSource = OrdersCollection;

            logFilePath = "delivery_log.txt";
        }

        private async void FilterOrders_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                string district = DistrictTextBox.Text;
                DateTime firstDeliveryTime = DateTime.Parse(FirstDeliveryTimeTextBox.Text);
                await LoadOrdersChunksAsync(district, firstDeliveryTime);
            }
            else
            {
                Log("Ошибка валидации: некорректные данные.");
            }
        }

        private async Task LoadOrdersChunksAsync(string district, DateTime firstDeliveryTime)
        {
            OrdersCollection.Clear();
            int skip = 0;

            while (true)
            {
                using (var context = new ApplicationDbContext())
                {
                    var query = context.Orders
                        .Where(o => o.District.Contains(district) &&
                                    o.DeliveryTime >= firstDeliveryTime &&
                                    o.DeliveryTime <= firstDeliveryTime.AddMinutes(30))
                        .Skip(skip)
                        .Take(ChunkSize);

                    var chunk = await query.ToListAsync();

                    if (chunk.Count == 0) break;

                    foreach (var order in chunk)
                    {
                        OrdersCollection.Add(order);
                    }

                    skip += ChunkSize;
                }
            }

            Log("Фильтрация завершена успешно.");
        }

        private bool ValidateInputs()
        {
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(DistrictTextBox.Text))
            {
                ErrorMessageTextBlock.Text = "Введите район.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return false;
            }

            DateTime parsedDate;
            if (!DateTime.TryParse(FirstDeliveryTimeTextBox.Text, out parsedDate))
            {
                ErrorMessageTextBlock.Text = "Некорректный формат времени.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return false;
            }

            return true;
        }

        private void DistrictTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInputs();
        }

        private void FirstDeliveryTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInputs();
        }

        private void Log(string message)
        {
            string logMessage = $"{DateTime.Now}: {message}\n";
            LogTextBox.AppendText(logMessage);
            LogTextBox.ScrollToEnd();

            File.AppendAllText(logFilePath, logMessage);
        }
    }
}
