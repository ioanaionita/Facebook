namespace Facebook.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_id : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "UserId");
        }
    }
}
