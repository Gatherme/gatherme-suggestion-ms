using System.Collections.Generic;
using gatherme_suggestion_ms.Serializer.Converters;
using Newtonsoft.Json;
namespace gatherme_suggestion_ms.Serializer
{
    //https://github.com/bytefish/Neo4JSample/blob/master/Neo4JSample/Neo4JSample/Serializer/ParameterSerializer.cs
    public class ParameterSerializer
    {
        public static IList<Dictionary<string, object>> ToDictionary<TSourceType>(IList<TSourceType> source)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(source, settings);

            return JsonConvert.DeserializeObject<IList<Dictionary<string, object>>>(json, new CustomDictionaryConverter());
        }
    }
}