# EasyCoreAPI

## Overview
The EasyCoreAPI SDK simplifies the setup of Swagger documentation, API versioning, and controller configurations in ASP.NET Core applications. It provides a seamless way to integrate modern API practices with minimal effort.

## Installation

1. Add the EasyCoreAPI SDK to your project using NuGet:
```bash
dotnet add package EasyCoreAPI
```

2. Include the namespace in your `Program.cs` or `Startup.cs` file:
```csharp
using EasyCoreAPI;
```

## Features.
- Enable annotations for better documentation.
- Configure API versioning with Swagger UI.
- Configure security definitions for APIs with token-based authentication.
- Automatically resolves XML comments from project files.


## Usage


## Requirements
- .NET 6.0 or later
- Compatible with ASP.NET Core applications.

### Add SwaggerGen
```csharp
services.AddSwaggerGen(Configuration, AuthScheme.Bearer");
```

### Add API Versioning
```csharp
services.AddApiVersioning(1);
```
### Add API Controllers
```csharp
services.AddApiControllers();
```

### Use SwaggerUI in WebApplication
```csharp
app.UseSwaggerUI();
```


## Contributing
1. Fork the repository.
2. Create a new branch.
3. Make your changes.
4. Commit your changes.
5. Push your changes to your fork.
6. Submit a pull request.


## License
This project is licensed under the MIT License.


## Support

For any questions or issues,
please open an issue on GitHub or
contact us at <a href="mailto:developer.biliksuun@gmail.com">
developer.biliksuun@gmail.com</a>.

## Authors
- Samuel Biliksuun

