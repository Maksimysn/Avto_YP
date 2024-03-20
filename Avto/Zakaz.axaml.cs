using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Avto;

public partial class Zakaz : Window
{
    private Orders _selectedOrder; // Заменяем Services на Order
    private MySqlConnection _connection;
    private List<Orders> _orders; // Заменяем List<Services> на List<Order>
    private string _connString = "server=localhost;database=YP_O;port=3306;User Id=sammy;password=Andr%123; Charset=utf8";
    public Zakaz()
    {
        InitializeComponent();
        ShowTable();
        _connection = new MySqlConnection(_connString);
        OrdersGrid.SelectionChanged += OrdersGrid_SelectionChanged;
    }
    private void OrdersGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (OrdersGrid.SelectedItem is Orders selectedOrder) // Заменяем ServicesGrid на OrdersGrid и Services на Order
        {
            _selectedOrder = selectedOrder; // Заменяем Services на Order
        }
    }
    
    public void ShowTable()
    {
        string sql = "SELECT Orders.orderID, Orders.customer_ID, Customers.nameC, Orders.service_ID, Services.name, Orders.date, Orders.total_price FROM Orders INNER JOIN Customers ON Orders.customer_ID = Customers.customerID INNER JOIN Services ON Orders.service_ID = Services.serviceID";

        _orders = new List<Orders>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var currentOrder = new Orders
                    {
                        orderID = reader.GetInt32("orderID"),
                        customer_ID = reader.GetInt32("customer_ID"),
                        nameC = reader.GetString("nameC"),
                        service_ID = reader.GetInt32("service_ID"),
                        name = reader.GetString("name"), // Используем name вместо Sname
                        date = reader.GetDateTime("date").ToShortDateString(), // Форматирование даты
                        total_price = reader.GetDouble("total_price")
                    };

                    _orders.Add(currentOrder);
                }
            }
        }

        OrdersGrid.ItemsSource = _orders;
    }
    
    
    private void AddOrder_Click(object? sender, RoutedEventArgs e)
    {
        var addForm = new D_Zakaz(this);
        addForm.Show();
    }

    private void DeleteOrder_Click(object? sender, RoutedEventArgs e)
    {
        
    }

    private void EditOrder_Click(object? sender, RoutedEventArgs e)
    {
        
    }
}