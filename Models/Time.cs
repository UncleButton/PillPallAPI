using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Time
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public required string DateTime { get; set; }

    public int ScheduleId {get; set;}
    
    public Schedule Schedule { get; set; }
}