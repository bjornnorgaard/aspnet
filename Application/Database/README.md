# Database

## Scripts

### Setup CLI

View [the Microsoft CLI docs](https://learn.microsoft.com/en-us/ef/core/cli/dotnet#installing-the-tools) for more
information on installing the EF
Core tools.

```sh
# Ensure you have the EF Core tools installed
dotnet tool install --global dotnet-ef
```

```sh
# If you need to update the EF Core tools, run:
dotnet tool update --global dotnet-ef
```

### Create Migration

```sh
# Create a new migration called "Initial"
dotnet ef migrations add Initial --project ../Application.csproj --output-dir Database/Migrations
```