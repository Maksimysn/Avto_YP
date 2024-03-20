namespace Avto;

public class Services
{
        // Свойство для хранения уникального идентификатора услуги
        public int serviceID { get; set; }

        // Свойство для хранения названия услуги
        public string name { get; set; }

        // Свойство для хранения описания услуги
        public string description { get; set; }

        // Свойство для хранения цены услуги
        public int price { get; set; }

        // Свойство для хранения фото услуги в виде массива байт
            public byte[] photo { get; set; }

        // Свойство для хранения скидки на услугу
        public int discount { get; set; }
        
        public double discountedPrice => price * (1 - discount / 100.0); // новое свойство для цены со скидкой
        
}
