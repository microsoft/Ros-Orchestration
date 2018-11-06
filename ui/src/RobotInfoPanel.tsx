import * as React from 'react';
import { PanelGroup }  from 'react-bootstrap';
import Robot from './Robot';
import RobotInfoCollapse from './RobotInfoCollapse';

class RobotInfoPanel extends React.Component <any, any>{

    constructor(props: any, context: any) {
        super(props, context);
        this.handleSelect = this.handleSelect.bind(this);
    }

    public getRobotInfoCollapses() {

        const result : JSX.Element = this.props.robots.map((robot : Robot) => 
            <RobotInfoCollapse robot={robot} key={robot.id}/> 
        );

        return result;
    }

    public handleSelect(activeKey : any) {
        this.props.onSelect(activeKey);
    }

    public render() {
        return (
            <PanelGroup accordion={ true } id="accordion" 
                onSelect={this.handleSelect}>
                { this.getRobotInfoCollapses() }
            </PanelGroup>
        );
    }
}

export default RobotInfoPanel;
