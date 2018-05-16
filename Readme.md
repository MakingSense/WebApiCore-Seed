# WebapiCore-seed [![Build Status](https://travis-ci.org/MakingSense/WebApiCore-Seed.svg?branch=master)](https://travis-ci.org/MakingSense/WebApiCore-Seed)

## Prerequisites

.NET Core SDK

A text processor / gui

### Download them

* [For Windows](https://www.microsoft.com/net/download/windows/build)
* [For Linux](https://www.microsoft.com/net/download/linux/build)
* [For Mac](https://www.microsoft.com/net/download/macos/build)

## Getting started

### For Windows users with Visual Studio

1. Open `WebApiCoreSeed.sln` located on the folder where the repository was downloaded

2. If the Seed.Api project is not selected as startup, just right click it and then click on `Set as StartUp Project` 

    ![set as startup](https://i.imgur.com/fTbU51p.gif)

3. Now you just have to run it, pressing `F5` or the run button the top, if you use iisexpress configuration your app will be attached to the port `:4992`, if you use the executable, the port used will be `:4993`

    ![Run it](https://i.imgur.com/8TuB31V.gif)

4. Now you just can open your favorite browser and navigate to `localhost:$Port/swagger` to see all the configured endpoints

### For users using vs code / console

1. Open the folder in vs code

2. Make sure you are in WebApiCore-Seed folder

3. Run `dotnet restore` on the integrated terminal, to install the dependencies of the project

4. Go to `Seed.Api` folder using `cd` command

5. Run `dotnet run` and wait, this would host the application on the :4993 port

6. Finally you can open a browser and navigate `localhost:4993/swagger` to check all the available  endpoints

## What uses

| Technology  | Description |
| :---------: | ----------- |
| [Asp .Net Web Api Core](https://docs.microsoft.com/en-us/aspnet/core/) | Core framework |
| [Entity Framework](https://docs.microsoft.com/en-us/ef/core/) | Data access library |
| [Net Core DI](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection) | Integrated dependecy injection library |
| [Xunit](https://xunit.github.io/) | Testing framework |
| [SwaggerUI](https://swagger.io/swagger-ui/) | UI that document and exposes the API endpoints |
| [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle) | Documentation generator for Swagger |
    
## Architecture
* DDD classic
    * Domain Services.
    * Inversion of control using conventions.
    * Autommaping for custom views decoupled from domain.
  
![demo](http://www.methodsandtools.com/archive/onion17.jpg)
