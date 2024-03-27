using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Medication
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    //pill info
    public required string Name {get; set;}
    public string? Description {get; set;}
    public required int NumPills {get;set;}
    public required string ExpirationDate {get;set;}
    public string PIN { get; set; } = "";

    //dosage info
    public int? MaxPillsPerDose {get;set;}
    public int? MaxDosesPerDay {get;set;}

    //pharmacy info
    public string? PrescriberName {get; set;}
    public string? PharmacyAddr1 {get; set;}
    public string? PharmacyAddr2 {get; set;}
    public string? PharmacyCity {get;set;}
    public string? PharmacyState {get;set;}
    public string? PharmacyZip {get;set;}
    public string? PharmacyPhone {get;set;}
    public List<ScheduleMed> ScheduleMeds { get; set; } = new List<ScheduleMed>();

    public int? UserId {get; set;}
}