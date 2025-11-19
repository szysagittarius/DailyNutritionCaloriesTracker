dotnet tool update --global dotnet-ef
dotnet ef migrations add InitialCreate --project ..\DNCT.Database\NT.Database.csproj

dotnet ef database update --project ..\DNCT.Database\NT.Ef.Database.csproj