[![Build Status](https://travis-ci.org/MakingSense/WebApiCore-Seed.svg?branch=master)](https://travis-ci.org/MakingSense/WebApiCore-Seed)
# WebapiCore-seed

## Prequisites
 * .Net core sdk
    * [Instruction for the latest sdk](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.1.200-sdk-download.md)
 * A text processor / gui like
    * [Visual Studio](https://www.visualstudio.com/es/downloads/)
    * [Visual studio code](https://code.visualstudio.com/Download)

[Net core tools download page](https://www.microsoft.com/net/download/windows)

## Getting started

* ### For windows users with visual studio
    1. Open `WebApiCoreSeed.sln` located on the folder where the repository was downloaded

    2. If the WebApiCoreSeed.WebApi project is not selected as startup, just right click it and then click on `Set as StartUp Project` 
    ![set as startup](https://i.imgur.com/fTbU51p.gif)

    3. Now you just have to run it, for this, you have two options, IIS Express or the , 

## Technologies
* Server Side
    * Asp .Net Web Api Core
    * Entity Framework(as DAL)
    * Net Core DI
    * Xunit
    
## Architecture
* DDD classic
    * Domain Services.
    * Inversion of control using conventions .
    * Autommaping for custom views decoupled from domain.
  
![demo](http://www.methodsandtools.com/archive/onion17.jpg)
