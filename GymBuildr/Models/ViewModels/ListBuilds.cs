using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GymBuildr.Models.ViewModels
{
    public class ListBuilds
    {
        public Dictionary<Build, List<Item>> itembuilds { get; set; }
        //public List<Item> items { get; set; }

    }
}