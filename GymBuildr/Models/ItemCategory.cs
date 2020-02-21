using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymBuildr.Models
{
    public class ItemCategory
    {
        [Key]
        public int ItemCategoryID { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}