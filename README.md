## Getting Started with Aoite

**Aoite (currently v3.0.16) is a library that makes it quicker and easier to any .Net Framework projects.**

### Install with Nuget

    install-package aoite

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

## What's included

+ *Aoite*
    - **CommandModel** : A Simplified with Command CQRS(Query Responsibility Segregation) pattern.
    - **Data** : Easier operation to the database.
    - **Logger**
    - **Net**
    - **Redis**
    - **Reflection**
    - **Serialization**
+ *System*
