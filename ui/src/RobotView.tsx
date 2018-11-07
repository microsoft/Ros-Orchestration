import * as React from 'react';
import {Col, Grid, Row} from 'react-bootstrap';
import Configuration from './Configuration';
import Map from './Map';
import Robot from './Robot';
import RobotInfoPanel from './RobotInfoPanel';
import RobotManagerClient from './RobotManagerClient';

class RobotView extends React.Component <any, any>{

    private static refreshInMs : number = +Configuration.refreshInMs;

    private robotManagerClient : RobotManagerClient;

    private interval : NodeJS.Timer;

    private isCancelled : boolean;

    constructor(props: any, context: any) {
        super(props, context);

        this.isCancelled = false;

        this.robotManagerClient = new RobotManagerClient();

        this.onRefreshAsync = this.onRefreshAsync.bind(this);
        this.onActiveRobot = this.onActiveRobot.bind(this);

        this.state = {
            activeRobot : {},
            robots : []
        };
    }

    public componentDidMount() {
        console.log("component mounting " + this.isCancelled)
        this.interval = setInterval(() => this.getRobotsAsync(), RobotView.refreshInMs);
    }

    public componentWillUnmount() {
        this.isCancelled = true;
        console.log("component unmounting " + this.isCancelled)
        clearInterval(this.interval);
    }

    public async getRobotsAsync(){
        await this.robotManagerClient.getRobotsAsync();

        const robotsInfo : Robot[] = this.robotManagerClient.responseRobots;

        if(robotsInfo !== undefined && this.isCancelled === false){
            this.setState({
                robots : robotsInfo
            });
        }
    }

    public async onRefreshAsync() {
        await this.getRobotsAsync();
    }

    public onActiveRobot(robotId : string) {
        this.setState({
            activeRobot : robotId
        })
    }

    public render() {
        return (
            <Grid id={"wrapper"}>
                <Row>
                    <Col xs={12} md={9} id={"map-wrapper"}>
                        <Map robots={this.state.robots} activeRobot={this.state.activeRobot}/>
                    </Col>
                    <Col xs={12} md={3}>
                        <RobotInfoPanel robots={this.state.robots} onSelect={this.onActiveRobot}/>
                    </Col>
                </Row>
            </Grid>
        )
    }
}

export default RobotView;