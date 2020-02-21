using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymBuildr.Models
{
    public class Item
    {
        [Key]
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        public string ItemImagePath { get; set; }
        public string ItemURL { get; set; }
        public string ItemBrand { get; set; }
        //category many to many
        public ICollection<ItemCategory> ItemCategories { get; set; }
        public ICollection<Build> Builds { get; set; }

    }
}