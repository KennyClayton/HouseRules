//$ "In ASP.NET Core, a controller is a C# class that is responsible for handling incoming HTTP requests and returning HTTP responses. It acts as the central component that receives requests, processes them, and sends back responses."
//* "We use controllers to create endpoints. The framework is able to discover those controllers in our code. How? We use app.MapControllers(); in Program.cs to call all of the below functions"
//* A controller is a class
//* "A controller class contains methods that are the handlers for the endpoints of the API."
//~ This model hold the controllers to create endpoints for chore entity

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HouseRules.Data;
using Microsoft.EntityFrameworkCore;
using HouseRules.Models;
using Microsoft.AspNetCore.Identity;

namespace HouseRules.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChoreController : ControllerBase
{
    private HouseRulesDbContext _dbContext;

    public ChoreController(HouseRulesDbContext context)
    {
        _dbContext = context;
    }

    //# "This method from the ChoreController gets the data for all users. This data should only be available to Admin users (it will be used to terminate employees, hire new ones, as well as upgrading an employee to be an Admin). [Authorize(Roles = "Admin")] ensures that this resource will only be accessible to authenticated users that also have the Admin role associated with their user id. A logged in user that is not an Admin will receive a 403 (Forbidden) response when trying to access this resource."

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok(_dbContext.Chores.ToList());
    }
}