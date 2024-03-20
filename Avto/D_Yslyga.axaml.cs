using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using MySql.Data.MySqlClient;

namespace Avto
{
    public partial class D_Yslyga : Window
    {
        private Yslyga _yslygaForm;
        private string _connString = "server=localhost;database=YP_O;port=3306;User Id=sammy;password=Andr%123; Charset=utf8";
        private Services _selectedService;

        public D_Yslyga(Yslyga yslygaForm, Services selectedService)
        {
            InitializeComponent();
            _yslygaForm = yslygaForm;
            _selectedService = selectedService;

            if (_selectedService != null) // Проверяем, редактирование или добавление
            {
                // Заполнение формы данными выбранной услуги
                FillFormWithServiceData();

                // Установить заголовок окна на "Изменение услуги"
                Title = "Редактирование услуги";

                // Скрыть кнопку "Добавить", если открыта форма для редактирования
                AddServiceButton.IsVisible = false;
            }
            else
            {
                // Установить заголовок окна на "Добавление услуги"
                Title = "Добавление услуги";

                // Скрыть кнопку "Изменить", если открыта форма для добавления
                EditServiceButton.IsVisible = false;
            }
        }


        
        // Добавляем метод для заполнения полей формы данными выбранной услуги
        private void FillFormWithServiceData()
        {
            if (_selectedService != null)
            {
                NameTextBox.Text = _selectedService.name;
                DescriptionTextBox.Text = _selectedService.description;
                PriceTextBox.Text = _selectedService.price.ToString();
                DiscountTextBox.Text = _selectedService.discount.ToString();

                // Преобразуем массив байт в изображение и устанавливаем его в элемент Image
                if (_selectedService.photo != null && _selectedService.photo.Length > 0)
                {
                    using (MemoryStream stream = new MemoryStream(_selectedService.photo))
                    {
                        var bitmap = new Bitmap(stream);
                        PhotoImage.Source = bitmap;
                    }
                }
            }
        }
        
        
        private void AddService_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PriceTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PhotoTextBox.Text))
                {
                    Console.WriteLine("Please fill in all required fields.");
                    return;
                }

                string photoPath = PhotoTextBox.Text;

                if (!File.Exists(photoPath))
                {
                    Console.WriteLine("Selected file does not exist.");
                    return;
                }

                int discount = 0;
                if (!string.IsNullOrWhiteSpace(DiscountTextBox.Text))
                {
                    int.TryParse(DiscountTextBox.Text, out discount);
                }

                InsertService(NameTextBox.Text, DescriptionTextBox.Text,
                    double.Parse(PriceTextBox.Text), photoPath, discount);
                _yslygaForm.ShowTable();
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding service: {ex.Message}");
            }
        }

        private void InsertService(string name, string description, double price, string photoPath, int discount)
        {
            string sql = "INSERT INTO Services (name, description, price, photo, discount) " +
                         "VALUES (@Name, @Description, @Price, @Photo, @Discount)";

            using (MySqlConnection connection = new MySqlConnection(_connString))
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@Price", price);
                
                // Читаем содержимое файла в массив байт
                byte[] photoBytes = File.ReadAllBytes(photoPath);
                command.Parameters.AddWithValue("@Photo", photoBytes); // Передаем массив байт
                
                command.Parameters.AddWithValue("@Discount", discount);
                command.ExecuteNonQuery();
            }
        }

        private async void SelectFile_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                var result = await dialog.ShowAsync(this);

                if (result != null && result.Length > 0)
                {
                    var filePath = result[0];

                    if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                    {
                        PhotoTextBox.Text = filePath;

                        // Устанавливаем источник изображения для элемента Image
                        PhotoImage.Source = new Bitmap(filePath);
                    }
                    else
                    {
                        Console.WriteLine("Selected file does not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting file: {ex.Message}");
            }
        }

        private void EditService_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedService != null)
                {
                    if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                        string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                        string.IsNullOrWhiteSpace(PriceTextBox.Text) ||
                        string.IsNullOrWhiteSpace(PhotoTextBox.Text))
                    {
                        Console.WriteLine("Please fill in all required fields.");
                        return;
                    }

                    string photoPath = PhotoTextBox.Text;

                    if (!File.Exists(photoPath))
                    {
                        Console.WriteLine("Selected file does not exist.");
                        return;
                    }

                    int discount = 0;
                    if (!string.IsNullOrWhiteSpace(DiscountTextBox.Text))
                    {
                        int.TryParse(DiscountTextBox.Text, out discount);
                    }

                    UpdateService(_selectedService.serviceID, NameTextBox.Text, DescriptionTextBox.Text,
                        double.Parse(PriceTextBox.Text), photoPath, discount);
                    _yslygaForm.ShowTable();
                    Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing service: {ex.Message}");
            }
        }
        private void UpdateService(int serviceID, string name, string description, double price, string photoPath, int discount)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(description) ||
                    string.IsNullOrWhiteSpace(price.ToString()) ||
                    string.IsNullOrWhiteSpace(photoPath))
                {
                    Console.WriteLine("Please fill in all required fields.");
                    return;
                }

                if (!File.Exists(photoPath))
                {
                    Console.WriteLine("Selected file does not exist.");
                    return;
                }

                string sql = "UPDATE Services " +
                             "SET name = @Name, description = @Description, price = @Price, photo = @Photo, discount = @Discount " +
                             "WHERE serviceID = @ServiceID";

                using (MySqlConnection connection = new MySqlConnection(_connString))
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ServiceID", serviceID);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Price", price);

                    // Читаем содержимое файла в массив байт
                    byte[] photoBytes = File.ReadAllBytes(photoPath);
                    command.Parameters.AddWithValue("@Photo", photoBytes); // Передаем массив байт

                    command.Parameters.AddWithValue("@Discount", discount);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating service: {ex.Message}");
            }
        }


        
        
        
        
        
        
    }
}
