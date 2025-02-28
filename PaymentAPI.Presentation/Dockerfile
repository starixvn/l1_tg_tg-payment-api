FROM hub.lumigame.com/base/dotnet/core/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["PaymentAPI.Presentation/PaymentAPI.Presentation.csproj", "PaymentAPI.Presentation/"]
RUN dotnet restore "./PaymentAPI.Presentation/PaymentAPI.Presentation.csproj"
COPY . .
WORKDIR "/src/PaymentAPI.Presentation"
RUN dotnet publish "PaymentAPI.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:9.0-alpine
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentAPI.Presentation.dll"]