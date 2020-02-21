using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GymBuildr.Data
{
    public class GymBuildrContext : DbContext
    {
        public GymBuildrContext() : base("name=GymBuildrContext")
        {
        }
        public System.Data.Entity.DbSet<GymBuildr.Models.Item> Items { get; set; }
        public System.Data.Entity.DbSet<GymBuildr.Models.Build> Builds { get; set; }
        public System.Data.Entity.DbSet<GymBuildr.Models.ItemCategory> ItemCategories { get; set; }
    }
}