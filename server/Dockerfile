FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Bootstrap/Bootstrap.csproj Bootstrap/
COPY Libs/Presentation/Presentation.csproj Libs/Presentation/
COPY Libs/Application/Application.csproj Libs/Application/
COPY Libs/Domain/Domain.csproj Libs/Domain/
COPY Libs/Infrastructure/Infrastructure.csproj Libs/Infrastructure/

RUN dotnet restore Bootstrap/Bootstrap.csproj
RUN dotnet restore Libs/Presentation/Presentation.csproj
RUN dotnet restore Libs/Application/Application.csproj
RUN dotnet restore Libs/Domain/Domain.csproj
RUN dotnet restore Libs/Infrastructure/Infrastructure.csproj

COPY . .

WORKDIR /src/Bootstrap
RUN dotnet publish -c release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bootstrap.dll"]