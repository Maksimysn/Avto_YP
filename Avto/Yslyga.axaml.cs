using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;
//using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Avalonia.Media.Imaging; // Для работы с изображениями Bitmap
using System.IO;
using Avalonia.Data.Converters; // Для работы с файлами и потоками данных




namespace Avto;

public partial class Yslyga : Window
{
    private Services _selectedServices;
    private MySqlConnection _connection;
    private List<Services> _services;
    private string _connString = "server=localhost;database=YP_O;port=3306;User Id=sammy;password=Andr%123; Charset=utf8";

    public Yslyga()
    {
        InitializeComponent();
        ShowTable();
        _connection = new MySqlConnection(_connString);
        ServicesGrid.SelectionChanged += ServicesGrid_SelectionChanged;
        SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        discountComboBox.SelectionChanged += discountComboBox_SelectionChanged;
        discountComboBox.ItemsSource = Getdiscount();
    }

    public void ShowTable()
    {
        string sql = "SELECT serviceID, name, description, price, photo, discount FROM Services";

        _services = new List<Services>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var currentService = new Services
                    {
                        serviceID = reader.GetInt32("serviceID"),
                        name = reader.GetString("name"),
                        description = reader.GetString("description"),
                        price = reader.GetInt32("price"),
                        photo = reader["photo"] as byte[], // Преобразование изображения в массив байт
                        discount = reader.GetInt32("discount")
                    };

                    _services.Add(currentService);
                }
            }
        }

        ServicesGrid.ItemsSource = _services;
    }

       
    private void ServicesGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
       if (ServicesGrid.SelectedItem is Services selectedServices)
       {
           _selectedServices = selectedServices;
       }
    }
    
    
   

    
    private void OpenNextForm_Click(object? sender, RoutedEventArgs e)
    {
        var addForm = new D_Yslyga(this, null); // Передаем null, потому что форма открыта для добавления
        addForm.Show();
    }

    private void DeleteServicesGrid_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedServices != null)
        {
            DeleteServices(_selectedServices.serviceID);
        }
    }
        
    private void DeleteServices(int serviceID)
    {
        using (_connection = new MySqlConnection(_connString))
        {
            _connection.Open();
            string queryString = $"DELETE FROM Services WHERE serviceID = {serviceID}";
            MySqlCommand command = new MySqlCommand(queryString, _connection);
            command.ExecuteNonQuery();
        }

        ShowTable();
    }
    

    private void EditServicesGrid_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedServices != null)
        {
            var editForm = new D_Yslyga(this, _selectedServices); // Передаем выбранную услугу
            editForm.Show();
        }
    }
    

    private List<int> Getdiscount()
    {
        List<int> discounts = new List<int>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            string sql = "SELECT discount FROM Services"; // Выбираем только столбец discount
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Проверяем, не является ли значение DBNull, и если нет, преобразуем его в int
                    if (reader["discount"] != DBNull.Value)
                    {
                        discounts.Add(reader.GetInt32("discount"));
                    }
                }
            }
        }

        return discounts;
    }

    
    private void FilterByDiscount(int discount) // Изменено на int
    {
        string sql = "SELECT serviceID, name, description, price, photo, discount FROM Services WHERE discount = @Discount";

        List<Services> filteredServices = new List<Services>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Discount", discount);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var currentService = new Services
                    {
                        serviceID = reader.GetInt32("serviceID"),
                        name = reader.GetString("name"),
                        description = reader.GetString("description"),
                        price = reader.GetInt32("price"),
                        photo = reader["photo"] as byte[], // Преобразование изображения в массив байт
                        discount = reader.GetInt32("discount")
                    };

                    filteredServices.Add(currentService);
                }
            }
        }

        ServicesGrid.ItemsSource = filteredServices;
    }

    
    private void discountComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (discountComboBox.SelectedItem is int selectedDiscount)
        {
            FilterByDiscount(selectedDiscount);
        }
    }
    
    private void ResetFilterButton_Click(object? sender, RoutedEventArgs e)
    {
        // Сбросить фильтр и отобразить полную таблицу
        ShowTable();
 
        // Очистить выбор в ComboBox
        discountComboBox.SelectedItem = null;
    }
    
    
    private bool _ascendingOrder = true; // Флаг для отслеживания порядка сортировки

    private void SortBy_Click(object? sender, RoutedEventArgs e)
    {
        if (_services != null && _services.Count > 0)
        {
            if (_ascendingOrder)
            {
                // Сортировка по возрастанию
                _services = _services.OrderBy(s => s.name).ToList();
            }
            else
            {
                // Сортировка по убыванию
                _services = _services.OrderByDescending(s => s.name).ToList();
            }

            // Изменяем порядок сортировки на противоположный
            _ascendingOrder = !_ascendingOrder;

            // Обновляем отображаемые данные в DataGrid
            ServicesGrid.ItemsSource = _services;
        }
    }

    
    private void SearchTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
        string searchQuery = SearchTextBox.Text.Trim();
        if (!string.IsNullOrEmpty(searchQuery))
        {
            // Выполните поиск по названию услуги и цене и обновите данные в DataGrid
            SearchAndRefreshTable(searchQuery);
        }
        else
        {
            // Если поле поиска пусто, отобразите все данные
            ShowTable();
        }
    }

    
    private void SearchAndRefreshTable(string searchQuery)
    {
        if (_services == null || _services.Count == 0)
        {
            return;
        }

        searchQuery = searchQuery.ToLower();

        // Фильтруем коллекцию _services по критериям поиска
        var filteredServices = _services
            .Where(s =>
                s.name.ToLower().Contains(searchQuery) ||
                s.price.ToString().Contains(searchQuery))
            .ToList();

        // Обновляем отображаемые данные в DataGrid
        ServicesGrid.ItemsSource = filteredServices;
    }
    
    
    
    
    
    
    
    
    
    
    
}