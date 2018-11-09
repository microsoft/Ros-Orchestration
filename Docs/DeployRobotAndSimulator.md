# Run Robot And Simulator

## Create Cluster

See the **Ros-Simulation** docs/ClusterCreation_ACS-Engine.md and docs/ClusterCreation_AKS.md for creating an ACS-Engine or AKS cluster in Azure respectively.

Make sure to follow the steps in docs/RunSimulationEndToEnd.md for installing the `image-secret` and tainting the dedicated simulator node.

## Build and Push Containers

See the **Ros-Simulation** docs/ContainerManagement.md for steps to build the base docker images of ```simulator``` and ```robot```. Based on these two docker images, build the corresponding ```simulator``` and ```robot``` docker images for **Ros-Orchestration** from [Dockerfile-simulator](../Robot/Dockerfile-simulator) and [Dockerfile-robot](../Robot/Dockerfile-robot), respectively. Please note that you may need to change the base image names behind **From** in the two Dockerfiles.

```console
    # Move to the Robot folder
> cd Robot
    # Build the ARGoS Simulator image for Ros-Orchestrator
> docker build -t simulator-orch . -f "Dockerfile-simulator"
    # Build the Robot image for Ros-Orchestrator
> docker build -t robot-orch . -f "Dockerfile-robot"
```

Once you have build the Docker images for the Orchestration, push the docker images to the Azure Container Registry (ACR) created in the [provisioning](./Provisioning.md) steps. For more information on how to login to ACR, tag, and push images, see [ACR Documentation](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-docker-cli)

## Install Helm Chart

See the **Ros-Simulation** docs/RunSimulationEndToEnd.md for deploying simulation on your cluster and verifying the deployment. In the [values.yaml](../helm/ros-orchestrator/values.yaml), there are several parameters could be adjusted for your deployment. Please note that **fleetManagerUrl** and **fleetManagerVersion** are used to allow robots connect to Azure IOT hub, therefore, make sure they are correct in your deployment.

| Property                  | Description       | Default                                                                 |
|---------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------|
| robotPrefix               | Prefix string used as the ros namespace for the robot on the simulator node                                                                                                                                          | robot                                                                   |
| **robotCount**                | Number of robot pods that will be provisioned in Kubernetes and in the simulator                                                                                                                                     | 2                                                                       |
| **simulatorThreadCount**      | Number of threads used to run simulator. Our recommendation is to set this equal to the number of cores for the simulator node                                                                                            | 4                                                                       |
| registrySecret            | Name of the Kubernetes secret used to store credentials to the Azure Container Registry                                                                                                                              | image-secret                                                            |
| robotTargetPrefix         | Prefix string used as the Kubernetes service name for the robot nimbro service. An index will be appended to the prefix (e.g. robot-nimbro-0).                                                                       | robot-nimbro-                                                           |
| simulatorTargetPrefix     | Prefix string used as the Kubernetes service name for the simulator nimbro service. An index will be appended to the prefix (e.g. simulator-nimbro-0). The index corresponds to the robot index.                     | simulator-nimbro-                                                       |
| portBase                  | Base port used for the nimbro ports. For all robots, this base port is used for nimbro receivers. On the simulator, the base port is the first port open, and successive robot receiver ports are added as 17000 + i | 17000                                                                   |
| nimbroProtocol            | Protocol used for Nimbro. Valid values are "tcp" or "udp".                                                                                                                                                           | tcp                                                                     |
| simulationCallbackTimeout | Simulator will wait at most this timeout period for ros twist messages from the robot.                                                                                                                               | 0.1 (seconds)                                                           |
| vncPassword               | Password to use for the vncserver                                                                                                                                                                                    | vncpassword                                                             |
| **fleetManagerUrl**               | The URL of fleet manager service. **Please make sure this matches your deployment**                                                                                                                                                                                  | contoso-fleetmanager.azurewebsites.net                                                             |
| **fleetManagerVersion**               | The API version of fleet manager service. **Please make sure this matches your deployment**                                                                                                                                                                                 | 1                                                             |
| **simulatorImage**            | Repository, tag, and pullPolicy for the simulator image.                                                                                                                                                             | repository: contoso.azurecr.io/simulator-orch tag: latest pullPolicy: Always |
| **robotImage**                | Repository, tag, and pullPolicy for the robot image.                                                                                                                                                                 | repository: contoso.azurecr.io/robot-orch tag: latest pullPolicy: Always     |
| resources                 | CPU and memory resource requests and limits for the robot containers. Robots need at least 1 CPU in most cases.                                                                                                      | limits:   cpu: 1   memory: 1024Mi requests:   cpu: 1   memory: 1024Mi   |


### Install Helm on the Cluster

Run the following command:

```> helm init --upgrade```

### Install Helm Release

To create a release, from the root folder run

```helm install helm/ros-orchestrator/```

## Upgrade Helm Release (if needed)

In the case where an image tag changes, the robot count changes, you will need upgrade the release. To do this without deleting and re-releasing, upgrade from root folder using:

```console

# Get release name
> helm list
# Use release name to upgrade
> helm upgrade <name> helm/ros-orchestrator/

```

## Delete Release (if needed)

To delete the release, you will need its name

```console

> helm list
> helm delete <name> --purge
```

Using '--purge' for deletion will remove the release name from your history, so that it can be used for another helm install (otherwise, it is kept around for rollbacks). If you do not use the purge option, and you are naming the releases, then you may run into conflicts when installing at a later time.

Ensure all nodes are terminated before creating re-installing the chart
