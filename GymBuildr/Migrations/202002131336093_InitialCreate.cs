namespace GymBuildr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Builds",
                c => new
                    {
                        BuildID = c.Int(nullable: false, identity: true),
                        BuildName = c.String(),
                    })
                .PrimaryKey(t => t.BuildID);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemID = c.Int(nullable: false, identity: true),
                        ItemName = c.String(),
                        ItemPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemImagePath = c.String(),
                        ItemURL = c.String(),
                        ItemBrand = c.String(),
                    })
                .PrimaryKey(t => t.ItemID);
            
            CreateTable(
                "dbo.ItemCategories",
                c => new
                    {
                        ItemCategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                    })
                .PrimaryKey(t => t.ItemCategoryID);
            
            CreateTable(
                "dbo.ItemBuilds",
                c => new
                    {
                        Item_ItemID = c.Int(nullable: false),
                        Build_BuildID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Item_ItemID, t.Build_BuildID })
                .ForeignKey("dbo.Items", t => t.Item_ItemID, cascadeDelete: true)
                .ForeignKey("dbo.Builds", t => t.Build_BuildID, cascadeDelete: true)
                .Index(t => t.Item_ItemID)
                .Index(t => t.Build_BuildID);
            
            CreateTable(
                "dbo.ItemCategoryItems",
                c => new
                    {
                        ItemCategory_ItemCategoryID = c.Int(nullable: false),
                        Item_ItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ItemCategory_ItemCategoryID, t.Item_ItemID })
                .ForeignKey("dbo.ItemCategories", t => t.ItemCategory_ItemCategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.Item_ItemID, cascadeDelete: true)
                .Index(t => t.ItemCategory_ItemCategoryID)
                .Index(t => t.Item_ItemID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemCategoryItems", "Item_ItemID", "dbo.Items");
            DropForeignKey("dbo.ItemCategoryItems", "ItemCategory_ItemCategoryID", "dbo.ItemCategories");
            DropForeignKey("dbo.ItemBuilds", "Build_BuildID", "dbo.Builds");
            DropForeignKey("dbo.ItemBuilds", "Item_ItemID", "dbo.Items");
            DropIndex("dbo.ItemCategoryItems", new[] { "Item_ItemID" });
            DropIndex("dbo.ItemCategoryItems", new[] { "ItemCategory_ItemCategoryID" });
            DropIndex("dbo.ItemBuilds", new[] { "Build_BuildID" });
            DropIndex("dbo.ItemBuilds", new[] { "Item_ItemID" });
            DropTable("dbo.ItemCategoryItems");
            DropTable("dbo.ItemBuilds");
            DropTable("dbo.ItemCategories");
            DropTable("dbo.Items");
            DropTable("dbo.Builds");
        }
    }
}
