using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DispenseLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }

    public int? ScheduleId { get; set; }

    public int? MedId {get; set;}

    public int? NumPills { get; set; }

    public DateTime Timestamp {get; set;}
}