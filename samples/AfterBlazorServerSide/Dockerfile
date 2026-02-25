#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS=80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG VERSION=0.0.0
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["samples/AfterBlazorServerSide/AfterBlazorServerSide.csproj", "samples/AfterBlazorServerSide/"]
COPY ["src/BlazorWebFormsComponents/BlazorWebFormsComponents.csproj", "src/BlazorWebFormsComponents/"]
COPY ["samples/SharedSampleObjects/SharedSampleObjects.csproj", "samples/SharedSampleObjects/"]
RUN dotnet restore "samples/AfterBlazorServerSide/AfterBlazorServerSide.csproj"
COPY . .
# Remove NBGV - not functional in Docker without .git; version is injected via VERSION build arg
RUN sed -i '/<PackageReference Include="Nerdbank.GitVersioning"/,/<\/PackageReference>/d' Directory.Build.props
WORKDIR "/src/samples/AfterBlazorServerSide"
RUN dotnet build "AfterBlazorServerSide.csproj" -c Release -o /app/build -p:Version=$VERSION -p:InformationalVersion=$VERSION

FROM build AS publish
RUN dotnet publish "AfterBlazorServerSide.csproj" -c Release -o /app/publish -p:Version=$VERSION -p:InformationalVersion=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AfterBlazorServerSide.dll"]
