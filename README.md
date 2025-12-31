
# Order Management System

This is a restaurant order management platform designed to handle concurrent order processing, storage optimization, and delivery management. 

The system processes orders through multiple states 
(pre-processing, cooking, placement, and delivery) while managing temperature-controlled storage units (Heater, Cooler, and Shelf). 

- On the web app, real-time updates via SignalR keep you informed of every order status change, and the intelligent placement algorithm ensures orders are stored at optimal temperatures to maintain freshness and meet delivery time requirements.

- On the console app, you can see logs from different order states and threads.




## Features

- Real-time processing
- Random Order generation
- Unit Tests and Integration tests
- Web and Console interfaces
- Serilog logging for all order states




## Order states explained

I use order state pattern from [here](https://refactoring.guru/design-patterns/state/csharp/example).

- Pre-process Order State - performs initial order processing (sets delivery time).
- Cook Order State - cooks the order.
- Place Order State - places the order on the heater, cooler or shelf.
- Move Order State - moves order from shelf to heater/cooler and/or discards an existing order to make space.
- Pick Order State - delivers an order.
- Discard Order State - removes an order.


## Run Locally

You can run the .NET solution in a couple of ways:

### Run Locally (Console) - dotnet CLI

If dotnet `10.0` locally installed, run the program directly for convenience.

1. cd into `src` directory
```
cd src
```

2. Run the project using dotnet CLI.
```
dotnet run --project CSS.Challenge.Application
```

### Run Locally (Console) - Visual Studio

1. Open `CSS.Challenge.slnx` solution file.
2. Right click on `CSS.Challenge.Application` and click `Set as Startup Project`.
3. Run the project `CSS.Challenge.Application` couple of times to get different test cases.

### Run Locally (Web app) - Visual Studio

1. Open `CSS.Challenge.slnx` solution file.
2. Right click on `CSS.Challenge.Web` and click `Set as Startup Project`.
3. Run the project `CSS.Challenge.Web`.

### Run Locally (Web app) - Docker

The `Dockerfile` defines a self-contained C# reference environment.

1. cd into `src` directory
```
cd src
```

2. Make sure the Docker is running.

3. Build the docker image.
```
docker build -t challenge .
```

4. Run the docker container.
```
$ docker run -p "3001:8080" challenge
```


## Running Tests

To run tests, run the following command using dotnet CLI:

```bash
  dotnet test
```

