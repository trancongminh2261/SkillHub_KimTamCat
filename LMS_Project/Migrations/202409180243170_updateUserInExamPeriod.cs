namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUserInExamPeriod : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbl_UserInExamPeriod",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ExamPeriodId = c.Int(nullable: false),
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
            DropTable("dbo.tbl_UserInExamPeriod");
        }
    }
}
