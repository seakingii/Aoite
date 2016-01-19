# Aoite [![NuGet](https://img.shields.io/nuget/v/Aoite.svg)](https://www.nuget.org/packages/Aoite) [![NuGet](https://img.shields.io/nuget/dt/Aoite.svg)](https://www.nuget.org/packages/Aoite/)

**Aoite 是一个 .Net Framework 的扩展库，利用 Aoite 可以更快、更便捷、更轻松的开发任何 .Net Framework 项目。**

[文档](https://treenew.gitbooks.io/aoite/content/)

## Aoite 定位

Aoite 并非是一个专注只某一个领域的库（如 `ServiceStack.Redis` 专注于 Redis 的客户端），它专注成为**`.Net Framework 的扩展库`**，无论是B/S项目、C/S项目、三层架构、领域模式、ASP.NET WebForms 或 MVC 等等，都可以轻松的运用它。其次，Aoite 具备非常强大的扩展性，可以解决各种特殊的业务场景。

它核心功能有：

- 简化与数据库的交互操作模块（支持`async`/`await`）
- 智能化 Ioc/DI 模块
- 可异步、可扩展、可测试的简化版 CQRS 模式
- 高性能的反射模块
- 高性能的二进制序列化
- 通用版 Redis 客户端
- 内置可扩展的日志模块
- 常用功能扩展

## Aoite 安装

安装`最新、稳定`版本的 Aoite 可以通过 Visual Studio 的【程序包管理器控制台】：


    PM> Install-Package Aoite

关于 nuget.org 上的 Aoite，通过 Nuget 搜索 _Aoite_ 或访问 https://nuget.org/packages/Aoite 了解更多信息。

## Aoite 编译

想要获取最新的源码可以到 [GitHub](https://github.com/treenew/Aoite) 下载。

Aoite 支持 .Net Framework 4.0 以及更高的版本，编译 Aoite 需要 Visual Studio 2015。打开 Aoite.sln 后通过右键解决方案，单击【配置管理器】，在弹出界面可根据实际需要编译你需要的版本。需要注意的是 `Aoite.Tests` 只能在 `DEBUG` 下编译。

各个版本之间有什么重大的不同？

+ NET40 不支持数据库交互的异步方法（如 `ToEntitiesAsync`）
+ NET40 和 NET45 都不支持 `FormattableString` 的 SQL 生成转换（如`$"SELECT name FROM Users WHERE Username = {username}"`）。

## 示例

Aoite 源码中包含一个简单的示例，可以访问  [Aoite.Samples](https://github.com/treenew/Aoite/tree/master/tests/Aoite.Samples) 了解。


# Aoite English

**Aoite is a library that makes it quicker and easier to any .Net Framework projects.**

[Documentation](https://treenew.gitbooks.io/aoite/content/)

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

### Quick start with CommandModel

Please see [Aoite.Samples](https://github.com/treenew/Aoite/tree/master/tests/Aoite.Samples) project.

## What's included

+ `Aoite`
    - `CommandModel` : A Simplified with Command CQRS(Query Responsibility Segregation) pattern.
    - `Data` : Easier operation to the database.
    - `Logger`: A Simplified log provider.
    - `Net`
    - `Redis`: A Simplified redis client.
    - `Reflection`: Easier to use .net reflection.
    - `Serialization`: Easier to use .net serialization.
+ `System`
    - `Core`
        - `Ajob`: Async job.
        - `BinaryValue`:Convert any object to byte array.
        - `ConsistentHash`1`
        - `DataSecurity`: Data security.
        - `FastRandom`: Fast random.
        - `GA`:
        - `ObjectPool`1`: Object pool base.
        - `Types`
        - `WhereParameters`
    - `Extensions`
    - `Ioc`
    - `Mapping`
    - `Result`
    - `Web`
