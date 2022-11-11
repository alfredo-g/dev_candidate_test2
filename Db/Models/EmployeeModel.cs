using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeProject;

public class EmployeeModel : BaseModel
{
    [Key]
    public int ID { get; set; }

    [Required]
    [StringLength(32)]
    public string Name { get; set; }

    [Required]
    [StringLength(13)]
    public string RFC { get; set; }

    // TODO: Validate date
    [Required, DataType(DataType.Date)]
    public DateTime BornDate { get; set; }

    [Required]
    public EmployeeStatus Status{ get; set; }
}

public enum EmployeeStatus 
{
    NotSet,
    Active,
    Inactive
}