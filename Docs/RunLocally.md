# Run Locally

## Pre-requisites

While it is possible to run certain parts of the project locally for debugging, the orchestrator requires Azure resources, such as Key Vault, IotHub, EventHub, and Azure Storage. Please follow the steps in the [Provisioning](./Provisioning.md) before attempting to run any component locally.

## Web APIs

Open the [RobotOrchestrator.sln](./../RobotOrchestrator.sln) in Visual Studio, and run build. Then set **Multiple Start Up Projects**:

- RobotOrchestrator.OrderProducer
- RobotOrchestrator.FleetManager
- RobotOrchestrator.OrderManager
- RobotOrchestrator.Dispatcher

Add an **appsettings.Local.json** file to each of the project folders above, with the following content:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "Vault": "<YOUR KEY VAULT NAME>", //e.g. robot-orch-kv-dev
  "ClientId": "<YOUR CLIENT ID>",
  "ClientSecret": "<YOUR CLIENT SECRET>"
}
```

Reference the **Client Id** and **Client Secret** you created in [Provisioning](./Provisioning.md). This will pull various secrets needed to run your application, such as IotHub Connection Strings, EventHub ConnectionStrings, Storage ConnectionStrings, and more.

## UI

- Run the following

```bash
    cd ui
    npm install
    npm run build
    npm run start
```

A window will open with the UI running locally. By default it will point to your local web api services. See section above on running [web apis locally](#Web-APIs).

To use azure services instead, set the corresponding environment variables or replace the strings ("localhost:44301", "localhost:44307") in [../ui/public/configuration.json](../ui/public/configuration.json) with the URLs for the services in the Azure Portal for the fleet manager and order manager. Please note, you have to remove "https://" in the URLs and get the URLs like "fleetmanager.azurewebsites.<i></i>net" and "ordermanager.azurewebsites.<i></i>net".

### Debug UI Locally

- For debugging the UI locally we recommend following the steps laid out at [this link](https://code.visualstudio.com/docs/nodejs/reactjs-tutorial#_debugging-react).

## Simulator and Robots

Follow the steps in the Ros-Simulation repo for building the `robot` and `simulator` images locally. They will serve as the base images for the orchestrator project.

To run locally, run the following command from the root directory of the project:

```bash
docker-compose up -d
```

This will build the orchestrator images robot-orch and simulator-orch the first time the command is run on your environment. To force a rebuild of the orchestrator images, run:

```bash
docker-compose build
```

A simulator and two robots will be created, hackbot0 and hackbot1.

To see/follow the logs for each container, run:

```bash
docker-compose logs -f simulator
docker-compose logs -f robot-0
docker-compose logs -f robot-1
```

To execute commands in the containers, run:

```bash
docker-compose exec simulator /bin/bash
docker-compose exec robot-0 /bin/bash
docker-compose exec robot-1 /bin/bash
```