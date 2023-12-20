using Microsoft.EntityFrameworkCore;
using AreasDataBase.Models;

namespace AreasDataBase.Data
{
    public class AreasDataBaseContext : DbContext
    {
        public AreasDataBaseContext(DbContextOptions<AreasDataBaseContext> options)
            : base(options)
        {
        }

        public DbSet<Area> Area { get; set; } = default!;
        public DbSet<City> City { get; set; } = default!;
        public DbSet<District> District { get; set; } = default!;
        public DbSet<Street> Street { get; set; } = default!;
        public DbSet<ResidentialBuilding> ResidentialBuilding { get; set; } = default!;
        public DbSet<Apartment> Apartment { get; set; } = default!;
        public DbSet<Citizen> Citizen { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost; Database=RPBD_lab3.v14; Username=postgres; Password=ytn z yt uhb,");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Area>().Property(a => a.IdArea)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions();

            modelBuilder.Entity<City>().Property(c => c.IdCity)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions();

            modelBuilder.Entity<District>().Property(d => d.IdDistrict)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions();

            modelBuilder.Entity<Street>().Property(s => s.IdStreet)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions();

            modelBuilder.Entity<ResidentialBuilding>().Property(r => r.IdResidentialBuilding)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions();

            modelBuilder.Entity<Apartment>().Property(a => a.IdApartment)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions();

            modelBuilder.Entity<Citizen>().Property(c => c.IdCitizen)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions();

            base.OnModelCreating(modelBuilder);
        }
    }
}
