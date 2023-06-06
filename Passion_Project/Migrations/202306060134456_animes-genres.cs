namespace Passion_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class animesgenres : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GenreAnimes",
                c => new
                    {
                        Genre_GenreID = c.Int(nullable: false),
                        Anime_AnimeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Genre_GenreID, t.Anime_AnimeID })
                .ForeignKey("dbo.Genres", t => t.Genre_GenreID, cascadeDelete: true)
                .ForeignKey("dbo.Animes", t => t.Anime_AnimeID, cascadeDelete: true)
                .Index(t => t.Genre_GenreID)
                .Index(t => t.Anime_AnimeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GenreAnimes", "Anime_AnimeID", "dbo.Animes");
            DropForeignKey("dbo.GenreAnimes", "Genre_GenreID", "dbo.Genres");
            DropIndex("dbo.GenreAnimes", new[] { "Anime_AnimeID" });
            DropIndex("dbo.GenreAnimes", new[] { "Genre_GenreID" });
            DropTable("dbo.GenreAnimes");
        }
    }
}
