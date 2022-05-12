namespace WinBot.Helpers
{
    public static class JsonHelper
    {
        private static JsonSerializerOptions serializeOptions = new JsonSerializerOptions() {
            WriteIndented = true,
            Converters = {
               new JsonStringEnumConverter()
            }
        };

        /// <summary>
        /// Load data from a JSON file
        /// </summary>
        /// <param name="fileName">File to load</param>
        /// <typeparam name="T">Type of data</typeparam>
        /// <returns>Deserialized data</returns>
        public static T LoadFromJson<T>(string fileName)
        {
            // Ensure the JSON file exists
            if(!File.Exists(fileName))
                throw new Exception("The specified JSON file does not exist!");

            // Deserialize
            string json = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<T>(json, serializeOptions);
        }

        /// <summary>
        /// Write data to a JSON file
        /// </summary>
        /// <param name="fileName">File to load</param>
        /// <param name="data">Data to serialize/write to the JSON file</param>
        public static void WriteToJson(string fileName, object data)
        {
            // Serialize and write
            string json = JsonSerializer.Serialize(data, serializeOptions);
            File.WriteAllText(fileName, json);
        }
    }
}