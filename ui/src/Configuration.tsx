// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import axios, {AxiosError, AxiosResponse} from 'axios';

class Configuration
{
    public static fleetManagerUrl :  string = "localhost:44301";
    public static fleetManagerVersion : string = "1";

    public static orderManagerUrl : string = "localhost:44307";
    public static orderManagerVersion : string = "1";

    public static canvasWidth : string = "600";
    public static canvasHeight : string = "500";

    public static mapWidth : string = "63.9000";
    public static mapHeight : string = "56.3000";

    public static async initialize()
    {
        await axios.get("configuration.json").then(this.handleJsonFileResponse).catch(this.handleError);
    }

    public static handleJsonFileResponse = (response : AxiosResponse<any>) =>
    {
        Configuration.jsonResponse = response.data as Map<string, string>;

        const fleetManagerUrlKey = "REACT_APP_FLEETMANAGER_URL";
        const orderManagerUrlKey = "REACT_APP_ORDERMANAGER_URL";

        if (Configuration.jsonResponse == null)
        {
            return;
        }

        const fleetManagerUrl = Configuration.jsonResponse[fleetManagerUrlKey];
        const orderManagerUrl = Configuration.jsonResponse[orderManagerUrlKey];

        if (fleetManagerUrl != null)
        {
            Configuration.fleetManagerUrl = Configuration.jsonResponse[fleetManagerUrlKey];
            console.log("fleet manager url: " + Configuration.fleetManagerUrl);
        }

        if (orderManagerUrl != null)
        {
            Configuration.orderManagerUrl = Configuration.jsonResponse[orderManagerUrlKey];
            console.log("order manager url: " + Configuration.orderManagerUrl);
        }
    }

    private static jsonResponse: Map<string, string>;

    private static handleError = (error : AxiosError) => {
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

export default Configuration;