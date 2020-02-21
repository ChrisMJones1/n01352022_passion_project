using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GymBuildr.Data;
using GymBuildr.Models;
using GymBuildr.Models.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;

namespace GymBuildr.Controllers
{
    public class ItemsController : Controller
    {
        private GymBuildrContext db = new GymBuildrContext();

        public ActionResult List()
        {
            
            List<Item> myitems = db.Items.SqlQuery("Select * from Items").ToList();
            List<ItemCategory> mycategories = db.ItemCategories.SqlQuery("Select * from ItemCategories").ToList();
            ListItems listItems = new ListItems();
            listItems.items = myitems;
            listItems.itemCategories = mycategories;
            return View(listItems);
        }
        [HttpGet]
        public ActionResult List(int price_range = 0, int[] categories = null)
        {
            string query;
            string categoryquery;
            string price_modifier = " ItemPrice >= 0";
            List<Item> myitems;
            List<ItemCategory> selectedCategories;

            //handle the price range sql addition before categories, since it could apply to either
            if(price_range > 0)
            {
                if(price_range > 800)
                {
                    price_modifier = " ItemPrice >= 800";
                } 
                else if(price_range == 50) 
                {
                    price_modifier = " ItemPrice <= 50";
                }
                else
                {
                    price_modifier = " (ItemPrice >= " + (price_range / 2).ToString() + "AND ItemPrice <= " + price_range.ToString() + ")";
                }
            }

            if (categories == null)
            {
                query = "Select * from Items where" + price_modifier;
                categoryquery = "select * from ItemCategories where 0 = 1";
                myitems = db.Items.SqlQuery(query).ToList();
                selectedCategories = db.ItemCategories.SqlQuery(categoryquery).ToList();
            }
            else
            {
                //form query that will only select the entries that have the chosen categories, then group by the items
                //and count the number of categories for each item, and use the having clause to only return the entries
                //with the same count of categories as given selected categories
                query = "select Items.*, COUNT(ItemCategoryItems.ItemCategory_ItemCategoryID) as category_count FROM Items inner join ItemCategoryItems ON Items.ItemID = ItemCategoryItems.Item_ItemID WHERE ItemCategoryItems.ItemCategory_ItemCategoryID = @id0 AND" + price_modifier;

                //create a query to return the selected categories as well for tracking
                categoryquery = "select * from ItemCategories where ItemCategoryID = @id0";

                //run a for loop to add any extra categories if more than one category was selected
                for(int i = 1; i < categories.Length; i++)
                {
                    query += " OR ItemCategoryItems.ItemCategory_ItemCategoryID = @id" + i.ToString();
                    categoryquery += " OR ItemCategoryID = @id" + i.ToString();
                }

                //add on the group by and having clause after the where clause. All Items table columns must be listed
                //in the group by clause due to T-SQL's inability to disable the full_group_by requirement unlike mySQL

                query += " GROUP BY Items.ItemID, Items.ItemName, Items.ItemImagePath, Items.ItemPrice, Items.ItemURL, Items.ItemBrand HAVING COUNT(ItemCategoryItems.ItemCategory_ItemCategoryID) = @count; ";

                //turn the query into an SQL statement and add the parameters
                //the number of parameters needed is equal to the number of categories selected plus 1 for the count
                SqlParameter[] sqlparams = new SqlParameter[categories.Length + 1];

                //also create sqlparams for the category query
                SqlParameter[] querysqlparams = new SqlParameter[categories.Length];

                for (int i = 0; i < categories.Length; i++)
                {
                    sqlparams[i] = new SqlParameter("@id" + i.ToString(), categories[i]);
                    querysqlparams[i] = new SqlParameter("@id" + i.ToString(), categories[i]);
                }

                //add in the count parameter
                sqlparams[sqlparams.Length - 1] = new SqlParameter("@count", categories.Length);
                myitems = db.Items.SqlQuery(query, sqlparams).ToList();
                selectedCategories = db.ItemCategories.SqlQuery(categoryquery, querysqlparams).ToList();

            }
            List<ItemCategory> mycategories = db.ItemCategories.SqlQuery("Select * from ItemCategories").ToList();

            ListItems listItems = new ListItems();
            listItems.items = myitems;
            listItems.itemCategories = mycategories;
            listItems.selectedItemCategories = selectedCategories;
            listItems.priceRange = price_range;
            return View(listItems);

        }
    }
}
