namespace Passion_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createtables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Animes",
                c => new
                    {
                        AnimeID = c.Int(nullable: false, identity: true),
                        AnimeName = c.String(),
                        ReleaseDate = c.DateTime(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.AnimeID);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        GenreID = c.Int(nullable: false, identity: true),
                        GenreName = c.String(),
                    })
                .PrimaryKey(t => t.GenreID);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        MemberID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.MemberID);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        ReviewID = c.Int(nullable: false, identity: true),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(),
                        ReviewDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reviews");
            DropTable("dbo.Members");
            DropTable("dbo.Genres");
            DropTable("dbo.Animes");
        }
    }
}
