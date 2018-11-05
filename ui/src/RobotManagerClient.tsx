import axios, {AxiosError, AxiosResponse} from 'axios';
import Configuration from './Configuration';
import Robot from './Robot';

export class FleetManagerClient
{
    public responseRobots : Robot[];

    private uriRobots : string;

    constructor()
    {
        const fleetManagerUrl = Configuration.fleetManagerUrl;
        const fleetManagerVersion = Configuration.fleetManagerVersion;
        this.uriRobots = "https://" + fleetManagerUrl + "/api/v" + fleetManagerVersion + "/Robots";
    }

    public async getRobotsAsync() {
        await axios.get(this.uriRobots)
          .then(this.handleResponseOfRobots).catch(this.handleError);
    }

    private handleResponseOfRobots = (response : AxiosResponse<any>) => {        
        this.responseRobots = response.data as Robot[];
    }

    private handleError = (error : AxiosError) => {
        if(error.response)
        {
            console.log(error.response.data);
            console.log(error.response.status);
            console.log(error.response.headers);
        }
        else{
            console.log(error.message);
        }
    }
}

export default FleetManagerClient;