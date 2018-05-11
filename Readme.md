# WebapiCore-seed [![Build Status](https://travis-ci.org/MakingSense/WebApiCore-Seed.svg?branch=master)](https://travis-ci.org/MakingSense/WebApiCore-Seed)

## Prerequisites
* .Net core sdk
* A text processor / gui

### Download them
* [For Windows](https://www.microsoft.com/net/download/windows/build)
* [For Linux](https://www.microsoft.com/net/download/linux/build)
* [For Mac](https://www.microsoft.com/net/download/macos/build)

## Getting started

* ### For windows users with visual studio
    1. Open `WebApiCoreSeed.sln` located on the folder where the repository was downloaded

    2. If the WebApiCoreSeed.WebApi project is not selected as startup, just right click it and then click on `Set as StartUp Project` 

        ![set as startup](https://i.imgur.com/fTbU51p.gif)

    3. Now you just have to run it, pressing `F5` or the run button the top, if you use iisexpress configuration your app will be attached to the port `:4992`, if you use the executable, the port used will be `:4993`

        ![Run it](https://i.imgur.com/8TuB31V.gif)

    4. Now you just can open your favorite browser and navigate to `localhost:$Port/swagger` to see all the configured endpoints

* ### For users using vs code / console
    1. Open the folder in vs code

    2. Make sure you are in WebApiCore-Seed folder

    3. Run `dotnet restore` on the integrated terminal, to install the dependencies of the project

    4. Go to `WebApiCoreSeed.WebApi` folder using `cd` command

    5. Run `dotnet run` and wait, this would host the application on the :4993 port

    6. Finally you can open a browser and navigate `localhost:4993/swagger` to check all the available  endpoints

## Next steps 

* ### Creating a new resource

    1. Let's assume that you want a `NewValue` resource, so your first step is give it a model, inside `WebApiCoreSeed.Data/Models` folder, create a new file called `NewValue.cs`

    2. Inside the file add the namespace 
        ``` csharp
        namespace WebApiCoreSeed.Data.Models
        ```

    3. Then you may want to create the class, make sure that inherits from `BaseEntity`

    4. When you're done adding properties add the reference to `System.ComponentModel.DataAnnotations` so you can add the [attributes](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations(v=vs.110).aspx) for a prevalidation 

    5. 
        If you want this resource in the database just add a property into `WebApiCoreSeedContext.cs` like 
        ``` csharp
        public DbSet<NewValue> NewValues { get; set; }
        ```
        and then register the table name into the method `OnModelCreating` (note that the property is plural but the table is singular)

        ``` csharp
        modelBuilder.Entity<NewValue>().ToTable("NewValue");
        ```
        finally if you want some initial data add it into `DatabaseSeed.cs` inside the `Initialize` method and before the `EnsureCreated()` but after the `SaveChanges()`

    6. 
        Then you're going to need a place to drop the business/domain logic. Inside `WebApiCoreSeed.Domain/Service` add a `NewValueService.cs` and into `WebApiCoreSeed.Domain/Service/Contracts` add a `INewValueService.cs`. 

        Inside `INewValueService.cs` add the namespace `WebApiCoreSeed.Domain.Services.Interfaces` and define all the operations that NewValue performs inside an interface.

        Add the namespace `WebApiCoreSeed.Domain.Services` into `NewValueService.cs`, implement all the logic defined on the interface, if you need access to the data base just create a readonly field and "inject it" from the constructor like this

        ``` csharp
        private readonly WebApiCoreSeedContext _dbContext;

        public NewValueService(WebApiCoreSeedContext dbContext)
        {
            _dbContext = dbContext;
        }
        ```

    7. 
        On `WebApiCoreSeed.WebApi/Controllers` add a file with the name `NewValueController.cs`, create a class called `NewValueController` under the namespace `WebApiCoreSeed.WebApi.Controllers`.

        Before the class firm you need to specify the route prefix, we normally use the name of the resource with a `api/` prefix. The class should inherit form `Controller`, is a good practice that every method must have the verb defined as an attribute and always returns an `IActionResult`. Something like this:
        ``` csharp
        [Route("api/newValue")]
        public class NewValueController : Controller
        {
            //...
            [HttpGet]
            public Task<IActionResult> MethodName()
            {
                //..
                return Ok(someValue); // retrun NoContent();
            }
            //...
        }        
        ```

        You should access to the business logic via the service. The service is always injected via controller using the constructor. Like this:

        ``` csharp
        private readonly INewValueService _newValueService;

        public NewValueController(INewValueService newValueService)
        {
            _newValueService = newValueService;
        }
        ```

    8. 
        In order for the framework to interpret what you're injecting, inside the `Startup.cs` file, there's a method called `ConfigureServices`, where you can register your service like this:
    
        ``` csharp
        services.AddTransient<INewValueService>(sp => new UserService(sp.GetRequiredService<WebApiCoreSeedContext>()));
        ```

    9. 
        It's time to document your resource, fortunately, swashbuckle understands the documentation comments, and adds them to the json that SwaggerUi consumes. So, for every action that you perform on your resource, a comment like this needs to exist:

        ``` csharp
        /// <summary>
        /// Gets a new value based on his id
        /// </summary>
        /// <param name="id" cref="Guid">Guid of the new value</param>
        /// <response code="200">The new value that has the given id</response>
        /// <response code="404">a new value with the given id was not found</response>
        /// <response code="500">server error</response>
        /// <return>A new value</return>
        [HttpGet("id")]
        public Task<IActionResult> Get(Guid id){...}
        ``` 

        In order to let swashbuckle understand the type of response you need to specify it with an attribute like this `
        ``` csharp
        /// <summary>
        /// </summary>
        [HttpGet("id")]
        [ProducesResponseType(typeof(NewValue), 200)]
        [ProducesResponseType(typeof(ErrorDto), 400)]
        [ProducesResponseType(typeof(ErrorDto), 500)]
        public Task<IActionResult> Get(Guid id){...}
        ```

    10. Finally it's time to test, for this you need to create two test files, one in `WebApiCoreSeed.UnitTests/Services` to test the service, in this case the file should be called `NewValueServiceTest.cs`, and other in `WebApiCoreSeed.UnitTests/Api/Controllers` for the controller called `NewValueControllerTest.cs`.

## What uses

| Technology  | Description |
| :---------: | ----------- |
| [Asp .Net Web Api Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.0) | Core framework |
| [Entity Framework](https://docs.microsoft.com/en-us/ef/core/) | Data access library |
| [Net Core DI](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-2.0) | Integrated dependecy injection library |
| [Xunit](https://xunit.github.io/) | Unit testing |
| [SwaggerUI](https://swagger.io/swagger-ui/) | Ui that document and exposes the api endpoints |
| [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle) | Documentation generator for swagger |
    
## Architecture
* DDD classic
    * Domain Services.
    * Inversion of control using conventions.
    * Autommaping for custom views decoupled from domain.
  
![demo](http://www.methodsandtools.com/archive/onion17.jpg)
