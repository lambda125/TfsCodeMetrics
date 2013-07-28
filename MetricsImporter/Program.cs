extern alias metrix;
using XBuild = metrix::CodeMetricsReport;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Metrics.Data;

namespace Metrics.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || !args.Any())
                args = new[] { "Sample.xml" };
                //args = new[] { "..\\..\\WebjetPlanIt_Master_Metrics_CI_20130418.4-Metrics.xml" };

            var metricsXmlData = XBuild.Load(args[0]);
            string buildName = Path.GetFileNameWithoutExtension(args[0]);
            var importer = new XmlImporter(new MetricsDbContext());

            Task importTask = importer.Import(metricsXmlData, buildName);
            importTask.Wait();

#if DEBUG
            Console.WriteLine("Done importing. Press [Enter] to close.");
            Console.ReadLine();
#endif
        }
    }
}
