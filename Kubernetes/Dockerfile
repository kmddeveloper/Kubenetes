#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Kubernetes/Web.csproj", "Kubernetes/"]
COPY ["TransferObjects/TransferObjects.csproj", "TransferObjects/"]
COPY ["DataReaderMapper/DataReaderMapper.csproj", "DataReaderMapper/"]
COPY ["AppConfiguration/AppConfiguration.csproj", "AppConfiguration/"]
COPY ["Business/Business.csproj", "Business/"]
COPY ["Repository/Repository.csproj", "Repository/"]
RUN dotnet restore "Kubernetes/Web.csproj"
COPY . .
WORKDIR "/src/Kubernetes"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

ENV ASPNETCORE_URLS=http://+:5000
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]
