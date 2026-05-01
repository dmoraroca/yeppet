// Als `using`:
//   using Zuppeto.Infrastructure.Persistence.Entities;

// Dins de la classe ZuppetoDbContext, després dels altres DbSet<>:
    public DbSet<CountryRecord> Countries => Set<CountryRecord>();

    public DbSet<CityRecord> Cities => Set<CityRecord>();
