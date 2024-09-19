namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateVideoCourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbl_Certificate", "ExpirationDate", c => c.DateTime());
            AddColumn("dbo.tbl_Certificate", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbl_Certificate", "Status");
            DropColumn("dbo.tbl_Certificate", "ExpirationDate");
        }
    }
}
