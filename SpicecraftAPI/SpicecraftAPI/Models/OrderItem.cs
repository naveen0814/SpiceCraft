    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace SpicecraftAPI.Models
    {
        public class OrderItem
        {
            [Key]
            public int OrderItemId { get; set; }
            public int OrderId { get; set; }
            public int MenuId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }

            public Order Order { get; set; }
            public MenuItem MenuItem { get; set; }
        }
    }