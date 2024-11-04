using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace CommBank.Services
{
    public class DatabaseSeeder
    {
        private readonly IMongoDatabase _database;

        public DatabaseSeeder(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task SeedDatabaseFromJsonFilesAsync(string folderPath)
        {
            // Get all JSON files in the folder
            var files = Directory.GetFiles(folderPath, "*.json");

            foreach (var file in files)
            {
                // Use the filename (without extension) as the collection name
                var collectionName = Path.GetFileNameWithoutExtension(file);
                
                // Read the JSON file content
                var jsonData = await File.ReadAllTextAsync(file);

                // Parse the entire JSON data as BsonDocument
                // Assuming the JSON data is an array of objects
                var documents = BsonSerializer.Deserialize<List<BsonDocument>>(jsonData);

                if (documents != null)
                {
                    // Get or create the collection with the given name
                    var collection = _database.GetCollection<BsonDocument>(collectionName);

                    // Insert the documents into the collection
                    await collection.InsertManyAsync(documents);
                }
            }
        }

    }
}
