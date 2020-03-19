#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["samples/AfterBlazorServerSide/AfterBlazorServerSide.csproj", "samples/AfterBlazorServerSide/"]
COPY ["src/BlazorWebFormsComponents/BlazorWebFormsComponents.csproj", "src/BlazorWebFormsComponents/"]
COPY ["samples/SharedSampleObjects/SharedSampleObjects.csproj", "samples/SharedSampleObjects/"]
RUN dotnet restore "samples/AfterBlazorServerSide/AfterBlazorServerSide.csproj"
COPY . .
WORKDIR "/src/samples/AfterBlazorServerSide"
RUN dotnet build "AfterBlazorServerSide.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AfterBlazorServerSide.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AfterBlazorServerSide.dll"]