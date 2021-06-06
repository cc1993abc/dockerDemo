﻿using System;

namespace Play.Catalog.Service.Entities
{
    public class Item
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string  Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.UtcNow;
    }
}