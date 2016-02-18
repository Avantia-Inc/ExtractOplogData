using System;

namespace ExtractOplogData
{
    public class OplogItem
    {
        public int Id { get; set; }
        public string MongoId { get; set; }
        public string AnswerData { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
