//$ What does this module do?
//&"This code sets up an initial "Admin" role, creates an admin user with login credentials, and associates that user with the "Admin" role. The HasData method is used to insert these initial records into the database when the database is created or migrated. This is commonly used in applications where you want to have an initial admin user with specific roles and permissions when the application is first deployed or when the database is created. However, make sure to secure the AdminPassword and any sensitive information properly, as it's not safe to store passwords in plain text."

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HouseRules.Models;
using Microsoft.AspNetCore.Identity;
using System.Runtime.Serialization;

namespace HouseRules.Data;
//*IMPORTANT - below, "HouseRulesDbContext is a custom DbContext class that inherits from IdentityDbContext<IdentityUser>. This means it's using Identity Framework for managing user accounts and roles."
public class HouseRulesDbContext : IdentityDbContext<IdentityUser> //# HouseRulesDbContext inherits from the IdentityDbContext<IdentityUser> class, rather than from DbContext
//# IdentityDbContext comes with a number of extra models and tables that will be added to the database. They include:
//# IdentityUser - this will hold login credentials for users
//# IdentityRole - this will hold the various roles that a use can have
//# IdentityUserRole - a many-to-many table between roles and users. These define which users have which roles.

//* LEARN ABOUT DbSet properties and how they give access to the database to get and set values of the properties of our tables
{
    //~ The first DbSet property "represents the Chore entity. It allows you to query and manipulate data in the "Chores" database table. 
    //$ Each of these DbSet properties serves as an entry point for Entity Framework Core to perform database operations (e.g., querying, inserting, updating, deleting) on the corresponding entity types. 
    //~ When you use these properties in your code, Entity Framework Core generates SQL queries to interact with the database tables associated with these entity types.
    //$ In summary, these DbSet properties define the data models or entities in your application and provide a way to interact with the database tables that store data related to those entities."
    private readonly IConfiguration _configuration;
    public DbSet<Chore> Chores { get; set; } 
    //* IMPORTANT - Above is a DbSet property it allows us to get values of properties from the tables in the database. Ie - we can access the chores table, look at the Name property of any chore listed there and get the value of that name property returned to us. We can also SET the value of the name property on a chore....all through this "DbSet Chore property".
    public DbSet<ChoreAssignment> ChoreAssignments { get; set; } //Same here. We can get and set property values for ChoreAssignments in the database
    public DbSet<ChoreCompletion> ChoreCompletions { get; set; } //Same here. This is our entry point into the database to retrieve data and/or submit changes to data for ChoreCompletions
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }


    public HouseRulesDbContext(DbContextOptions<HouseRulesDbContext> context, IConfiguration config) : base(context)
    {
        _configuration = config;
    }

    //* IMPORTANT - below, "in the OnModelCreating method, you're using the modelBuilder to configure the database schema."
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); //# this is a method

        //* below, "This is seeding the database with an "Admin" role. It assigns a unique identifier (Id) to the role. Sets the name of the role as "Admin" and normalizes it to "admin."
        modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole //# seeding the database with the identityrole information
        {
            Id = "c3aaeb97-d2ba-4a53-a521-4eea61e59b35",
            Name = "Admin",
            NormalizedName = "admin"
        });

        //* below, "This is seeding the database with an "admin" IdentityUser. It assigns a unique identifier (Id) to the user. Sets the user's username, email, and password hash. The password hash is generated using PasswordHasher<IdentityUser>"
        modelBuilder.Entity<IdentityUser>().HasData(new IdentityUser //# seeding the database with the identityuser information
        {
            Id = "dbc40bc6-0829-4ac5-a3ed-180f5e916a5f", //* "Id specifies a unique identifier for the IdentityUser."
            UserName = "Administrator", //* "UserName specifies the username as "Administrator."
            Email = "admina@strator.comx", //* "Email specifies the email address as "admina@strator.comx."
            PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, _configuration["AdminPassword"]) //* this " specifies the hashed password for the user, which is generated using PasswordHasher<IdentityUser>().HashPassword(null, _configuration["AdminPassword"])"
        });

        //* below, "This associates the "Admin" role with the "admin" IdentityUser. It specifies the RoleId and UserId to establish this association."
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = "c3aaeb97-d2ba-4a53-a521-4eea61e59b35", //this gives gives the IdentityUser the role of Admin 
            UserId = "dbc40bc6-0829-4ac5-a3ed-180f5e916a5f"
        });

        //* below, "This appears to be seeding a UserProfile entity related to the "admin" IdentityUser. It sets various properties for the UserProfile, including the Id, IdentityUserId, FirstName, LastName, and Address."
        modelBuilder.Entity<UserProfile>().HasData(new UserProfile
        {
            Id = 1,
            IdentityUserId = "dbc40bc6-0829-4ac5-a3ed-180f5e916a5f",
            FirstName = "Admina",
            LastName = "Strator",
            Address = "101 Main Street",
        });

        //^ Seed the database with some chores
        modelBuilder.Entity<Chore>().HasData(new Chore[]
        {
            new Chore
            {
            Id = 1,
            Name = "Mow the lawn",
            Difficulty = 4,
            ChoreFrequencyDays = 7
            },
            new Chore
            {
            Id = 2,
            Name = "Fold the laundry",
            Difficulty = 1,
            ChoreFrequencyDays = 2
            },
            new Chore
            {
            Id = 3,
            Name = "Load and unload dishwasher",
            Difficulty = 2,
            ChoreFrequencyDays = 1
            },
            new Chore
            {
            Id = 4,
            Name = "Mop the floors",
            Difficulty = 4,
            ChoreFrequencyDays = 2
            },
            new Chore
            {
            Id = 5,
            Name = "Clean the bathrooms",
            Difficulty = 5,
            ChoreFrequencyDays = 5
            }
        });

        //^ Seed the database with some ChoreAssignments
        //& "These represent the chores a user is currently assigned. You can just assign them to the one admin user that will be created when the database is created."
        modelBuilder.Entity<ChoreAssignment>().HasData(new ChoreAssignment[]
        {
            new ChoreAssignment // first assignment is for user 1 to mop the floors every 2 days
            {
            Id = 1,
            UserProfileId = 1,
            ChoreId = 4
            },
            new ChoreAssignment // second assignment is also for user 1 to mow the lawn every 7 days
            {
            Id = 2,
            UserProfileId = 1,
            ChoreId = 1
            }
        });

        //^ Seed the database with a ChoreCompletion
        //& "These represent individual completions of a chore, and can be completed by any user, not only users that are assigned that chore."
        modelBuilder.Entity<ChoreCompletion>().HasData(new ChoreCompletion[]
        {
            new ChoreCompletion // first chore completed was by user 1 who folded the laundry on 10/4/23
            {
            Id = 1,
            UserProfileId = 1,
            ChoreId = 2,
            CompletedOn = new DateTime(2023, 10, 4)
            },
            new ChoreCompletion // second chore completed was by user 1 who did the dishes also on 10/4/23
            {
            Id = 2,
            UserProfileId = 1,
            ChoreId = 3,
            CompletedOn = new DateTime(2023, 10, 4)
            },
            new ChoreCompletion // third chore completed was by user 1 who cleaned the bathrooms on 10/5/23
            {
            Id = 3,
            UserProfileId = 1,
            ChoreId = 5,
            CompletedOn = new DateTime(2023, 10, 5)
            }
        });





    }
}