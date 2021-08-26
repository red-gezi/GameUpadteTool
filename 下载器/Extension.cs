using Newtonsoft.Json;
public static class Extension
{
    public static string ToJson(this object DataObject, Formatting formatting= Formatting.None)
    {
        return JsonConvert.SerializeObject(DataObject, formatting);
    }
    public static T ToObject<T>(this string target)
    {
        return JsonConvert.DeserializeObject<T>(target);
    }
}
