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
    public class BuildController : Controller
    {
        private GymBuildrContext db = new GymBuildrContext();

        public ActionResult List()
        {

            List<Build> mybuilds = db.Builds.SqlQuery("Select * from Builds").ToList();
            //List<List<Item>> myitems = db.Items.SqlQuery("Select Items.* from Items inner join ItemBuilds ON Items.ItemID = ItemBuilds.Item_ItemID").ToList();
            Dictionary<Build, List<Item>> myitems = new Dictionary<Build, List<Item>>();
            //make a dictionary of the builds and the list of the associated items
            foreach (var build in mybuilds)
            {
                //make a list of all the items in each build
                List<Item> itemlist = db.Items.SqlQuery("Select Items.* from Items inner join ItemBuilds ON Items.ItemID = ItemBuilds.Item_ItemID where Build_BuildID = " + build.BuildID).ToList();
                //make a dictionary with the given build and the associated list of items as the key/value
                myitems.Add(build, itemlist);
            }
            ListBuilds listBuilds = new ListBuilds();
           
            listBuilds.itembuilds = myitems;
            return View(listBuilds);
        }
        public ActionResult New(string? buildname)
        {
            //error handling for accidental navigation to add page
            if (buildname == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //try catch to add the new build to the builds table
            try
            {
                string query = "INSERT INTO Builds (BuildName) VALUES (@name);";

                //set the params
                SqlParameter[] sqlparams = new SqlParameter[1];
                sqlparams[0] = new SqlParameter("@name", buildname);
                //execute the command
                db.Database.ExecuteSqlCommand(query, sqlparams);
                //grab the newly inserted record to redirect to the modify page
                Build newbuild = db.Builds.SqlQuery("select * from Builds ORDER BY BuildID desc;").FirstOrDefault();
                // redirect to relevant modifier page
                return RedirectToAction("Modify", new { id = newbuild.BuildID });
            }
            catch (Exception error)
            {
                Console.WriteLine("Error in creating Build with a title of BuildName = " + buildname);
                Console.WriteLine("Exception: " + error);
                return RedirectToAction("List");
            }

        }
        //make handling for updating a build name
        [HttpPost]
        public ActionResult Update(int? BuildID, string? BuildName)
        {
            //error handling for accidental navigation to update page
            if (BuildName == null || BuildID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //try catch to update the new name of the build to the builds table
            try
            {
                string query = "UPDATE Builds SET BuildName = @BuildName WHERE BuildID = @BuildID;";

                //set the params
                SqlParameter[] sqlparams = new SqlParameter[2];
                sqlparams[0] = new SqlParameter("@BuildName", BuildName);
                sqlparams[1] = new SqlParameter("@BuildID", BuildID);
                //execute the command
                db.Database.ExecuteSqlCommand(query, sqlparams);
      
                // redirect to the list page
                return RedirectToAction("List");
            }
            catch (Exception error)
            {
                Console.WriteLine("Error in updating Build with of ID of " + BuildID.ToString() + " and BuildName = " + BuildName);
                Console.WriteLine("Exception: " + error);
                return RedirectToAction("List");
            }

        }

        //make handling for removing a specified item from a build from the list view
        [HttpPost]
        public ActionResult RemoveItem(int? ItemID, int? BuildID)
        {
            //error handling for accidental navigation to delete page
            if (ItemID == null || BuildID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //run the delete in a try catch for error handling during delete request
            try
            {
                string query = "delete from ItemBuilds where (Build_BuildID = @id0 AND Item_ItemID = @id1);";

                //set the params
                SqlParameter[] sqlparams = new SqlParameter[2];
                sqlparams[0] = new SqlParameter("@id0", BuildID);
                sqlparams[1] = new SqlParameter("@id1", ItemID);
                //execute the command
                db.Database.ExecuteSqlCommand(query, sqlparams);
                //get the ID of the newly created query

            }
            catch(Exception error)
            {
                Console.WriteLine("Error in deleting Item with BuildID = " + BuildID.ToString() + "And ItemID = " + ItemID.ToString());
                Console.WriteLine("Exception: " + error);
            }
            return RedirectToAction("List");

        }

        //make handling for deleting an entire build, which includes deleting it from the bridging table
        //make handling for removing a specified item from a build from the list view
        [HttpPost]
        public ActionResult Delete(int? BuildID)
        {
            //error handling for accidental navigation to delete page
            if (BuildID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //run the delete in a try catch for error handling during delete request
            try
            {
                //first delete it from the bridging table
                string query = "delete from ItemBuilds where (Build_BuildID = @id0);";

                //set the params
                SqlParameter[] sqlparams = new SqlParameter[1];
                sqlparams[0] = new SqlParameter("@id0", BuildID);
                //execute the command
                db.Database.ExecuteSqlCommand(query, sqlparams);

                //now delete the build from the Builds table
                string query2 = "delete from Builds where (BuildID = @id0);";

                //set the params
                SqlParameter[] sqlparams2 = new SqlParameter[1];
                sqlparams2[0] = new SqlParameter("@id0", BuildID);
                //execute the command
                db.Database.ExecuteSqlCommand(query2, sqlparams2);

            }
            catch (Exception error)
            {
                Console.WriteLine("Error in deleting Build with BuildID = " + BuildID.ToString());
                Console.WriteLine("Exception: " + error);
            }
            return RedirectToAction("List");

        }

        public ActionResult Modify(int? id)
        {
            //error handling for accidental navigation to delete page
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Item> myitems = db.Items.SqlQuery("Select * from Items").ToList();
            List<ItemCategory> mycategories = db.ItemCategories.SqlQuery("Select * from ItemCategories").ToList();
            //get the build being referenced
            Build mybuild = db.Builds.SqlQuery("select * from Builds where Buildid=@BuildID", new SqlParameter("@BuildID", id)).FirstOrDefault();

            //get the list of items associated with the Build, to preselect the given items
            List<Item> itemlist = db.Items.SqlQuery("Select Items.* from Items inner join ItemBuilds ON Items.ItemID = ItemBuilds.Item_ItemID where Build_BuildID = @id0", new SqlParameter("@id0", id)).ToList();

            ModifyBuilds modifyBuilds = new ModifyBuilds();
            modifyBuilds.items = myitems;
            modifyBuilds.buildItems = itemlist;
            modifyBuilds.build = mybuild; 
            modifyBuilds.itemCategories = mycategories;
            return View(modifyBuilds);
        }
        //create the filtering get method
        [HttpGet]
        public ActionResult Modify(int? id, int price_range = 0, int[] categories = null)
        {

            //error handling for accidental navigation/invalid submission to the modify request page
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Build mybuild = db.Builds.SqlQuery("select * from Builds where Buildid=@BuildID", new SqlParameter("@BuildID", id)).FirstOrDefault();

            //get the list of items associated with the Build, to preselect the given items
            List<Item> itemlist = db.Items.SqlQuery("Select Items.* from Items inner join ItemBuilds ON Items.ItemID = ItemBuilds.Item_ItemID where Build_BuildID = @id0", new SqlParameter("@id0", id)).ToList();

            string query;
            string categoryquery;
            string price_modifier = " ItemPrice >= 0";
            List<Item> myitems;
            List<ItemCategory> selectedCategories;

            //handle the price range sql addition before categories, since it could apply to either
            if (price_range > 0)
            {
                if (price_range > 800)
                {
                    price_modifier = " ItemPrice >= 800";
                }
                else if (price_range == 50)
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
                for (int i = 1; i < categories.Length; i++)
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

            ModifyBuilds modifyBuilds = new ModifyBuilds();
            modifyBuilds.items = myitems;
            modifyBuilds.buildItems = itemlist;
            modifyBuilds.build = mybuild;
            modifyBuilds.itemCategories = mycategories;
            modifyBuilds.selectedItemCategories = selectedCategories;
            modifyBuilds.priceRange = price_range;
            return View(modifyBuilds);

        }
        //create the modifying POST logic for when a change is submitted
        [HttpPost]
        public ActionResult Modify(int? id, int[] add = null, int[] delete = null)
        {
            //error handling for accidental navigation to delete page
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //if delete requests have been sent, handle them first
            if(delete != null)
            {
                foreach(int deleteID in delete)
                {
                    try
                    {
                        string query = "delete from ItemBuilds where (Build_BuildID = @id0 AND Item_ItemID = @id1);";

                        //set the params
                        SqlParameter[] sqlparams = new SqlParameter[2];
                        sqlparams[0] = new SqlParameter("@id0", id);
                        sqlparams[1] = new SqlParameter("@id1", deleteID);
                        //execute the command
                        db.Database.ExecuteSqlCommand(query, sqlparams);
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine("Error in deleting Item with BuildID = " + id.ToString() + "And ItemID = " + deleteID.ToString());
                        Console.WriteLine("Exception: " + error);
                    }

                }
            }
            //do the same as above, but with insert into for all the insert add commands
            if (add != null)
            {
                foreach (int addID in add)
                {
                    try
                    {
                        string query = "INSERT INTO ItemBuilds (Build_BuildID, Item_ItemID) VALUES (@id0, @id1);";

                        //set the params
                        SqlParameter[] sqlparams = new SqlParameter[2];
                        sqlparams[0] = new SqlParameter("@id0", id);
                        sqlparams[1] = new SqlParameter("@id1", addID);
                        //execute the command
                        db.Database.ExecuteSqlCommand(query, sqlparams);
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine("Error in adding Item into Build with BuildID = " + id.ToString() + "And ItemID = " + addID.ToString());
                        Console.WriteLine("Exception: " + error);
                    }

                }
            }
            return RedirectToAction("List");

        }
    }
}