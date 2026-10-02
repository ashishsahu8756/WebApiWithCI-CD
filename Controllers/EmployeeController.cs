using ApiProject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
 
namespace ApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> InsertEmployee(Employee employee)
        {
            try
            {
                using IDbConnection connection =
                    new SqlConnection(
                        _configuration.GetConnectionString("DefaultConnection"));

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>("Employee_Insert",
                    new
                    {
                        employee.Name,
                        employee.Email,
                        employee.Salary
                    },
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    return BadRequest("Unable to insert employee");
                }

                if (result.Id == 0)
                {
                    return Conflict(new
                    {
                        message = result.Message
                    });
                }

                return Ok(new
                {
                    id = result.Id,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal Server Error",
                    error = ex.Message
                });
            }
        }
    }
}
