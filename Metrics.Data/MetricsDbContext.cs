using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Metrics.Data
{
    public class MetricsDbContext : DbContext, IMetricsStore
    {
        public MetricsDbContext() : base("name=TFSCodeMetricsConnection")
        {    
        }

        public MetricsDbContext(string connectionString) : base(connectionString)
        {
        }

        public IDbSet<Build> Builds { get; set; }
        public IDbSet<Target> Targets { get; set; }
        public IDbSet<Module> Modules { get; set; }
        public IDbSet<Namespace> Namespaces { get; set; }
        public IDbSet<CodeType> CodeTypes { get; set; }
        public IDbSet<CodeMember> CodeMembers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //http://stackoverflow.com/questions/1334143/sql-server-datetime2-vs-datetime
            modelBuilder.Properties<DateTime>()
                        .Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Entity<Build>()
                        .HasMany(e => e.Metrics)
                        .WithMany();

            modelBuilder.Entity<Target>()
                        .HasMany(e => e.Metrics)
                        .WithMany();

            modelBuilder.Entity<Module>()
                        .HasMany(e => e.Metrics)
                        .WithMany();

            modelBuilder.Entity<Namespace>()
                        .HasMany(e => e.Metrics)
                        .WithMany();

            modelBuilder.Entity<CodeType>()
                        .HasMany(e => e.Metrics)
                        .WithMany();

            modelBuilder.Entity<CodeMember>()
                        .HasMany(e => e.Metrics)
                        .WithMany();

            base.OnModelCreating(modelBuilder);
        }
    }
}
