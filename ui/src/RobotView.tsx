import * as React from 'react';
import {Button, Col, Glyphicon, Grid, Row} from 'react-bootstrap';
import Robot from './Robot';
import RobotInfoPanel from './RobotInfoPanel';
import RobotManagerClient from './RobotManagerClient';
import RobotMap from './RobotMap';

class RobotView extends React.Component <any, any>{

    private static refreshInMs : number = 3000;

    private robotManagerClient : RobotManagerClient;

    private interval : NodeJS.Timer;

    constructor(props: any, context: any) {
        super(props, context);

        this.robotManagerClient = new RobotManagerClient();

        this.onRefreshAsync = this.onRefreshAsync.bind(this);
        this.onActiveRobot = this.onActiveRobot.bind(this);

        this.state = {
            activeRobot : {},
            robots : []
        };
    }

    public componentDidMount() {
        this.interval = setInterval(() => this.getRobotsAsync(), RobotView.refreshInMs);
    }

    public componentWillUnmount() {
        clearInterval(this.interval);
    }

    public async getRobotsAsync(){
        await this.robotManagerClient.getRobotsAsync();

        const robotsInfo : Robot[] = this.robotManagerClient.responseRobots;

        if(robotsInfo !== undefined){
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
            <Grid>
                <Button
                    onClick={this.onRefreshAsync} bsStyle={"primary"}>
                    Refresh  <Glyphicon glyph="refresh" />
                </Button>
                <Row>
                    <Col xs={12} md={7}>
                        <RobotMap robots={this.state.robots} telemetries={this.state.telemetries} activeRobot={this.state.activeRobot}/>
                    </Col>
                    <Col xs={12} md={5}>
                        <RobotInfoPanel robots={this.state.robots} onSelect={this.onActiveRobot}/>
                    </Col>
                </Row>
            </Grid>
        )
    }
}

export default RobotView;