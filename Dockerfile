﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM node:alpine as build-frontend
WORKDIR /src/frontend
COPY /src/Web/Client .
# RUN ls
WORKDIR /src/frontend
# RUN ls
RUN npm install -g pnpm
RUN pnpm install
RUN pnpm build
# RUN ls

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-service
ARG BUILD_CONFIGURATION=Release

FROM build-service AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/server
COPY /src/Web .
RUN dotnet publish "Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN ls

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build-frontend /src/frontend/dist ./wwwroot
ENTRYPOINT ["dotnet", "Web.dll"]
