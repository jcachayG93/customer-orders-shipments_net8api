# Hello There
If you are learning C#, this project is an excellent follow-along example. I will be updating it regularly until it is complete.
What you see in this project is the result of several years of experimentation, R&D, reading, and experience.

Feel free to clone this project and experiment with it. Ask me questions by [Linkedin](https://www.linkedin.com/in/julio-cachay-2a159226b/)

# Planning, progress and performance.

## Preparation (Project setup)
- [x] Create Solution with projects (1hr)
- [x] Add a controller with a Ping end-point (5 minutes)
- [x] Add some value objects (1 hr)
- [x] Implement the Order aggregate (1 hr)
- [x] Add the infrasturture layer with a postgres database and EF Core ORM (2 hrs)
- [x] Add integration testing (using an in memory database) (1 hr) <-- About 6 hours so far.

## Features (user stories)
- [x] feature 1: As a sales person, I want to create sales orders, so I can send them to my customers (30 min)
- [x] feature 2: As a sales person, I want to see a list of sales order (30 min)
- [x] feature 3: As a sales person, I want to modify orders so I can attend my customer requests (2 hrs)
- [x] feature 4: Add error handling, with a centralized global exception handler that returns problem details (30 min)
- [x] feature 5: As a sales person, I want to mark an order as ordered so It can be shipped. (30 min)
- [x] feature 6: As a sales person, I want a packing list to be automatically created when I mark an order as ordered. (3 hrs)
- [ ] feature 7: As a sales person, I want the order be be freezed once ordered, so I know it wont change after I sent it to a customer.
- [ ] feature 8: As a shipping manager, I need to see the contents of the packing lists.
- [ ] feature 9: Create UI
- [ ] feature 10: Create design system
- [ ] feature 11: Connect UI and Back-end
- [ ] feature 12: Create Orders screen
- [ ] feature 13: Create Shipments screen
- [ ] deploy   
  
# CustomerOrders

This is a Demo project with two purposes:
1. Serve as a Professional portfolio project for Julio Cachay (owner of this repo)
2. Serve as a learning tool for people interested in learning C#.

## Getting started

Requirements:
- Net 8 SDK [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Docker [Download here](https://www.docker.com/products/docker-desktop/)
- EF Core CLI tools [More here](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

## What this App does?
1. The Sales Person creates a Sales Order, and see the existing Sales Orders.
3. The Sales Person can add/remove/update lines for the Order.
4. The Sales Person can mark a SalesOrder as ordered, which freezes it so nobody can change it again (for now)
5. When the Sales Person marks the SalesOrder as ordered, a Packing List is created (reflecting the contents of the order)
6. The Shipping Manager can see the Packing List.
   
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
#### Domain Events
When the SalesOrder is marked as Ordered, a domain event (OrderOrdered) is dispatched:
- The Handler creates a Packing list.
- This operation is Idempotent.
- This operation is transactional (if creating the Packing List fails, the Sales Order is not marked as Ordered)

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
- Fluent validation
