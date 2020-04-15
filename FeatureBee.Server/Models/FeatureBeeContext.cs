namespace FeatureBee.Server.Models
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    using FeatureBee.Server.Migrations;

    public class FeatureBeeContext : DbContext
    {
        public FeatureBeeContext()
            : base("FeatureBeeContext")
        {
            //Database.Log = s => Debug.WriteLine(s);
            Database.SetInitializer<FeatureBeeContext>(null);
            //Database.SetInitializer(new CreateDatabaseIfNotExists<FeatureBeeContext>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<FeatureBeeContext, Configuration>());
        }

        public DbSet<FeatureViewModel> Features { get; set; }
        public DbSet<FeatureHistoryViewModel> FeatureHistory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<FeatureViewModel>()
                .HasMany(x => x.Conditions)
                .WithRequired(x => x.FeatureViewModel)
                .WillCascadeOnDelete(true);
        }

        public void Initialize(bool force)
        {
            Database.Initialize(force);
        }
    }
}