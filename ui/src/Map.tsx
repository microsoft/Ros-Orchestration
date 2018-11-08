// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as d3 from 'd3';
import { AxisDomain, AxisScale, Selection } from 'd3';
import * as React from 'react';
import Configuration from './Configuration';
import Robot from './Robot';
import RobotStatus from './RobotStatus';

class Map extends React.Component<any, any> {

    private ref: SVGSVGElement;

    private svg: Selection<d3.BaseType, {}, null, undefined>;

    private readonly mapDimensions = { 
        height: +Configuration.mapHeightInPixels,
        width: +Configuration.mapWidthInPixels 
    };

    private readonly margins = {
        bottom: +Configuration.marginsBottom,
        left: +Configuration.marginsLeft,
        right: +Configuration.marginsRight,
        top: +Configuration.marginsTop
    }

    private readonly positionUpdateIntervalInMs = 300;

    constructor(props: any, context: any) {
        super(props, context);
    }

    public render() {
        return (
            <div id="map">
                <svg ref={(ref: SVGSVGElement) => this.ref = ref} />
            </div>
        );
    }

    public async componentDidMount() {
        const svgDimensions = this.getSvgDimensions(this.mapDimensions.width, this.mapDimensions.height, this.margins)
        const gridDimensions = this.getGridDimensions(this.mapDimensions.width, this.mapDimensions.height);

        const xRange = this.getXRange(gridDimensions);
        const yRange = this.getYRange(gridDimensions);

        this.svg = this.createSvgElement(svgDimensions.width, svgDimensions.height);
        this.drawMapImage(xRange, yRange, gridDimensions, this.svg, this.drawCursorText);
        this.drawLegend(0, this.mapDimensions.height + 30);
        this.drawCursorText(gridDimensions, this.svg, [0,0]);

        const robots = this.getRobots();
        this.drawMap(xRange, yRange, gridDimensions, this.svg, robots);
        this.drawRobotsOnMap(xRange, yRange, this.svg, robots);
    }

    public async componentDidUpdate() {
        const gridDimensions = this.getGridDimensions(this.mapDimensions.width, this.mapDimensions.height);

        const xRange = this.getXRange(gridDimensions);
        const yRange = this.getYRange(gridDimensions);

        const robots = this.getRobots();
        this.drawRobotsOnMap(xRange, yRange, this.svg, robots);
    }

    public getSvgDimensions(mapWidth: number, mapHeight: number, margins: any) {
        const svgDimensions = {
            height: mapHeight + margins.top + margins.bottom,
            width: mapWidth + margins.left + margins.right
        };
        return svgDimensions;
    }

    public getGridDimensions(mapWidth: number, mapHeight: number) {
        const gridDimensions = {
            height: mapHeight,
            width: mapWidth
        };
        return gridDimensions;
    }

    public createSvgElement(svgWidth: number, svgHeight: number) {
        const ref = d3.select(this.ref)
        .classed("svg-content", true);

        const svg = ref
            .attr("width",'100%')
            .attr("height", '100%')
            .attr("preserveAspectRatio", "xMinYMin meet")
            .attr("viewBox", "0 0 " + svgWidth + " " + svgHeight)
            .append("g")
            .attr("transform",
                "translate(" + this.margins.left + "," + this.margins.top + ")");

        return svg;
    }

    public drawMapImage(xRange: any, yRange: any, gridDimensions: any, svg: any, drawCursorText: any) {
        
        // Draw map image
        this.svg.append("svg:image")
        .attr("xlink:href", "/map-simple.png")
        .style("opacity", .5)
        .attr("height", gridDimensions.height)
        .attr("width", gridDimensions.width)
        .on('mousemove', () => {
            const coords = d3.mouse(d3.event.currentTarget);
            coords[0] = xRange.invert(coords[0]);
            coords[1] = yRange.invert(coords[1]);
            drawCursorText(gridDimensions, svg, coords);
        })
        .on('mouseout', () => {
            drawCursorText(gridDimensions, svg, [0.0, 0.0]);
        });
    }

    public makeGridlinesX(range: AxisScale<AxisDomain>) {
        return d3.axisBottom(range)
            .ticks(10)
    }

    public makeGridlinesY(range: AxisScale<AxisDomain>) {
        return d3.axisLeft(range)
            .ticks(10)
    }

    public drawMap(xRange: any, yRange: any, gridDimensions: any, svg: any, robotStates: any) {

        // add the X gridlines
        svg.append("g")
            .attr("class", "grid")
            .attr("transform", "translate(0," + gridDimensions.height + ")")
            .attr("pointer-events", "none")
            .call(this.makeGridlinesX(xRange)
                .tickSize(-gridDimensions.height)
                .tickFormat((domainValue: AxisDomain, index: number) => "")
            );

        // add the Y gridlines
        svg.append("g")
            .attr("class", "grid")
            .attr("pointer-events", "none")
            .call(this.makeGridlinesY(yRange)
                .tickSize(-gridDimensions.width)
                .tickFormat((domainValue: AxisDomain, index: number) => "")
            );

        // add the X Axis
        svg.append("g")
            .attr("transform", "translate(0," + gridDimensions.height + ")")
            .call(d3.axisBottom(xRange));

        // add the Y Axis
        svg.append("g")
            // .attr("transform", "translate(0,0)")
            .call(d3.axisLeft(yRange));
    }

    public drawRobotsOnMap(xRange: any, yRange: any, svg: any, robots: Robot[]) {
        
        const robotElements = svg.selectAll("g.robot")
            .data(robots, (d: Robot) => {
                return d.id
            });

        // existing robot elements
        const robotCircles = robotElements.select("circle.robotCircle");
      
        robotCircles
            .transition()
            .duration(this.positionUpdateIntervalInMs) 
            .attr("class", "robotCircle")
            .attr("pointer-events", "none")
            .attr("r", (d: Robot) => this.getRobotSize(d.id))
            .attr("cx", (d: Robot) => xRange(d.telemetry.position.x))
            .attr("cy", (d: Robot) => yRange(d.telemetry.position.y))
            .style("fill", (d: Robot) => this.getRobotColor(d.telemetry.status.toString()));

        const robotLabels = robotElements.select("text.robotLabel");
        robotLabels.text((d: Robot) => d.id)
            .transition()
            .duration(this.positionUpdateIntervalInMs)
            .attr("pointer-events", "none")
            .attr("class", "robotLabel")
            .attr("x", (d: Robot) => xRange(d.telemetry.position.x) + (this.getRobotSize(d.id) / 2))
            .attr("y", (d: Robot) => yRange(d.telemetry.position.y) - this.getRobotSize(d.id));

        // new robot elements
        const robotEntry = robotElements
            .enter()
            .append("g")
            .attr("class", "robot");
        
        robotEntry.append("circle")
            .attr("pointer-events", "none")
            .attr("class", "robotCircle")
            .attr("r", (d: Robot) => this.getRobotSize(d.id))
            .attr("cx", (d: Robot) => xRange(d.telemetry.position.x))
            .attr("cy", (d: Robot) => yRange(d.telemetry.position.y))
            .style("fill", (d: Robot) => this.getRobotColor(d.telemetry.status.toString()))

        robotEntry.append("text")
            .text((d: Robot) => d.id)
            .attr("pointer-events", "none")
            .attr("class", "robotLabel")
            .attr("x", (d: Robot) => xRange(d.telemetry.position.x) + (this.getRobotSize(d.id) / 2))
            .attr("y", (d: Robot) => yRange(d.telemetry.position.y) - this.getRobotSize(d.id))

        // remove old robot elements
        robotElements.exit().remove();

        return robotElements;
    }

    public drawLegend(xOffset: number, yOffset: number) {

        const robotStatuses: string[] = Object.keys(RobotStatus).map((key: string) => {
            return RobotStatus[key]
        }).filter(value => typeof value === 'string') as string[];

        const legend = this.svg.selectAll("g.legend")
            .data(robotStatuses)
            .enter().append("svg:g")
            .attr("class", "legend")
            .attr("transform", (d: string, i: number) => {
                return "translate(" + xOffset + "," + (i * 20 + yOffset) + ")";
            });

        legend.append("svg:circle")
            .attr("class", "legend")
            .attr("r", 3.5)
            .style("fill", (status: any) => this.getRobotColor(status));

        legend.append("svg:text")
            .attr("class", "legend")
            .attr("x", 12)
            .attr("dy", ".31em")
            .text((status: string) => {
                return status;
            });

        return legend;
    }

    public drawCursorText(gridDimensions: any, svg: Selection<d3.BaseType, {}, null, undefined>, coords: number[]) {

        const xOffset = gridDimensions.width - 120;
        const yOffset = gridDimensions.height + 30;

        let textbox = svg.selectAll("g.cursortextbox")
            .data(["CursorTextbox"]);

        textbox = textbox.enter()
            .append("svg:g")
            .attr("class", "cursortextbox")
            .attr("transform", (d: string) => {
                return "translate(" + xOffset + "," + yOffset + ")";
            }).merge(textbox);

        const text = textbox.selectAll("text.cursortext")
            .data([coords]);

        text.enter()
            .append("svg:text")
            .attr("class", "cursortext")
            .attr("x", 12)
            .attr("dy", ".31em")
            .merge(text)
            .text((coordinates: any) => {
                return "X:" + coordinates[0].toFixed(3) + ", Y:" + coordinates[1].toFixed(3);
            });

        return text;
    }

    private getRobots() {
        let robots = [];

        if (this.props.robots !== undefined) {
            robots = this.props.robots;
        }

        return robots;
    }

    private getRobotColor(robotStatus: string) {
        switch (robotStatus) {
            case RobotStatus[RobotStatus.Onboarding]: {
                return "orange";
            }
            case RobotStatus[RobotStatus.Idle]: {
                return "green";
            }
            case RobotStatus[RobotStatus.Busy]: {
                return "red";
            }
            case RobotStatus[RobotStatus.Failed]: {
                return "gray";
            }
        }

        return "black";
    }

    private getRobotSize(robotId: string) {

        const regularDotSize: number = 5;
        const currentDotSize: number = 10;

        let dotSize: number;

        if (robotId === this.props.activeRobot) {
            dotSize = currentDotSize;
        }
        else {
            dotSize = regularDotSize;
        }

        return dotSize;
    }

    private getXRange(gridDimensions: any) {
        const xRange = d3.scaleLinear().range([0, gridDimensions.width]);
        xRange.domain([0, +Configuration.mapWidthInMeters]);
        return xRange;
    }

    private getYRange(gridDimensions: any) {
        const yRange = d3.scaleLinear().range([gridDimensions.height, 0]);
        yRange.domain([0, +Configuration.mapHeightInMeters]);
        return yRange;
    }
}

export default Map;
