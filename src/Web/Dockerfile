#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-service
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["Web.csproj", "src/Web/"]
#RUN dotnet restore "src/Web/Web.csproj"
#COPY . .
##WORKDIR "/src/src/Web/Client"
##RUN npm install
##RUN npm build
#WORKDIR "/src/src/Web"
#RUN dotnet build "Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM node:alpine as build-frontend

#FROM node as build-frontend
WORKDIR /src
RUN pwd && ls
COPY ["Client/", "/src/client/"]
WORKDIR /src/client
RUN ls
WORKDIR /src/client
RUN npm install
RUN npm build
#
## copy our frontend into published app's wwwroot folder
##FROM builder-service AS publisher
##COPY --from=builder-frontend /client/build /app/wwwroot/
#
#FROM build-service AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Web.dll"]
