import * as React from 'react';
import Configuration from './Configuration';
import Position2d from './Position2d';
import RobotStatus from './RobotStatus';
import SimpleCanvas from './SimpleCanvas';

class RobotMap extends React.Component <any, any>{
    
    private maxPosition : Position2d;

    private canvasWidth : number;
    private canvasHeight : number;

    private mapWidth : number;
    private mapHeight : number;

    private simpleCanvas : SimpleCanvas;

    constructor(props: any, context: any){
        super(props, context);

        this.canvasWidth = +Configuration.canvasWidth;
        this.canvasHeight = +Configuration.canvasHeight;

        this.mapWidth = +Configuration.mapWidth;
        this.mapHeight = +Configuration.mapHeight;
        
        this.maxPosition = new Position2d(this.mapWidth, this.mapHeight);
    }

    public async drawMap() {
        if(this.simpleCanvas == null)
        {
            this.simpleCanvas = new SimpleCanvas("Map");
        }

        this.simpleCanvas.clear();
            
        await this.simpleCanvas.drawImage("map-simple.png", 0, 0, this.canvasWidth, this.canvasHeight, 0.1);

        for(const robot of this.props.robots){
            const positionInCanvas = this.translate(robot.telemetry.position, this.maxPosition, this.canvasWidth, this.canvasHeight);

            let circleRadius = 4;

            if (robot.telemetry.robotId === this.props.activeRobot) {
                circleRadius = 6;
            }

            this.simpleCanvas.drawCircle(positionInCanvas.x, positionInCanvas.y, circleRadius, this.getColor(robot.telemetry.status));
        }
    }

    public componentDidMount() {
        this.drawMap();
    }

    public componentDidUpdate() {
        this.drawMap();
    }

    public render() {
        return  (
            <div>
                <canvas id="Map" width="600" height="500"/>
            </div>
        );
    }

    private getColor(robotStatus : string) : string{
        switch(robotStatus){
            case RobotStatus[RobotStatus.Onboarding]:{
                return "gold";
            }
            case RobotStatus[RobotStatus.Idle]:{
                return "green";
            }
            case RobotStatus[RobotStatus.Busy]:{
                return "red";
            }
            case RobotStatus[RobotStatus.Failed]:{
                return "gray";
            }
        }
        
        return "black";
    }

    private translate(position : Position2d, maxPosition : Position2d, canvasWidth : number, canvasHeight : number) : Position2d {
        
        const positionInCanvas = new Position2d(0, 0);

        positionInCanvas.x = 1.0 * position.x / maxPosition.x * canvasWidth;

        positionInCanvas.y = 1.0 * position.y / maxPosition.y * canvasHeight;

        return positionInCanvas;
    }
    
}

export default RobotMap;