using System;

namespace Avto;

public class Orders
{
    // Свойство для хранения уникального идентификатора заказа
    public int orderID { get; set; }

    // Свойство для хранения ID клиента
    public int customer_ID { get; set; }

    // Свойство для хранения названия клиента
    public string nameC { get; set; }

    // Свойство для хранения ID услуги
    public int service_ID { get; set; }

    // Свойство для хранения названия услуги
    public string name { get; set; }

    // Свойство для хранения даты заказа
    public string date { get; set; }

    // Свойство для хранения общей стоимости заказа
    public double total_price { get; set; }
}
