using System.IO;
using System.Threading.Tasks;
using CodeMetricsSchema;
using Metrics.Data;

namespace MetricsImport
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null)
                args = new[] {"..\\..\\WebjetPlanIt_Master_Metrics_CI_20130418.4-Metrics"};

            var metricsXmlData = CodeMetricsReport.Load(args[0]);
            string buildName = Path.GetFileNameWithoutExtension(args[0]);
            var importer = new XmlImporter(new MetricsDbContext());

            Task importTask = importer.Import(metricsXmlData, buildName);
            importTask.Wait();
        }
    }
}
