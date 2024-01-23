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
    public DbSet<ScheduleTime> ScheduleTimes { get; set; }
    public DbSet<Time> Times { get; set; }
    public DbSet<User> Users { get; set; }
    
}