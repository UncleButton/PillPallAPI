using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ScheduleMed
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? MedicationId {get;set;}
    public Medication Medication { get; set; }
    public int NumPills {get;set;}
    public int? ScheduleId {get; set;}
    public Schedule Schedule { get; set; }
}