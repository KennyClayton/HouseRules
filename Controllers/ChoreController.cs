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
    [HttpPost("{id}/complete")] //*This parameter allows you to capture the id value from the URL route.
    // [Authorize]
    public IActionResult PostNewCompletedChore(int id, [FromQuery] int userId) //* These 2 parameters allow you to capture the id values from the URL route. So in Postman if I enter this, it will assign chore 3 as being completed by userProfileId 2 "https://localhost:5001/api/chore/3/complete?userprofileid=2" Because it is taking the 3 as int id for which chore Id to update...and it is taking the 2 as the FromQuery int userId (and remember userid and id could be called anything we wanted, so it didn't have to be userProfileId....it's just a variable)
    {
        //Get a chore by taking the chore Id from the user and searching the database for a match
        Chore chore = _dbContext.Chores.SingleOrDefault(c => c.Id == id);
        if (chore == null)
        {
            return NotFound("No chore by that id exists");
        }
        //Create a new chore completion
        var choreCompleted = new ChoreCompletion
        {
            UserProfileId = userId, // Replace with the userProfileId passed in the POST request
            UserProfile = null, // You can populate UserProfile if needed
            ChoreId = id,
            Chore = chore, //set the Chore property to the instance of the chore passed to us by the user
            CompletedOn = DateTime.Now // Set the CompletedOn property to the current date and time
        };
        // Add the chore completion to the database context
        _dbContext.ChoreCompletions.Add(choreCompleted);
        _dbContext.SaveChanges();

        // Return a 204 No Content response
        return NoContent();
    }


    //^ ENDPOINTS
    //& accessible to admin users only:

    //^ POST /api/chore
    //& Post a new chore to be created
    //~ Creating a new thing/object in the db involves two heavy lifters: POST and .Add
    //~ 1. specifying POST as the HTTP method and ...
    //~ 2. using the .Add method on a template object. 
    [HttpPost] //no parameter needed. /chore is the endpoint already.
    [Authorize(Roles = "Admin")] //only admins can access
    public IActionResult PostNewChore(Chore chore) //pass in a chore object to work with below
    {
        _dbContext.Chores.Add(chore); //go to the db, select chores table, add a chore instance/object
        _dbContext.SaveChanges(); //save the changes to the database
        return Created($"/api/chore/{chore.Id}", chore); //return an HTTP response "201 Created" with the Created method. 
    }


    //^ PUT /api/chore/{id}
    //& This endpoint should allow updating all of the columns of the Chore table (except Id)
    //the columns are the values of each property. ie - the Chore table has a property of Name. And each Name row will have a value for that Name property. That value should be updateable.
    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin")]
    public IActionResult UpdateAChore(Chore chore, int id) //give the function below a chore instance and feed it an id
    {
        Chore choreToUpdate = _dbContext.Chores.SingleOrDefault(c => c.Id == id); //grab a chore by Id to update it
        if (choreToUpdate == null)
        {
            return NotFound();
        }
        else if (id != chore.Id)
        {
            return BadRequest();
        }
        //These are the only properties that we want to make editable
        choreToUpdate.Name = chore.Name; // the new name value will go into the chore.Name value
        choreToUpdate.Difficulty = chore.Difficulty;
        choreToUpdate.ChoreFrequencyDays = chore.ChoreFrequencyDays;

        _dbContext.SaveChanges();

        return NoContent();
    }


    //^ DELETE /api/chore/{id}
    //& This endpoint will delete a chore with the matching id
    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    public IActionResult DeleteAChore(int id) //give the function below an id //? Do we not have to pass a Chore chore because it can be inferred that it is needed when we code a Chore chore below?
    {
        Chore choreToDelete = _dbContext.Chores.SingleOrDefault(c => c.Id == id); //find a chore among the list of chores by Id to delete it
        if (choreToDelete == null) // If the work order with the specified ID does not exist, return a "Not Found" response
        {
            return NotFound();
        }
        _dbContext.Chores.Remove(choreToDelete);
        _dbContext.SaveChanges();

        return NoContent();
    }


    //^ POST /api/chore/{id}/assign
    //& This endpoint will assign a chore to a user.
    //& Pass the userId in as a query string param, as in the completion endpoint above.
    //& This endpoint can return a 204 response.
    [HttpPost("{id}/assign")] //*This parameter allows you to capture the id value from the URL route.
    // [Authorize(Roles = "Admin")]
    public IActionResult AssignChore(int id, [FromQuery] int userId) //* The {id} route parameters allow you to capture the id value from the URL route. The userId is provided in the url query string. So in Postman if I enter this, it will assign choreId 3 to userProfileId 2 "https://localhost:5001/api/chore/3/assign?userprofileid=2" Because it is taking the 3 as int id for which chore Id to update...and it is taking the 2 as the FromQuery int userId (and remember userid and id could be called anything we wanted, so it does not have to be userProfileId....it's just a variable)
    {
        //Get a chore by taking the chore Id from the user and searching the database for a match
        Chore chore = _dbContext.Chores.SingleOrDefault(c => c.Id == id);
        if (chore == null)
        {
            return NotFound("No chore assignment by that id exists");
        }
        // Create a new chore completion
        var choreToAssign = new ChoreAssignment //store this newly created object in a variable called "choreToAssign"
        {
            UserProfileId = userId, // Replace userId with the userProfileId number that we pass in the POST request url (FromQuery), which is 2 in this example
            // UserProfile = null, // You can populate UserProfile if needed
            ChoreId = id //this "id" value also comes from the url we use in Postman
            // Chore = chore, //set the Chore property to the instance of the chore passed to us by the user, ie the one that we found by matching the chore.Id number given to us in the url
        };
        // Add the chore assignment to the database context
        _dbContext.ChoreAssignments.Add(choreToAssign);
        _dbContext.SaveChanges();

        // Return a 204 No Content response
        return NoContent();
    }

    // this below version Alex helped me out with until i could figure out why my version wasnt working
    // [HttpPost("{id}/assign")] 
    // public IActionResult AssignChore(int id, int? userId)
    // {
    //     ChoreAssignment newChoreAssignment = new ChoreAssignment();
    //     if (userId != null)
    //     {
    //         int userIdAsInt = (int)userId;
    //         newChoreAssignment.UserProfileId = userIdAsInt;
    //         newChoreAssignment.ChoreId = id;
    //         _dbContext.ChoreAssignments.Add(newChoreAssignment);
    //         _dbContext.SaveChanges();
    //         return NoContent();
    //     }
    //     return BadRequest();
    // }



    //^ POST /api/chore/{id}/unassign
    //& This endpoint will unassign a chore to a user.
    //& Pass the userId in as a query string param, as in the other endpoints above.

    [HttpPost("{id}/unassign")] //*This parameter allows you to capture the id value from the URL route.
    // [Authorize(Roles = "Admin")]
    public IActionResult UnassignChore(int id, [FromQuery] int userId) //* These two parameters allow you to capture the id values from the URL route. So in Postman if I enter this, it will assign choreId 3 to userProfileId 1 "https://localhost:5001/api/chore/3/unassign?userprofileid=1" Because it is taking the 3 as int id for which chore Id to update...and it is taking the 1 as the FromQuery int userId (and remember userid and id could be called anything we wanted, so it does not have to be userProfileId....it's just a variable)
    {
        {
            // Retrieve multiple ChoreAssignments that match the ChoreId and UserProfileId
            var choreAssignments = _dbContext.ChoreAssignments
                .Where(ca => ca.ChoreId == id && ca.UserProfileId == userId)
                .ToList();

            if (choreAssignments.Count == 0)
            {
                return NotFound("No chore assignments match the provided ChoreId and UserProfileId.");
            }

            // Perform any necessary actions with the choreAssignments here. Here, we will delete the assignments:
            foreach (var assignment in choreAssignments)
            {
                _dbContext.ChoreAssignments.Remove(assignment);
            }

            _dbContext.SaveChanges();

            return NoContent();
        }
    }

}