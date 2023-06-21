namespace Passion_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reviews", "MemberID", "dbo.Members");
            DropIndex("dbo.Reviews", new[] { "MemberID" });
            AddColumn("dbo.Reviews", "UserID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Reviews", "UserID");
            AddForeignKey("dbo.Reviews", "UserID", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Reviews", "MemberID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reviews", "MemberID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Reviews", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Reviews", new[] { "UserID" });
            DropColumn("dbo.Reviews", "UserID");
            CreateIndex("dbo.Reviews", "MemberID");
            AddForeignKey("dbo.Reviews", "MemberID", "dbo.Members", "MemberID", cascadeDelete: true);
        }
    }
}
