using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymBuildr.Models
{
    public class Build
    {
        [Key]
        public int BuildID { get; set; }
        public string BuildName { get; set; }
        public ICollection<Item> Items { get; set; }


    }
}