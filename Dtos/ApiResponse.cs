namespace EmployeeProject;

public record ApiResponse<T>
{
    public string? ErrorMessage { get; set; } = null;
    public bool Successful => ErrorMessage is null;
    public T Response { get; set; }
}