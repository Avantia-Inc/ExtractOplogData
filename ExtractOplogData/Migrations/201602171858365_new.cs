namespace ExtractOplogData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _new : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OplogItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MongoId = c.String(),
                        AnswerData = c.String(),
                        LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OplogItems");
        }
    }
}
