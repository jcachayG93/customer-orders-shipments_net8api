# Hello There
If you are learning C#, this project is an excellent follow-along example. I will be updating it regularly until it is complete.
What you see in this project is the result of several years of experimentation, R&D, reading, and experience.

Feel free to clone this project and experiment with it. Ask me questions by [Linkedin](https://www.linkedin.com/in/julio-cachay-2a159226b/)

# Plan

## Preparation (Project setup)
- [x] Create Solution with projects.
- [x] Add a controller with a Ping end-point.
- [x] Add some value objects.
- [x] Implement the Order agggregate.
- [x] Add the infrasturture layer with a postgres database and EF Core ORM.
- [x] Add integration testing (using an in memory database)

## Features (user stories)
- [x] feature 1: As a sales person, I need to be able to create sales orders for my customers.
- [ ] feature 2: As a sales person, I need to see a list of orders.
  
# CustomerOrders

This is a Demo project with two purposes:
1. Serve as a Professional portfolio project for Julio Cachay (owner of this repo)
2. Serve as a learning tool for people interested in learning C#.

## Getting started

Requirements:
- Net 8 SDK [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Docker [Download here](https://www.docker.com/products/docker-desktop/)
- EF Core CLI tools [More here](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)


### Start the database:
This application uses a development database that runs on Docker. To run, go to the
BackEnd folder (the folder contains a file named docker-compose.yml)

Make sure Docker desktop (or equivalent) is running.

> docker compose up -d

This will create a container named **customer_orders_db_x**, that is
a postgres database.

Each time you run this app in development mode, the database is recreated (all data is deleted),
this is by design.

### Run the app:
1. Clone the repository.
2. Navigate to: Backend/src/WebApi
3. Run (assuming you have the Dotnet SDK installed)

> dotnet run

4. Explore the API: [https://localhost:5118/swagger](https://localhost:5118/swagger)

## Run the tests.
This is a Test Driven Development project. To run the tests:
1. In the directory that contains the .sln file:

> dotnet test

## Overview
This explains what this project will be once it is completed.

### Stack

- Asp.Net core 8 API.

## Architecture & Design
This is a work in progress. I've shared here the goal for the final product.

### Stack
This project is an ASP.Net core 8 Web API with Controllers.

### Domain Driven Design
A guiding principle for this project is domain-driven design.

### Vertical Slice Architecture
This project will use vertical slice architecture, organizing the code by use case.

### Test Driven Development
Everything in this project is based on Test Driven Development:
1. We write definitions with no implementations.
2. We write the tests.
3. Run the tests, and they fail (Red)
4. Implement, and the tests pass (Green)
5. Look at the code, eliminate repetition, document, clean up (Refactor)

### Other Technologies
- MediatR
- PostgresDb
- Ef Core
- Docker
