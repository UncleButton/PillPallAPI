
public class ScheduleResponse
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public List<ScheduleMed> ScheduleMeds { get; set; } = new List<ScheduleMed>();

    public List<Time> Times { get; set; } = new List<Time>();
}