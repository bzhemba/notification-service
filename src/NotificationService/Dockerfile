FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NotificationService/NotificationService.csproj", "src/NotificationService/"]
COPY ["src/Application/NotificationService.Application/NotificationService.Application.csproj", "src/Application/NotificationService.Application/"]
COPY ["src/Application/NotificationService.Application.Abstractions/NotificationService.Application.Abstractions.csproj", "src/Application/NotificationService.Application.Abstractions/"]
COPY ["src/Application/NotificationService.Application.Models/NotificationService.Application.Models.csproj", "src/Application/NotificationService.Application.Models/"]
COPY ["src/Application/NotificationService.Application.Contracts/NotificationService.Application.Contracts.csproj", "src/Application/NotificationService.Application.Contracts/"]
COPY ["src/Infrastructure/NotificationService.Infrastructure.Persistence/NotificationService.Infrastructure.Persistence.csproj", "src/Infrastructure/NotificationService.Infrastructure.Persistence/"]
COPY ["src/Presentation/NotificationService.Presentation.Grpc/NotificationService.Presentation.Grpc.csproj", "src/Presentation/NotificationService.Presentation.Grpc/"]
COPY ["src/Presentation/NotificationService.Presentation.Http/NotificationService.Presentation.Http.csproj", "src/Presentation/NotificationService.Presentation.Http/"]
COPY ["src/Presentation/NotificationService.Presentation.Kafka/NotificationService.Presentation.Kafka.csproj", "src/Presentation/NotificationService.Presentation.Kafka/"]
RUN dotnet restore "src/NotificationService/NotificationService.csproj"
COPY . .
WORKDIR "/src/src/NotificationService"
RUN dotnet build "NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]
