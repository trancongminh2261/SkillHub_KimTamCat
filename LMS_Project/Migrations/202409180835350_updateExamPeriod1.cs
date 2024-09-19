namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateExamPeriod1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tbl_ExamPeriod", "PassingScore");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tbl_ExamPeriod", "PassingScore", c => c.Double(nullable: false));
        }
    }
}
