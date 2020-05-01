using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using gatherme_suggestion_ms.Serializer;
using System.Collections;
namespace gatherme_suggestion_ms.Service
{
    public class ReportService : IReportService
    {
        private Neo4JClient client;
        private Report reAux {get; set;} //machete
        private ReportInfo info {get;set;}
        /*Contructor*/
        public ReportService(Neo4JClient client)
        {
            this.client = client;
        }
        /*DB operations*/
        public async Task<List<Report>> getAllReports()
        {

            string cypher = new StringBuilder()
            .AppendLine("MATCH (n:Report)")
            .AppendLine("RETURN n.id, n.commentary")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<Report> reportList = new List<Report>();
            try
            {
                var reader = await session.RunAsync(cypher);
                while (await reader.FetchAsync())
                {
                    int count = 0;
                    string aux = "";
                    foreach (var item in reader.Current.Values)
                    {
                        if (count == 1)
                        {
                            Report myreport = new Report
                            {
                                Id = aux,
                                Commentary = item.Value.ToString()
                            };
                            reportList.Add(myreport);
                        }
                        else
                        {
                            aux = item.Value.ToString();
                        }
                        count++;
                    }
                }

            }
            finally { await session.CloseAsync(); }
            return reportList;
        }
        //Crear relaciones
        private async Task<string> reportRelationship(IList<ReportInfo> reportInfos)
        {
            System.Console.WriteLine(reportInfos.Count);
            string cypher = new StringBuilder()
            //Etiqueta todo
            .AppendLine("UNWIND $reportInfos AS reportInfo")
            //Encontrar usuarios
            .AppendLine("MATCH (us:User { id: reportInfo.user.id})")
            .AppendLine("MATCH (usr:User { id: reportInfo.userReported.id})")
            .AppendLine("MATCH (r:Report { id: reportInfo.report.id})")
            //Relacion
            .AppendLine("WITH us, usr,r")
            .AppendLine("MERGE (us)-[re:DOES_REPORT]->(r)")
            .AppendLine("MERGE (r)-[rep:REPORTS]->(usr)")
            .AppendLine("RETURN us.name, type(re), r.id, type(rep), usr.name")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            string ans = "";
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "reportInfos", ParameterSerializer.ToDictionary(reportInfos) } });
                while (await reader.FetchAsync())
                {
                    foreach (var item in reader.Current.Values)
                    {
                        System.Console.WriteLine(item.Value.ToString());
                        ans+=item.Value.ToString() + " ";
                    }
                }
            }
            finally { await session.CloseAsync(); }
            return ans;
        }
        //Crear nodo
        private async Task newReportNodo(IList<Report> reports)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $reports AS report")
            .AppendLine("CREATE(:Report{id: report.id, commentary: report.commentary})")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "reports", ParameterSerializer.ToDictionary(reports) } });

            }
            finally { await session.CloseAsync(); }
        }
        //Crear reporte
        public async Task<string> CreateReport(ReportInfo reportInfo)
        {
            Report auxReport = new Report{
                Id = System.Guid.NewGuid().ToString(),
                Commentary = reportInfo.Report.Commentary.ToString()
            };
            ReportInfo metadata = new ReportInfo 
            {
                Report = auxReport,
                User = reportInfo.User,
                UserReported = reportInfo.UserReported
            };
            reAux = auxReport;
            info = metadata;
            await newReportNodo(Reports);
            string ans = await reportRelationship(ReportInfos);
            return ans;

        }
        /*Interfaz*/

        public IList<Report> Reports
        {
            get
            {
                return new []{reAux};
            }
        }

        public IList<ReportInfo> ReportInfos
        {
            get
            {
                return new []{info};
            }
        }
    }
}