using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
namespace gatherme_suggestion_ms.Service
{
    public interface IReportService
    {
        IList<Report> Reports { get; }
        IList<ReportInfo> ReportInfos {get;}
    }
}