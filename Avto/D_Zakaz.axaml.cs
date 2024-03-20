using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Avto
{
    public partial class D_Zakaz : Window
    {
        private Zakaz _zakazForm;
        private string _connString = "server=localhost;database=YP_O;port=3306;User Id=sammy;password=Andr%123; Charset=utf8";

        public D_Zakaz(Zakaz zakazForm)
        {
            InitializeComponent();
            _zakazForm = zakazForm;
            
             // Заполняем ComboBox данными о клиентах и услугах
                FillCustomersComboBox();
                FillServicesComboBox();

        }

        private void AddOrderButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomersComboBox.SelectedItem == null || ServicesComboBox.SelectedItem == null || DatePicker.SelectedDate == null)
                {
                    Console.WriteLine("Please select a customer, a service, and a date for the order.");
                    return;
                }

                // Получаем выбранного клиента и услугу
                var selectedCustomer = (Customers)CustomersComboBox.SelectedItem;
                var selectedService = (Services)ServicesComboBox.SelectedItem;

                
               

                var orderDate = DatePicker.SelectedDate.GetValueOrDefault().DateTime;
                
                // Выполняем вставку заказа
                InsertOrder(selectedCustomer.customerID, selectedService.serviceID, orderDate);

                // Обновляем таблицу заказов на основной форме
                _zakazForm.ShowTable();

                // Закрываем окно
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding order: {ex.Message}");
            }
        }

        private void InsertOrder(int customerID, int serviceID, DateTime orderDate)
        {
            string sql = "INSERT INTO Orders (customer_ID, service_ID, date) " +
                         "VALUES (@CustomerID, @ServiceID, @OrderDate)";

            using (MySqlConnection connection = new MySqlConnection(_connString))
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@CustomerID", customerID);
                command.Parameters.AddWithValue("@ServiceID", serviceID);
                command.Parameters.AddWithValue("@OrderDate", orderDate);
                command.ExecuteNonQuery();
            }
        }
        
        
        
       private void FillCustomersComboBox()
{
    try
    {
        string sql = "SELECT * FROM Customers";
        List<Customers> customers = new List<Customers>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var customer = new Customers
                    {
                        customerID = reader.GetInt32("customerID"),
                        nameC = reader.GetString("nameC"),
                        email = reader.GetString("email"),
                        phone_number = reader.GetString("phone_number"),
                        gender = reader.GetString("gender"),
                        registration_date = reader.GetDateTime("registration_date")
                    };

                    customers.Add(customer);
                }
            }
        }

        foreach (var customerName in customers.Select(c => c.nameC))
        {
            CustomersComboBox.Items.Add(customerName);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error filling customers combo box: {ex.Message}");
    }
}

private void FillServicesComboBox()
{
    try
    {
        string sql = "SELECT * FROM Services";
        List<Services> services = new List<Services>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var service = new Services
                    {
                        serviceID = reader.GetInt32("serviceID"),
                        name = reader.GetString("name"),
                        description = reader.GetString("description"),
                        // Другие свойства услуги могут быть добавлены здесь
                    };

                    services.Add(service);
                }
            }
        }

        foreach (var serviceName in services.Select(s => s.name))
        {
            ServicesComboBox.Items.Add(serviceName);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error filling services combo box: {ex.Message}");
    }
}

        
        
        
        
        
    }
}
