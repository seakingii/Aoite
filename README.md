# Aoite [![NuGet](https://img.shields.io/nuget/v/Aoite.svg)](https://www.nuget.org/packages/Aoite) [![NuGet](https://img.shields.io/nuget/dt/Aoite.svg)](https://www.nuget.org/packages/Aoite/)

**Aoite is a library that makes it quicker and easier to any .Net Framework projects.**


## Download

The Aoite library is available on nuget.org via package name `Aoite`.

To install Aoite, run the following command in the Package Manager Console

    PM> Install-Package Aoite

More information about NuGet package avaliable at https://nuget.org/packages/Aoite

## Builds

You'll need .NET Framework 4.0 or later to use the precompiled binaries. To build client, you'll need Visual Studio 2015.

Launch the Solution Configuration Manager, select build configuration:

- [Debug | Release]: Build with .Net Framework **4.6** runtime.
- [Debug | Release].Net45: Build with .Net Framework **4.5** runtime.
- [Debug | Release].Net40: Build with .Net Framework **4.0** runtime.

## Getting Started

### Quick start with Aoite.Data

```c#
    var engine = new DbEngine(new SqlEngineProvider("connection string"));
    var username = "daniel";
    var fields = "username,password,memo";
    var command = engine.Parse($"SELECT {fields::} FROM Users WHERE Username = {username}");
    Assert.Equal("SELECT username,password,memo FROM Users WHERE Username = @p0", command.Text);
    Assert.Equal(1, command.Count);
    Assert.Equal("@p0", command[0].Name);
    Assert.Equal(username, command[0].Value);
```

### What's included

+ *Aoite*
    - **CommandModel** : A Simplified with Command CQRS(Query Responsibility Segregation) pattern.
    - **Data** : Easier operation to the database.
    - **Logger**
    - **Net**
    - **Redis**
    - **Reflection**
    - **Serialization**
+ *System*
