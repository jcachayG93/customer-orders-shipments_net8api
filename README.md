# CustomerOrders

This is a Demo project with two purposes:
1. Serve as a Professional portfolio project for Julio Cachay (owner of this repo)
2. Serve as a learning tool for people interested in learning C#.

## Getting started

Requirements:
- Net 8 SDK [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

To run:
1. Clone the repository.
2. Navigate to: src/WebApi
3. Run (assuming you have the Dotnet SDK installed)

> dotnet run

4. Explore the api: [https://localhost:5118/swagger](https://localhost:5118/swagger)

## Run the tests.
This is a Test Driven Development project. To run the tests:
1. Go to the root of the repository (where the sln file is located)

> dotnet test

## Overview
This is an explanation on what this project will be once it is completed.

### Stack

- Asp.Net core 8 API.

## Architecture & Design
This is a work in progress, what is outlined here is the goal for the final product.

### Stack
This project is an ASP.Net core 8 Web Api with Controllers.

### Domain Driven Design
A guiding principle for this project is Domain Driven Design.

### Vertical Slice Architecture
This project will use vertical slice architecture, organizing the code by use case.

### Test Driven Development
Everything in this project is based on Test Driven Development:
1. We write definitions with no implementations.
2. We write the tests.
3. Run the tests and they fail (Red)
4. Implement and the tests pass (Green)
5. Look at the code, eliminate repetition, document, clean up (Refactor)

### Other Technologies
- MediatR
- PostgresDb
- Docker