namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateExamPeriod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbl_ExamPeriod", "MaxQuantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbl_ExamPeriod", "MaxQuantity");
        }
    }
}
