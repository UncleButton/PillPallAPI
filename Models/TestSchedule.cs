using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class TestSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name {get; set;} = "";

    public int? PIN {get; set;}

    public List<ScheduleMed> ScheduleMeds { get; set; } = new List<ScheduleMed>();

    public List<Time> Times { get; set; } = new List<Time>();
}