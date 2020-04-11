using Neo4j.Driver;
namespace gatherme_suggestion_ms.Settings
{
    public interface IConnectionSettings
    {
         string Uri {get;}

         IAuthToken AuthToken {get;}
    }
}