using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProject.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class EmployeeController : ControllerBase
{
    // Fix seed data
    // input valid date
    private readonly EmployeeRepository _EmployeeRepo;
    public EmployeeController(EmployeeRepository employeeRepo) 
    {
        _EmployeeRepo = employeeRepo;
    }

    [HttpGet("getemployees")]
    public async Task<ApiResponse<IEnumerable<EmployeeDto>>> GetEmployees([FromQuery] string? name = null)
    {
        // NOTE: You can also use the CQRS Mediator instead of calling the repository directly,
        // it helps you to reduce dependencies between objects.
        var result = from employee in await _EmployeeRepo.GetEmployeesAsync() 
                        orderby employee.BornDate descending
                        select new EmployeeDto {
                            Id = employee.ID, // TODO: Protect this ID 
                            Name =  employee.Name,
                            RFC = employee.RFC,
                            BornDate = employee.BornDate.ToString("dd-MM-yyy"),
                            Status = (int)employee.Status
                        };

        if(name is not null) 
            result = from employee in result 
                where employee.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase) 
                select employee;

        return new ApiResponse<IEnumerable<EmployeeDto>> { Response = result };
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddEmployee([FromBody] EmployeeDto dto)
    {
        string rfc = dto.RFC.ToUpper();
        if(rfc.Length != 13) 
            return Ok(new ApiResponse<string> { ErrorMessage = "Characters missing from the RFC field" });

        if(!ValidateRFC(rfc, dto.Name.ToUpper()[0], DateTime.Parse(dto.BornDate)))
            return Ok(new ApiResponse<string> { ErrorMessage = "The RFC provided is the wrong format" });

        if(await _EmployeeRepo.FindByRFC(rfc)) 
            return Ok(new ApiResponse<string> { ErrorMessage = "RFC already exists" });
        
        if(DateTime.Compare(DateTime.Parse(dto.BornDate), DateTime.Now.AddYears(-18)) > 0)
            return Ok(new ApiResponse<string> { ErrorMessage = "Select a valid date" });

        await _EmployeeRepo.AddEmployee(new EmployeeModel {
            Name = dto.Name,
            RFC = rfc,
            BornDate = DateTime.Parse(dto.BornDate),
            Status = (EmployeeStatus)dto.Status
        });

        return Ok(new ApiResponse<string> { Response = "Added" });
    }

    [HttpDelete("rm/{id:int}")]
    public async Task<IActionResult> DeleteEmployee([FromRoute]int id)
    {
        // NOTE: Most of the time you just soft-delete a user
        await _EmployeeRepo.DeleteEmployee(id); // TODO: Check for errors
        return Ok(new ApiResponse<string> { Response = $"Deleted-ID: {id}" });
    }

    #region Private Helpers
    private static bool ValidateRFC(string rfc, char initialNameLetter, DateTime bornDate)
    {
        // NOTE: Checks with the name, year, month and day, 
        // the remaining ones just checks the correct format

        for(int i = 1; i <= rfc.Length; i++)
        {
            char c = rfc[i-1];
            switch(i)
            {
                case (int)IndexRFC.LetraInicialPaterno:
                case (int)IndexRFC.VocalInicialPaterno:
                case (int)IndexRFC.LetraInicialMaterno:
                    if(!char.IsLetter(c)) return false;
                    break;
                case (int)IndexRFC.LetraInicialNombre:
                    if(!char.IsLetter(c) || c != initialNameLetter) return false;
                    break;
                case (int)IndexRFC.Año:
                    UInt32 year = 0;
                    if(!UInt32.TryParse($"{c}{rfc[i]}", out year)) return false;
                    // This can be improved
                    int bornYear = bornDate.Year;
                    int first = bornYear % 10;
                    bornYear /= 10;
                    int second = bornYear % 10;
                    if(year != (second*10 + first)) return false;
                    break;
                case (int)IndexRFC.Mes:
                    UInt32 month = 0;
                    if(!UInt32.TryParse($"{c}{rfc[i]}", out month)) return false;
                    if(month < 1 || month > 12 || month != bornDate.Month) return false;
                    break;
                case (int)IndexRFC.Dia:
                    UInt32 day = 0;
                    if(!UInt32.TryParse($"{c}{rfc[i]}", out day)) return false;
                    if(day < 1 || day > 31 || day != bornDate.Day) return false;
                    break;
                case (int)IndexRFC.Unico:
                case (int)IndexRFC.Unico+1:
                case (int)IndexRFC.Unico+2:
                    if(!char.IsLetterOrDigit(c)) return false;
                    break;
            }
        }

        return true;
    }

    enum IndexRFC : byte
    {
        LetraInicialPaterno = 1,
        VocalInicialPaterno = 2,
        LetraInicialMaterno = 3,
        LetraInicialNombre = 4,
        Año = 5, // 5 & 6
        Mes = 7, // 7 & 8
        Dia = 9, // 9 & 10
        Unico = 11 // 11, 12 & 13
    }
    #endregion
}