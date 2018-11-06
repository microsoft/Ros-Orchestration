import axios, {AxiosError, AxiosResponse} from 'axios';
import Configuration from './Configuration';
import Order from './Order';

export class OrderManagerClient
{
    public response : Order[];

    private uri : string

    constructor()
    {
        const orderManagerUrl = Configuration.orderManagerUrl;
        const orderManagerVersion = Configuration.orderManagerVersion;

        this.uri = "https://" + orderManagerUrl + "/api/v" + orderManagerVersion + "/Orders";
    }

    public async getOrdersAsync() {
        await axios.get(this.uri)
          .then(this.handleResponse).catch(this.handleError);
    }

    private handleResponse = (response : AxiosResponse<any>) => {
        this.response = response.data as Order[];
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

export default OrderManagerClient;