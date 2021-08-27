using Newtonsoft.Json;
public static class Extension
{
    public static T ToObject<T>(this string target) => JsonConvert.DeserializeObject<T>(target);
    public static string ToJson(this object DataObject, Formatting formatting = Formatting.None) => JsonConvert.SerializeObject(DataObject, formatting);

}
