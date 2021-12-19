using System.IO;

namespace WinBot.Util
{
    public static class ResourceManager
    {
        public static string GetResourcePath(string name, ResourceType type = ResourceType.MiscData)
        {
            // Get a file name
            string fileName = name;
            if(type == ResourceType.Config || type == ResourceType.JsonData)
                fileName += ".json";
            else if(type == ResourceType.Image)
                fileName += ".png";
            // MiscData is treated as is

            // Get the full path
            string path = fileName;
            if(type == ResourceType.JsonData || type == ResourceType.MiscData)
                path = "Data/" + path;
            else if(type == ResourceType.Image)
                path = "Images/" + path;

            return path;
        }

        public static bool ResourceExists(string name, ResourceType type = ResourceType.MiscData)
        {
            return File.Exists(GetResourcePath(name, type));
        }
    }

    public enum ResourceType
    {
        JsonData, MiscData, Config, Image
    }
}