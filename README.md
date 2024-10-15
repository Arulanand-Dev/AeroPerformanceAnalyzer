# Aero Performance Analyzer Service

Aero Performance Analyzer Service is a .NET 8 application designed to process race car telemetry data for performance analysis. This service provides an API to analyze telemetry data, detect specific conditions, and visualize the results.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [API Documentation](#api-documentation)

## Features

- Process and analyze telemetry data from multiple channels.
- Calculate new telemetry channels based on input data.
- Identify and visualize conditions met during telemetry analysis.
- API for interacting with the telemetry data.

## Prerequisites

Before you begin, ensure you have the following installed:

- .NET 8 SDK: [Download .NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio: [Download Visual Studio](https://visualstudio.microsoft.com/vs/) or any preferred code editor
- Git: [Download Git](https://git-scm.com/) (optional, for cloning the repository)

## Installation

1. Clone the repository:
   
   `git clone https://github.com/Arulanand-Dev/AeroPerformanceAnalyzerService.git`

2. Navigate to the project directory:
   
   `cd AeroPerformanceAnalyzerService`

3. Restore the project dependencies:
   
   `dotnet restore`

## Running the Application

1. To run the application in development mode, execute the following command:
   
   `dotnet run`

2. Once the application is running, it will be accessible at `https://localhost:44308`.

3. Open a web browser and navigate to `https://localhost:44308/swagger/index.html` to access the Swagger API documentation.

## API Documentation

- The API documentation can be accessed via Swagger UI at the following URL:

   [Swagger API Documentation](https://aeroperformanceanlayzerservice-d2bretahcsexckgf.canadacentral-01.azurewebsites.net/swagger/index.html)
