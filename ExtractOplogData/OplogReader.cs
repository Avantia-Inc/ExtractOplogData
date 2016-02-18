using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ExtractOplogData
{
    public class OplogReader
    {
        DataContext dataContext;

        public OplogReader()
        {
            dataContext = DataContext.Create();
        }

        public void Read(string path)
        {
            var sb = new StringBuilder();
            string line = null;
            using (var file = File.OpenRead(path))
            {
                using (var streamRdr = new StreamReader(file))
                {
                    while (!streamRdr.EndOfStream)
                    {
                        line = streamRdr.ReadLine();
                        sb.AppendLine(line);
                        if (line == "}")
                        {
                            ProcessRecord(sb.ToString());
                            sb.Clear();
                        }
                    }
                }
            }
        }

        void ProcessRecord(string data)
        {
            var jObj = JObject.Parse(data);
            if (jObj["ns"].Value<string>() == "data_entry.FormEntry" && jObj["op"].Value<string>() == "u" && ids.Contains(jObj["o2"]["_id"].Value<string>()))
            {
                var formEntry = jObj["o"];
                var id = formEntry["_id"].Value<string>();
                if (formEntry["Answers"].Values().Any() && formEntry["LastUpdateTimestamp"].HasValues)
                {
                    Console.WriteLine("Found update item for {0}", id);
                    var answers = formEntry["Answers"];
                    var lastUpdate = DateTime.FromFileTimeUtc(formEntry["LastUpdateTimestamp"]["$numberLong"].Value<long>());
                    // find existing record
                    var item = dataContext.OplogItems.SingleOrDefault(o => o.MongoId == id);
                    if (item == null)
                    {
                        // insert new item
                        dataContext.OplogItems.Add(new OplogItem {
                            MongoId = id,
                            AnswerData = GetJsonString(answers),
                            LastUpdated = lastUpdate
                        });
                        Console.WriteLine("Adding item for {0}", id);
                        dataContext.SaveChanges();
                    }
                    else
                    {
                        // update item
                        if (item.LastUpdated < lastUpdate)
                        {
                            item.AnswerData = GetJsonString(answers);
                            item.LastUpdated = lastUpdate;
                            dataContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            Console.WriteLine("Updating item for {0}", id);
                            dataContext.SaveChanges();
                        }
                    }
                }
            }
        }

        string GetJsonString(JToken token)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var jw = new JsonTextWriter(sw))
                {
                    token.WriteTo(jw);
                }
            }
            return sb.ToString();
        }

        string[] ids = new string[] { };
    }
}
