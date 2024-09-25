namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateExamPeriod2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tbl_ExamPeriod", "VideoCourseId", c => c.Int());
            AlterColumn("dbo.tbl_ExamPeriod", "ExtensionPeriod", c => c.Int());
            AlterColumn("dbo.tbl_ExamPeriod", "MaxQuantity", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tbl_ExamPeriod", "MaxQuantity", c => c.Int(nullable: false));
            AlterColumn("dbo.tbl_ExamPeriod", "ExtensionPeriod", c => c.Int(nullable: false));
            AlterColumn("dbo.tbl_ExamPeriod", "VideoCourseId", c => c.Int(nullable: false));
        }
    }
}
