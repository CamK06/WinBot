namespace WinBot.Helpers
{
    // This whole class isn't strictly required, but it makes other parts of code slightly cleaner
    public static class Resources
    {
        /// <summary>
        /// Get the path of a resource
        /// </summary>
        /// <param name="fileName">File name of resource</param>
        /// <param name="type">Type of resource</param>
        /// <returns>The file path of the resource</returns>
        public static string GetResourcePath(string fileName, ResourceType type = ResourceType.Resource) 
        {
            if(type == ResourceType.Data)
                return $"Data/{fileName}";
            else if(type == ResourceType.Resource)
                return $"Resource/{fileName}";
            return "";
        }
        
        /// <summary>
        /// Get whether or not a resource file exists
        /// </summary>
        /// <param name="fileName">File name of resource</param>
        /// <param name="type">Type of resource</param>
        /// <returns></returns>
        public static bool ResourceExists(string fileName, ResourceType type = ResourceType.Resource)
        {
            return File.Exists(GetResourcePath(fileName, type));
        }
    }

    public enum ResourceType
    {
        Data, Resource
    }
}