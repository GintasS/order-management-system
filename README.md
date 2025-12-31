
# Order Management System

This is a restaurant order management platform designed to handle concurrent order processing, storage optimization, and delivery management. 

The system processes orders through multiple states 
(pre-processing, cooking, placement, and delivery) while managing temperature-controlled storage units (Heater, Cooler, and Shelf). 

- On the web app, real-time updates via SignalR keep you informed of every order status change, and the intelligent placement algorithm ensures orders are stored at optimal temperatures to maintain freshness and meet delivery time requirements.

- On the console app, you can see logs from different order states and threads.

## How does it work

1. Order comes in either from API or a Random generator.
2. We pre-process the order (set delivery time in seconds).
3. Cook the order. Current set-up: instantly.
4. Place the order in either heater, cooler or shelf based on the preffered temperature.
5. When delivery time is due, we either deliver the order or discard it (if order freshness is 0).

Now, if a preffered storage place for an order is full, we try to:
- place it on the shelf;
- discard an existing item on the shelf based on the discard criteria (more on it below);
- move an hot/cold item from shelf to a preffered location (if there's enough room);

## Order model

```csharp
 public string Id { get; set; } // Unique 5 digit id.

 public string Name { get; set; } // Dish name

 public OrderTypeEnum OrderType { get; set; } // Order type: Hot, Cold or Room

 public long Price { get; set; }

 public long Freshness { get; set; } // Freshness in seconds. If it's 0 when order is about to get delivered, we discard it.

 public int PickupTime { get; set; } // When the order is to be delivered.

 public OrderStoragePlaceEnum CurrentLocation { get; set; } // Where Order is stored (Heater, Cold, Shelf)
```

## Threads

In order to process orders in real-time, we are using threads.

|Thread name|Description|
|-----------|------------|
|ThreadFreshness|Removes freshness points from orders every 1 sec.|
|ThreadListSizes|Prints heater, cooler and shelf item counts.|
|ThreadPickupTime|Removes pickup time from orders every 1 sec.|
|Main thread|Processes the orders|.

### Order Discard criteria

We are receiving orders **every 500 ms** with varying delivery times **(4-8 seconds)**. <br>
We really don't want to discard orders that are about to get delivered (delivery time is close to 0), as we would lose:
  - the money for the order, 
  - the work of the kitchen, 
  - ingredients
  - storage space
  - processing time
  - delivery people's time & effort
  - etc.

Also, when it comes to user experience (a user has placed an order), we want to discard an order:
  - that has a long pickup time in order 

So we could inform the user soon and let the user order something from us again.

#### First Discard Strategy

Thus, my **first discard strategy** is to discard a single hot/cold order from the shelf with the highest pickup time.

#### Second Discard Strategy

My second discard strategy (if we can't execute the first strategy) is to discard a room temperature order with the highest pickup time.

#### How do I do it in O(1) time?

- I'm constantly saving a single hot/cold order (inside a shelf storage area) with highest pickup time inside `HotOrColdShelfItemsDictionary.cs`.
- I'm also saving a room temperature highest pickup time order.

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

## Discard criteria



So my discard criteria rationale is as follows:

- As we are getting orders rapidly, we mostly care about the order's delivery time to the user. We really don't want to discard orders that are about to get delivered (pickup time is close to 0), as we would lose:
    - the money for the order, 
    - the work of the kitchen, 
    - ingredients
    - storage space
    - processing time
    - delivery people's time & effort
    - etc.

- Also, when it comes to user experience (a user has placed an order), we want to discard an order that has a long pickup time in order to inform the user soon and let the user order something from us again.

Thus, my **first discard strategy** is to discard a single hot/cold order from the shelf with the highest pickup time.

My **second discard strategy** (if we can't execute the first strategy) is to discard a room temperature order with the highest pickup time.

I'm constantly saving a single hot/cold order with highest pickup time inside `HotOrColdShelfItemsDictionary.cs` for the O(1) discard, as well as room temperature order for O(1) discard.



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

