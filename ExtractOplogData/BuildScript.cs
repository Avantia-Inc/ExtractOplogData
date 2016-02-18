using System.IO;
using System.Text.RegularExpressions;

namespace ExtractOplogData
{
    public class BuildScript
    {
        DataContext dataContext;

        public BuildScript()
        {
            dataContext = DataContext.Create();
        }

        public void Generate(string outputPath)
        {
            using (var file = File.Create(outputPath))
            {
                using (var sw = new StreamWriter(file))
                {
                    sw.WriteLine("use data_entry");
                    sw.WriteLine("db.FormEntry.bulkWrite([");
                    string comma = string.Empty;
                    foreach (var item in dataContext.OplogItems)
                    {
                        sw.WriteLine("\t{0}{{ updateOne :", comma);
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tfilter: {{ _id: '{0}' }},", item.MongoId);
                        sw.WriteLine("\t\t\tupdate: {{ $set: {{ Answers: {0} }} }},", FixNumberLong(item.AnswerData));
                        sw.WriteLine("\t\t\tupsert: false");
                        sw.WriteLine("\t\t}\n\t}");
                        comma = ",";
                    }
                    sw.WriteLine("]);");
                }
            }
        }

        string FixNumberLong(string data)
        {
            var reg = new Regex("\\{\"\\$numberLong\":\"(\\d+)\"\\}");
            return reg.Replace(data, "NumberLong($1)");
        }
    }
}
