using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeProject;

public record EmployeeDto
{
    // TODO: Use DataProtector to hide the actual id
    public int Id { get; set; }

    [Required]
    [RegularExpression(@"^(([A-za-z]+[\s]{1}[A-za-z]+)|([A-Za-z]+))$")]
    public string Name { get; set; }

    [Required]
    [StringLength(13)]
    public string RFC { get; set; }

    [Required]
    public string BornDate { get; set; }

    [Required]
    public int Status { get; set; }
}