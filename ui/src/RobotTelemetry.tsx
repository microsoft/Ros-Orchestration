// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import Position2d from './Position2d';
import RobotStatus from './RobotStatus';

class RobotTelemetry{
    public id : string;

    public createdDateTime : Date;

    public position : Position2d;

    public status : RobotStatus;

    public orderId : string;

    public robotId : string;
}

export default RobotTelemetry;