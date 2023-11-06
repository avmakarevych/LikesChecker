using Newtonsoft.Json;

namespace LikesChecker.Utilities;

public class SerializationHelper
{
    public static void SerializeAndWriteToFile(string filePath, object data)
    {
        var json = JsonConvert.SerializeObject(data);
        File.WriteAllText(filePath, json);
    }
}