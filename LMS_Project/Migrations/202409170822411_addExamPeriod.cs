namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addExamPeriod : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbl_ExamPeriod",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        VideoCourseId = c.Int(nullable: false),
                        ExamId = c.Int(nullable: false),
                        ExtensionPeriod = c.Int(nullable: false),
                        PassingScore = c.Double(nullable: false),
                        Description = c.String(),
                        Status = c.String(),
                        Enable = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tbl_ExamPeriod");
        }
    }
}
