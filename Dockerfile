FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY HospitalAPI.csproj ./
RUN dotnet restore HospitalAPI.csproj

COPY . .
RUN dotnet publish HospitalAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "HospitalAPI.dll" ]