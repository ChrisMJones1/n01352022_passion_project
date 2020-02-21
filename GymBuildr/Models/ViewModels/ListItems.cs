using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GymBuildr.Models.ViewModels
{
    public class ListItems
    {
        public List<Item> items { get; set; }
        public List<ItemCategory> itemCategories { get; set; }
        public List<ItemCategory> selectedItemCategories { get; set; }
        public int priceRange { get; set; }
    }
}