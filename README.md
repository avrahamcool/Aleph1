# Aleph1
Common solutions

this repository includes all the code for Aleph1 projects

## Build
Clone the project

each project in the solution results in a nuget package, build the desired project in release mode and then Pack.

### 0. Aleph1.DI
* #### Aleph1.DI.Contracts
adding this nuget to your project will add the necessary boilerplate code for registering a new Dependency into the DI container.

this Dependency can now be used by DI Consumers without directly referencing the dependency itself.
* #### Aleph1.DI.UnityImplementation
adding this project to a `main` project will add the necessary boilerplate code for registering all dependencies from the dependents into the DI without referencing them.

* #### Aleph1.DI.CustomConfigurationSection
custom ConfigSection for easy configuring the DI from app/web config

### 1. Aleph1.Utilities
* #### Aleph1.Utilities
general helper functions for web/desktop Applications. (like getting the current logged in user regardless if in a web app or desktop)

### 2. Aleph1.Logging
* #### Aleph1.Logging
adding a [Logged] aspect for easy logging every function in your code. (can be easily configured via the config file)

adding a helper function to facilitate NLog logging in Aleph1 format

example: (in any class)
```csharp
ILogger logger = LogManager.GetCurrentClassLogger();
logger.LogAleph1(LogLevel.Info, "message");
```

### 3. Aleph1.Security
* #### Aleph1.Security.Contracts
a common interface for the custom Security handler used by Aleph1 projects

* #### Aleph1.Security.Implementation.3DES
a concrete implementation of the Security interface using 3DES

### 4. Aleph1.ClientFile
* #### Aleph1.ClientFile.Models
ClientFile Model for easy file uploading via JSON POST directly from the client.
Should be use with [aleph1-client-file](https://github.com/avrahamcool/aleph1-client-file)