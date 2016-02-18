using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExtractOplogData
{
    public class UpdateRelatedAppAnswers
    {
        MongoClient client;

        public UpdateRelatedAppAnswers()
        {
            client = new MongoClient();
        }

        public void Run()
        {
            var formEntries = client.GetDatabase("data_entry").GetCollection<BsonDocument>("FormEntry");
            // loop through each id
            Parallel.ForEach(ids, id => {
                Console.WriteLine("Start processing {0}", id);

                // get app
                var mainApp = formEntries.AsQueryable().Single(f => f["_id"] == id);
                // get all related apps
                var relatedAppIds = mainApp["SchoolData"].AsBsonArray.Where(s => !s["FormEntryId"].IsBsonNull && s["FormEntryId"] != id).Select(s => s["FormEntryId"].AsString);
                foreach (var relatedAppId in relatedAppIds)
                {
                    // get related app
                    var relatedApp = formEntries.AsQueryable().SingleOrDefault(f => f["_id"] == relatedAppId);
                    if (relatedApp != null)
                    {
                        Console.WriteLine("Found related app {0} for {1}", relatedAppId, id);
                        if (relatedApp["Answers"].IsBsonNull || !relatedApp["Answers"].IsBsonArray || relatedApp["Answers"].AsBsonArray.Count == 0)
                        {
                            Console.WriteLine("Adding answers from {0} to related app {1}", id, relatedAppId);
                            relatedApp["Answers"] = mainApp["Answers"];
                            formEntries.ReplaceOne(f => f["_id"] == relatedAppId, relatedApp);
                        }
                    }
                }

                Console.WriteLine("Complete processing {0}", id);
            });
        }

        string[] ids = new string[] { };
    }
}
