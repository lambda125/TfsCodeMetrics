extern alias metrix;
//aliases for global metrics xml classes
using System;
using XBuild = metrix::CodeMetricsReport;
using XTarget = metrix::Target;
using XModule = metrix::Module;
using XNamespace = metrix::Namespace;
using XType = metrix::Type;
using XMember = metrix::Member;
using XMetric = metrix::Metric;

using Ploeh.AutoFixture;
using System.Linq;

namespace Metrics.SampleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || !args.Any())
            {
                args = new[] { "Sample.xml" };
            }

            var fixture = new Fixture();

            fixture.Customize<XMetric>(c => c.Without(m => m.Untyped));
            fixture.Customize<XType>(c => c.Without(m => m.Untyped));
            fixture.Customize<XNamespace>(c => c.Without(m => m.Untyped));
            fixture.Customize<XModule>(c => c.Without(m => m.Untyped));
            fixture.Customize<XTarget>(c => c.Without(m => m.Untyped));
            fixture.Customize<XBuild>(c => c.Without(m => m.Untyped));

            var data = fixture.Create<XBuild>();

            data.Save(args[0]);

            Console.Write("Done generated sample file '{0}'. Press [Enter] to close.", args[0]);
            Console.ReadLine();
        }
    }
}
