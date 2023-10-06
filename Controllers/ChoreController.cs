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
    //* These two parts combined ensure that your ChoreController has access to the HouseRulesDbContext when it needs to interact with the database. 
    //@ PART 1 - get an instance of the database
    private HouseRulesDbContext _dbContext; //* IMPORTANT - this is a dependency for 
    // "This line [above] declares a private field named "_dbContext", which is an instance of the HouseRulesDbContext class. This field will hold an instance of the database context."
    //@ PART 2 - assign the database instance to a variable so we can use it later in code
    public ChoreController(HouseRulesDbContext context)
    {
        _dbContext = context; //this is the constructor body that gives the database a variable named _dbContext which we use in our http controllers below
    }
    // Above is called the "constructor for your ChoreController class. It takes a HouseRulesDbContext object as a parameter, and within the constructor body, it assigns the passed context to the _dbContext field, effectively setting up the dependency injection for the controller."



    //^ ENDPOINTS 
    //~accessible to ALL logged in users:

    //^ GET /api/chore
    //~ This endpoint will return all chores
    [HttpGet]
    // [Authorize]
    public IActionResult Get()
    {
        return Ok(_dbContext.Chores.ToList());
    }


    //^ GET /api/chore/{id}
    //~ This endpoint will return a chore with the current assignees and all completions (you do not need to include each UserProfile that did the completion)
    [HttpGet("{id}")]
    // [Authorize]
    public IActionResult GetAChoreByIdWithCompletions(int id)
    {
        
        Chore chore = _dbContext.Chores
        .Include(c => c.ChoreCompletions)
        .SingleOrDefault(c => c.Id == id);
        if (chore == null)
            {
                return NotFound();
            }
        return Ok(chore);
    }

    //^ POST /api/chore/{id}/complete
    //~ This endpoint will create a new ChoreCompletion.
    //~ Use a query string parameter to indicate the userId that will be assigned to the chore matching the id in the URL.
    //~ Set the CompletedOn property in the controller method so that the client doesn't have to pass it in.
    //~ This endpoint can return a 204 No Content response once it has created the completion.



    //^ ENDPOINTS
    //& accessible to admin users only:

    //^ POST /api/chore
    //& Post a new chore to be created



    //^ PUT /api/chore/{id}
    //& This endpoint should allow updating all of the columns of the Chore table (except Id)



    //^ DELETE /api/chore/{id}
    //& This endpoint will delete a chore with the matching id



    //^ POST /api/chore/{id}/assign
    //& This endpoint will assign a chore to a user.
    //& Pass the userId in as a query string param, as in the completion endpoint above.
    //& This endpoint can return a 204 response.



    //^ POST /api/chore/{id}/unassign
    //& This endpoint will unassign a chore to a user.
    //& Pass the userId in as a query string param, as in the other endpoints above.





}