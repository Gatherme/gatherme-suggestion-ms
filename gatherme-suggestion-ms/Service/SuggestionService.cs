using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using gatherme_suggestion_ms.Serializer;
using System.Collections;
namespace gatherme_suggestion_ms.Service
{
    public class SuggestionService : ISuggestionService
    {
        private Neo4JClient client;
        private ArrayList mySuggestions;
        private ArrayList mySuggestionInfo;
        /*Contructor*/
        public SuggestionService(Neo4JClient client)
        {
            this.client = client;
            this.mySuggestions = new ArrayList();
            this.mySuggestionInfo = new ArrayList();
        }
        /*DB operations*/
        private async Task CreateSuggestion(IList<Suggestion> suggestions)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $suggestions AS suggestion")
            .AppendLine("CREATE(:Suggestion{id: suggestion.id, isActive: suggestion.isActive})")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            try
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "suggestions", ParameterSerializer.ToDictionary(suggestions) } });
            }
            finally { await session.CloseAsync(); }
        }
        private async Task CreateRelationshipSuggestion(IList<SuggestionInfo> suggestionMetadata)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $suggestionMetadata AS suggestionMetadata")
            // find Suggestion
            .AppendLine("MATCH (s:Suggestion { id: suggestionMetadata.suggestion.id})")
            // suggested user
            .AppendLine("WITH suggestionMetadata, s")
            .AppendLine("UNWIND suggestionMetadata.suggestedUser AS suggestedUser")
            .AppendLine("MATCH (u:User { id: suggestedUser.id})")
            .AppendLine("MERGE (s)-[r:SUGGEST]->(u)")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "suggestionMetadata", ParameterSerializer.ToDictionary(suggestionMetadata) } });
            }
            finally { await session.CloseAsync(); }
        }
        private async Task<List<User>> searchUserToSuggest(IList<User> users)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $users AS user")
            .AppendLine("MATCH (mu:User { id: user.id})")
            .AppendLine("MATCH (mu)--(l:Like)")
            .AppendLine("MATCH (l)--(u:User)")
            .AppendLine("WHERE  NOT ((mu)-->(u)) AND NOT(mu.id = u.id)")
            .AppendLine("AND NOT((mu)-[:GET]-(:Suggestion)-[:SUGGEST]-(u))")
            .AppendLine("RETURN u.id, u.name")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<User> listUsers = new List<User>();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "users", ParameterSerializer.ToDictionary(users) } });
                while (await reader.FetchAsync())
                {
                    int count = 0;
                    string id = "";
                    // Each current read in buffer can be reached via Current
                    foreach (var item in reader.Current.Values)
                    {
                        if ((count % 2 != 0))
                        {
                            User aux = new User
                            {
                                Id = id,
                                Name = item.Value.ToString()
                            };
                            listUsers.Add(aux);
                        }
                        else
                        {
                            id = item.Value.ToString();
                        }
                        System.Console.WriteLine(item.Value);
                        count++;
                    }

                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return listUsers;
        }
        public async Task<IList<SuggestionInfo>> CreateSuggestedRelation(IList<User> users)
        {
            List<User> myList = await searchUserToSuggest(users);
            if (myList.Count == 0)
            {
                //
            }
            else
            {
                foreach (var item in myList)
                {
                    Suggestion aux = new Suggestion
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        IsActive = true
                    };
                    SuggestionInfo metaData = new SuggestionInfo
                    {
                        Suggestion = aux,
                        SuggestedUser = item
                    };
                    addSuggestion(aux);
                    addMetadata(metaData);
                }
                await CreateSuggestion(Suggestions);
                await CreateRelationshipSuggestion(SuggestionInfos);
            }
            return SuggestionInfos;
        }
        public async Task<List<Suggestion>> getAllSuggestions()
        {
            string cypher = new StringBuilder()
            .AppendLine("MATCH (n:Suggestion)")
            .AppendLine("RETURN n.id, n.isActive")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<Suggestion> suggestionList = new List<Suggestion>();
            try
            {
                var reader = await session.RunAsync(cypher);
                while (await reader.FetchAsync())
                {
                    int count = 0;
                    string id = "";
                    // Each current read in buffer can be reached via Current
                    foreach (var item in reader.Current.Values)
                    {
                        if ((count % 2 != 0))
                        {
                            Suggestion aux = new Suggestion
                            {
                                Id = id,
                                IsActive = bool.Parse(item.Value.ToString())
                            };
                            suggestionList.Add(aux);
                        }
                        else
                        {
                            id = item.Value.ToString();
                        }
                        System.Console.WriteLine(item.Value);
                        count++;
                    }
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
            }
            finally
            {
                await session.CloseAsync();
            }
            return suggestionList;
        }
        public async Task ChangeIsActive(IList<Suggestion> suggestions)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $suggestions AS suggestion")
            .AppendLine("MATCH (s:Suggestion {id: suggestion.id})")
            .AppendLine("SET s.isActive = false")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "suggestions", ParameterSerializer.ToDictionary(suggestions) } });
            }
            finally { await session.CloseAsync(); }
        }


        /*Interfaz*/
        public void addSuggestion(Suggestion suggestion)
        {
            this.mySuggestions.Add(suggestion);
        }
        public void addMetadata(SuggestionInfo suggestionInfo)
        {
            this.mySuggestionInfo.Add(suggestionInfo);
        }



        public IList<Suggestion> Suggestions
        {
            get
            {
                return mySuggestions.ToArray(typeof(Suggestion)) as Suggestion[];
            }
        }
        public IList<SuggestionInfo> SuggestionInfos
        {
            get
            {
                return mySuggestionInfo.ToArray(typeof(SuggestionInfo)) as SuggestionInfo[];
            }
        }
    }
}