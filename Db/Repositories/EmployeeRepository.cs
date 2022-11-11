using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject;

// TODO: Make a interface
public class EmployeeRepository
{
    private readonly AppDbContext _Context;

    public EmployeeRepository(AppDbContext context)
    {
        _Context = context;
    }

    public async Task<ICollection<EmployeeModel>> GetEmployeesAsync()
    {
        var result = new List<EmployeeModel>();
        try
        {
            result = await _Context.Employees.ToListAsync();
        }
        catch (System.Exception ex)
        {
            // TODO Logging system
            System.Console.WriteLine(ex.Message);
        }

        return result;
    }

    public async Task AddEmployee(EmployeeModel employee)
    {
        try
        {
            await _Context.Employees.AddAsync(employee);
            await _Context.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            // TODO Logging system
            System.Console.WriteLine(ex.Message);
        }

        // TODO: error return
    }

    public async Task<bool> FindByRFC(string rfc)
    {
        try
        {
            var result = await _Context.Employees.FirstOrDefaultAsync(x => x.RFC.Equals(rfc, System.StringComparison.InvariantCultureIgnoreCase));
            return (result is not null);
        }
        catch (System.Exception ex) 
        {
            System.Console.WriteLine(ex.Message);
        }

        return false;
    }

    public async Task DeleteEmployee(int id) 
    {
        var empl = await _Context.Employees.FirstOrDefaultAsync(x => x.ID == id);
        if(empl is not null) 
        {
            empl.Deleted = true;
        }

        await _Context.SaveChangesAsync();
    }
}