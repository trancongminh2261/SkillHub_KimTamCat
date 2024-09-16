namespace LMS_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bigUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbl_AntiDownVideo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VideoUploadId = c.String(),
                        Title = c.String(),
                        Minute = c.Int(nullable: false),
                        Thumbnail = c.String(),
                        VideoUrl = c.String(),
                        Enable = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tbl_VideoConfig",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LessonVideoId = c.Int(nullable: false),
                        StopInMinute = c.Int(nullable: false),
                        Type = c.String(),
                        Enable = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tbl_VideoConfigOption",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VideoConfigQuestionId = c.Int(nullable: false),
                        Content = c.String(),
                        IsCorrect = c.Boolean(nullable: false),
                        Enable = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tbl_VideoConfigQuestion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VideoConfigId = c.Int(nullable: false),
                        Content = c.String(),
                        Index = c.Int(nullable: false),
                        Enable = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tbl_VideoCourseAllow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VideoCourseId = c.Int(nullable: false),
                        ValueId = c.Int(),
                        Type = c.String(),
                        Enable = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.tbl_LessonVideo", "AntiDownVideoId", c => c.Int());
            AddColumn("dbo.tbl_UserInformation", "LastLoginDate", c => c.DateTime());
            AddColumn("dbo.tbl_VideoCourse", "IsPublic", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbl_VideoCourse", "IsPublic");
            DropColumn("dbo.tbl_UserInformation", "LastLoginDate");
            DropColumn("dbo.tbl_LessonVideo", "AntiDownVideoId");
            DropTable("dbo.tbl_VideoCourseAllow");
            DropTable("dbo.tbl_VideoConfigQuestion");
            DropTable("dbo.tbl_VideoConfigOption");
            DropTable("dbo.tbl_VideoConfig");
            DropTable("dbo.tbl_AntiDownVideo");
        }
    }
}
