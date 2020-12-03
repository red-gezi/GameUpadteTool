using Newtonsoft.Json;
public static class Extension
{
    public static string ToJson(this object DataObject)
    {
        return JsonConvert.SerializeObject(DataObject);
    }
    public static T ToObject<T>(this string target)
    {
        return JsonConvert.DeserializeObject<T>(target);
    }
}
