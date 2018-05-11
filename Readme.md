[![Build Status](https://travis-ci.org/MakingSense/WebApiCore-Seed.svg?branch=master)](https://travis-ci.org/MakingSense/WebApiCore-Seed)
# WebapiCore-seed

## Prequisites
 * .Net core sdk
    * [Instructions for the latest sdk](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.1.200-sdk-download.md)
 * A text processor / gui like
    * [Visual Studio](https://www.visualstudio.com/es/downloads/)
    * [Visual studio code](https://code.visualstudio.com/Download)

[Net core tools download page](https://www.microsoft.com/net/download/windows)

## Getting started

* ### For windows users with visual studio
    1. Open `WebApiCoreSeed.sln` located on the folder where the repository was downloaded

    2. If the WebApiCoreSeed.WebApi project is not selected as startup, just right click it and then click on `Set as StartUp Project` 

    ![set as startup](https://i.imgur.com/fTbU51p.gif)

    3. Now you just have to run it, pressing `F5` or the run button the top, if you use iisexpress configuration your app will be attached to the port `:4992`, if you use the excecutable, the port used will be `:4993`

    ![Run it](https://i.imgur.com/8TuB31V.gif)

    4. Now you just can open your favorite browser and navigate to `localhost:$Port/swagger` to see all the configured endpoints

* ### For users using vs code / console
    1. Open the folder in vs code

    2. Make sure you are in WebApiCore-Seed folder

    3. Run `dotnet restore` on the integrated terminal, to install the dependencies of the project

    4. Go to `WebApiCoreSeed.WebApi` folder using `cd` command

    5. Run `dotnet run` and wait, this would host the application on the :4993 port

    6. Finnaly you can open a browser and navigate `localhost:4993/swagger` to check all the availdable endpoints



## Technologies


* Server Side

| Tecnology  | Reference |
| ---- | --------- |
 Asp .Net Web Api Core | [Reference](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.0) | 
| Entity Framework(as DAL) | [Reference](https://docs.microsoft.com/en-us/ef/core/) |
| Net Core DI | [Reference](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-2.0) |
| Xunit | [Reference](https://xunit.github.io/) | 
| SwaggerUI | [Reference](https://swagger.io/swagger-ui/) | 
| Swashbuckle | [Reference](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.0&tabs=visual-studio%2Cvisual-studio-xml) | 
    
## Architecture
* DDD classic
    * Domain Services.
    * Inversion of control using conventions .
    * Autommaping for custom views decoupled from domain.
  
![demo](http://www.methodsandtools.com/archive/onion17.jpg)
