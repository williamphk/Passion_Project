namespace Passion_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class review1m : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reviews", "AnimeID", c => c.Int(nullable: false));
            AddColumn("dbo.Reviews", "MemberID", c => c.Int(nullable: false));
            CreateIndex("dbo.Reviews", "AnimeID");
            CreateIndex("dbo.Reviews", "MemberID");
            AddForeignKey("dbo.Reviews", "AnimeID", "dbo.Animes", "AnimeID", cascadeDelete: true);
            AddForeignKey("dbo.Reviews", "MemberID", "dbo.Members", "MemberID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "MemberID", "dbo.Members");
            DropForeignKey("dbo.Reviews", "AnimeID", "dbo.Animes");
            DropIndex("dbo.Reviews", new[] { "MemberID" });
            DropIndex("dbo.Reviews", new[] { "AnimeID" });
            DropColumn("dbo.Reviews", "MemberID");
            DropColumn("dbo.Reviews", "AnimeID");
        }
    }
}
