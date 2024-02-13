using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<Container> Containers { get; set; }
    public DbSet<ContainerMedMap> ContainerMedMaps { get; set; }
    public DbSet<DispenseLog> DispenseLogs { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<ScheduleMed> ScheduleMeds { get; set; }
    public DbSet<Time> Times { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Schedule>()
            .HasMany(p => p.Times)
            .WithOne(c => c.Schedule)
            .HasForeignKey(c => c.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ScheduleMed>()
            .HasKey(pc => new {pc.ScheduleId, pc.MedicationId});

        modelBuilder.Entity<ScheduleMed>()
            .HasOne(pc => pc.Medication)
            .WithMany(p => p.ScheduleMeds)
            .HasForeignKey(pc => pc.MedicationId);

        modelBuilder.Entity<ScheduleMed>()
            .HasOne(pc => pc.Schedule)
            .WithMany(p => p.ScheduleMeds)
            .HasForeignKey(pc => pc.ScheduleId);

    }

}