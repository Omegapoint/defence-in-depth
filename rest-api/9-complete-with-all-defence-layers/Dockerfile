# Build .NET application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Setup build space
WORKDIR /app
COPY . ./

# Build dll
RUN dotnet publish --output /app/out --configuration Release ./

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Copy build
WORKDIR /app
COPY --from=build-env /app/out .

# Demo 1 - Do not run as root user
RUN chown -R www-data:www-data /app
USER www-data

# Select non-root port
ENV ASPNETCORE_URLS=http://+:5000

# Launch dll
ENTRYPOINT ["dotnet", "9-complete-with-all-defence-layers.dll"]
