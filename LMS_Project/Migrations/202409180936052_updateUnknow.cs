namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUnknow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbl_VideoCourse", "ExtensionPeriod", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbl_VideoCourse", "ExtensionPeriod");
        }
    }
}
