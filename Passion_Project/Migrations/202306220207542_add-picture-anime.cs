namespace Passion_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addpictureanime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Animes", "AnimeHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Animes", "PicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Animes", "PicExtension");
            DropColumn("dbo.Animes", "AnimeHasPic");
        }
    }
}
