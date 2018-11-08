// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as React from 'react';
import { Panel }  from 'react-bootstrap';

class RobotInfoCollapse extends React.Component <any, any>{

    public render() {
        return (
            <Panel eventKey={this.props.robot.id}>
                <Panel.Heading>
                    <Panel.Title toggle={true}>{this.props.robot.id}</Panel.Title>
                </Panel.Heading>
                <Panel.Body collapsible={true}>
                    <div> Telemetry id:  {this.props.robot.telemetry.id} </div>
                    <div> X:  {this.props.robot.telemetry.position.x.toFixed(3)} </div>
                    <div> Y:  {this.props.robot.telemetry.position.y.toFixed(3)} </div>
                    <div> Status:  {this.props.robot.telemetry.status} </div>
                    <div> Order:  {this.props.robot.telemetry.orderId} </div>
                </Panel.Body>
            </Panel>
        );
    }
}

export default RobotInfoCollapse;