using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Schedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }

    public int UserId { get; set; }

    public string Name {get; set;} = "";

    public int? PIN {get; set;}

    public List<ScheduleMed>? ScheduleMeds { get; set; }

    public List<Time>? Times { get; set; } = new List<Time>();

    public string Days { get; set; } = "";

    public string notificationEmail = "";
}