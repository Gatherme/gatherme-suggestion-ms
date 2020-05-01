using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using gatherme_suggestion_ms.Serializer;
using System.Collections;
namespace gatherme_suggestion_ms.Service
{
    public class CategoryService : ICategoryService
    {
        private Neo4JClient client;
        private ArrayList myCategories;

        /*Constructor*/
        public CategoryService(Neo4JClient client)
        {
            this.client = client;
            this.myCategories = new ArrayList();
        }

        /*DB operations*/

        public async Task<string> CreateCategory(IList<Category> categories)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $categories AS category")
            .AppendLine("CREATE(c:Category{name: category.name})")
            .AppendLine("RETURN c.name")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            string ans = "";
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "categories", ParameterSerializer.ToDictionary(categories) } });
                while (await reader.FetchAsync())
                {
                    foreach (var item in reader.Current.Values)
                    {
                        ans += item.Value.ToString() + " ";
                    }
                }
            }
            finally { await session.CloseAsync(); }
            return ans;
        }
        public async Task<List<Category>> GetAllCategories()
        {
            string cypher = new StringBuilder()
            .AppendLine("MATCH (n:Category)")
            .AppendLine("RETURN n.name")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<Category> categoryList = new List<Category>();
            try
            {
                var reader = await session.RunAsync(cypher);
                while (await reader.FetchAsync())
                {
                    // Each current read in buffer can be reached via Current
                    foreach (var item in reader.Current.Values)
                    {
                        Category aux = new Category
                        {
                            Name = item.Value.ToString()
                        };
                        System.Console.WriteLine(item.Value);
                        categoryList.Add(aux);
                    }

                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return categoryList;
        }
        public async Task<List<User>> GetUsers(IList<Category> categories)
        {
            string cypher = new StringBuilder()
              .AppendLine("UNWIND $categories AS category")
              .AppendLine("MATCH (c:Category { name: category.name })")
              .AppendLine("MATCH (c)<-[:IS]-(l:Like)")
              .AppendLine("WITH c,l")
              .AppendLine("MATCH (l)<-[:LIKE]-(u:User)")
              .AppendLine("RETURN u.id, u.name")
              .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<User> users = new List<User>();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "categories", ParameterSerializer.ToDictionary(categories) } });
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
                            users.Add(aux);
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
            return users;
        }
        public async Task<List<bool>> ExistCategory(IList<Category> categories)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $categories AS category")
            .AppendLine("OPTIONAL MATCH (c:Category{name: category.name})")
            .AppendLine("RETURN c.name")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<bool> boolList = new List<bool>();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "categories", ParameterSerializer.ToDictionary(categories) } });
                while (await reader.FetchAsync())
                {
                    foreach (var item in reader.Current.Values)
                    {
                        if (item.Value == null)
                        {
                            boolList.Add(false);
                        }
                        else
                        {
                            boolList.Add(true);
                        }
                    }
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return boolList;
        }
        public async Task<List<Like>> LikeByCategory(IList<Category> categories)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $categories AS category")
            .AppendLine("MATCH (c:Category{name: category.name})")
            .AppendLine("MATCH (c)<-[:IS]-(l:Like)")
            .AppendLine("RETURN l.name")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<Like> myLikes = new List<Like>();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "categories", ParameterSerializer.ToDictionary(categories) } });
                System.Console.WriteLine("****Likes by category:");
                while (await reader.FetchAsync())
                {
                    foreach (var item in reader.Current.Values)
                    {
                        Like auxLike = new Like
                        {
                            Name = item.Value.ToString()
                        };
                        System.Console.WriteLine(item.Value.ToString());
                        myLikes.Add(auxLike);
                    }
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return myLikes;
        }

        /*Interfaz*/
        public void addCategory(Category category)
        {
            this.myCategories.Add(category);
        }
        public IList<Category> Categories
        {
            get
            {
                return myCategories.ToArray(typeof(Category)) as Category[];
            }
        }
    }
}