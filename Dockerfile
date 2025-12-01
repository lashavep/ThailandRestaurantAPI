# 1) Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# 2) Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY RestaurantAPI/RestaurantAPI.csproj RestaurantAPI/
RUN dotnet restore RestaurantAPI/RestaurantAPI.csproj
COPY . .
RUN dotnet publish RestaurantAPI/RestaurantAPI.csproj -c Release -o /out

# 3) Final image
FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "RestaurantAPI.dll"]
