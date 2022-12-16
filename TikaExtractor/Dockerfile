FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY /bin/Release/net7.0/linux-x64/publish .
ENTRYPOINT ["./TikaExtractor"]

#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
#WORKDIR /src
#COPY ["TikaExtractor.csproj", ""]
#RUN dotnet restore "TikaExtractor.csproj"
#COPY . .
#RUN dotnet build "TikaExtractor.csproj" -c Release -o /app/build

#FROM build AS publish
#RUN dotnet publish "TikaExtractor.csproj" -c Release -o /app/publish

#FROM mcr.microsoft.com/dotnet/runtime:7.0 AS final
#WORKDIR /app
#COPY --from=publish ./app/publish .
#ENTRYPOINT ["dotnet", "TikaExtractor"]


