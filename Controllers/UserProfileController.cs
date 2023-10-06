//$ "In ASP.NET Core, a controller is a C# class that is responsible for handling incoming HTTP requests and returning HTTP responses. It acts as the central component that receives requests, processes them, and sends back responses."
//* "We use controllers to create endpoints. The framework is able to discover those controllers in our code. How? We use app.MapControllers(); in Program.cs to call all of the below functions"
//* A controller is a class
//* "A controller class contains methods that are the handlers for the endpoints of the API."


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HouseRules.Data;
using Microsoft.EntityFrameworkCore;
using HouseRules.Models;
using Microsoft.AspNetCore.Identity;

namespace HouseRules.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
    private HouseRulesDbContext _dbContext;

    public UserProfileController(HouseRulesDbContext context)
    {
        _dbContext = context;
    }

    //# "This method from the UserProfileController gets the data for all users. This data should only be available to Admin users (it will be used to terminate employees, hire new ones, as well as upgrading an employee to be an Admin). [Authorize(Roles = "Admin")] ensures that this resource will only be accessible to authenticated users that also have the Admin role associated with their user id. A logged in user that is not an Admin will receive a 403 (Forbidden) response when trying to access this resource."


//* LEARN THE STRUCTURE OF A CONTROLLER 
//~Controllers have methods applied to them. Below is a breakdown of the method applied to the handler
    [HttpGet] //This is an http method attribute specifying the HTTP verb ... it handles HTTP GET requests
    [Authorize] //This is also called an attribute that says the action method below should be protected. "[Authorize] does not specify a specific role, so it allows any authenticated user to access the action. However, [Authorize(Roles = "Admin")] would specify that only users with the "Admin" role can access the action."
    //@ -------------------BEGIN METHOD------------------- // this method is applied to the handler below it
    public IActionResult Get() 
        //@ public "is a public method. It can be accessed from outside the class."
        //@ IActionResult "is the return type of the method, indicating that the method will return an object that represents an HTTP response."
        //@ Get "This is the method name.  It's the name of the action method that handles HTTP GET requests to the specified route."
    //@ -------------------END METHOD-------------------
    //& -------------------BEGIN HANDLER------------------- //this handler enclosed by {} says what to do with the HTTP GET request is made to the endpoint
    {
        return Ok(_dbContext.UserProfiles.ToList()); 
            //& return Ok(_dbContext.UserProfiles.ToList()); 
            //This is the actual logic inside the method. It retrieves a list of user profiles from the database (_dbContext.UserProfiles.ToList()) and returns it as an HTTP response with a status code 200 OK (Ok()).
    }
    //& -------------------END HANDLER-------------------



    [HttpGet("withroles")]
    // [Authorize(Roles = "Admin")]
    public IActionResult GetWithRoles()
    {
        // "The query [below] gets user profiles, then searches for user roles associated with the profile, and maps each of those to role names."
        return Ok(_dbContext.UserProfiles
        .Include(up => up.IdentityUser)
        .Select(up => new UserProfile
        {
            Id = up.Id,
            FirstName = up.FirstName,
            LastName = up.LastName,
            Address = up.Address,
            Email = up.IdentityUser.Email,
            UserName = up.IdentityUser.UserName,
            IdentityUserId = up.IdentityUserId,
            Roles = _dbContext.UserRoles
            .Where(ur => ur.UserId == up.IdentityUserId)
            .Select(ur => _dbContext.Roles.SingleOrDefault(r => r.Id == ur.RoleId).Name)
            .ToList()
        }));
    }

    [HttpPost("promote/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Promote(string id)
    {
        IdentityRole role = _dbContext.Roles.SingleOrDefault(r => r.Name == "Admin");
        // This will create a new row in the many-to-many UserRoles table.
        _dbContext.UserRoles.Add(new IdentityUserRole<string>
        {
            RoleId = role.Id,
            UserId = id
        });
        _dbContext.SaveChanges();
        return NoContent();
    }

    [HttpPost("demote/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Demote(string id)
    {
        IdentityRole role = _dbContext.Roles
            .SingleOrDefault(r => r.Name == "Admin");
        IdentityUserRole<string> userRole = _dbContext
            .UserRoles
            .SingleOrDefault(ur =>
                ur.RoleId == role.Id &&
                ur.UserId == id);

        _dbContext.UserRoles.Remove(userRole);
        _dbContext.SaveChanges();
        return NoContent();
    }

    //^GET /api/userprofile/{id}
    [HttpGet("{id}")]
    [Authorize] // no role specified here so that ANY logged in user can access this

    public IActionResult GetUserProfileWithChores(int id) //? why are we using a string data type on Id property in some foreign keys? I changed it to int here as the parameter
    {
        return Ok(_dbContext.UserProfiles
            .Include(up => up.ChoreAssignments)
                .ThenInclude(ca => ca.Chore)
            .Include(up => up.ChoreCompletions)
                .ThenInclude(cc => cc.Chore)
            .SingleOrDefault(up => up.Id == id)); //single out the matching results of the where filter above.
    }

}

//* IMPORTANT - 
//* "After filtering with Where, you typically use SingleOrDefault or FirstOrDefault to extract a single item from the filtered results.
//# SingleOrDefault is used when you expect there to be at most one matching item in the collection. It throws an exception if there is more than one matching item or if there are no matching items.
//~ firstOrDefault is used when you expect one or more matches but only want the first matching item (if any). It doesn't throw an exception if there are multiple matches; it just returns the first one.
