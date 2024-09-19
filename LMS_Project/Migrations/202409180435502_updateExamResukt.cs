namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateExamResukt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbl_ExamResult", "ExamPeriodId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbl_ExamResult", "ExamPeriodId");
        }
    }
}
