import * as d3 from 'd3';
import { AxisDomain, AxisScale, Selection } from 'd3';
import * as React from 'react';
import RobotStatus from './RobotStatus'

class Map extends React.Component <any, any> {

    private ref : SVGSVGElement;

    private svg : Selection<d3.BaseType, {}, null, undefined>;

    // Map dimensions must match aspect ratio of map image
    private readonly mapDimensions = { width : 600, height: 529};

    public async componentDidMount() {
        const margins = { top: 20, right: 20, bottom: 200, left: 100 }

        const svgDimensions = this.getSvgDimensions(this.mapDimensions.width, this.mapDimensions.height, margins)
        const gridDimensions = this.getGridDimensions(this.mapDimensions.width, this.mapDimensions.height);

        const ref = d3.select(this.ref)
        
        const svg = ref
            .attr("width", svgDimensions.width)
            .attr("height", svgDimensions.height)
            .append("g")
            .attr("transform",
                "translate(" + margins.left + "," + margins.top + ")");

        this.svg = svg;
        const drawCursorText = this.drawCursorText;
        const cursorTextX = this.mapDimensions.width - 80;
        const cursorTextY = this.mapDimensions.height + 30;

        const xRange = d3.scaleLinear().range([0, gridDimensions.width]);
        const yRange = d3.scaleLinear().range([gridDimensions.height, 0]);
        xRange.domain([0, 63.9000]);
        yRange.domain([0, 56.3000]);

        this.drawCursorText(svg,cursorTextX,cursorTextY,[0.0,0.0]);

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
                drawCursorText(svg,cursorTextX,cursorTextY,coords);
            })
            .on('mouseout', () => {
                drawCursorText(svg,cursorTextX,cursorTextY,[0.0,0.0]);
            });
            
        this.drawLegend(0, this.mapDimensions.height + 30);
    }

    public async componentDidUpdate() {
        console.log("Called component did update.")

        const gridDimensions = this.getGridDimensions(this.mapDimensions.width, this.mapDimensions.height);
        const xRange = d3.scaleLinear().range([0, gridDimensions.width]);
        const yRange = d3.scaleLinear().range([gridDimensions.height, 0]);

        const stateArray = this.getFakeStateArray();
        const robotStates = this.getRobotStates(stateArray);
        this.drawMap(xRange, yRange, gridDimensions, this.svg, robotStates);
        this.drawRobotsOnMap(xRange, yRange, this.svg, robotStates);
    }

    public render() {
        return (
            <div id="map">
                <svg className="container" ref={(ref: SVGSVGElement) => this.ref = ref} />
            </div>
        );
    }

    // TODO: Delete if using real data
    public getRandomOffset() {
        const offset = 0.5;
        return (Math.random() * 2 * offset) - offset;
    }

    // TODO: Delete if using real data
    public getFakeStateArray() {
        const stateArray = [
        [{"id":"09/30/2018 11:21:20","createdDateTime":"2018-09-30T11:21:20.288738","position":{"x":41.9773764723991,"y":47.502031711303552},"status":"Busy","orderId":"51e9eb09-e87f-48b3-bc32-7834857cbbc0","robotId":"aksrobot0"}],
        [{"id":"09/30/2018 11:21:20","createdDateTime":"2018-09-30T11:21:20.615891","position":{"x":38.448305146354556,"y":31.359546944589617},"status":"Busy","orderId":"716eedbd-9243-4fd5-adbd-698ae1c8243a","robotId":"aksrobot1"}],
        [{"id":"09/30/2018 11:21:19","createdDateTime":"2018-09-30T11:21:19.852447","position":{"x":36.9873,"y":24.868},"status":"Idle","orderId":"","robotId":"aksrobot2"}],
        [{"id":"09/30/2018 11:21:20","createdDateTime":"2018-09-30T11:21:20.078209","position":{"x":50.5029,"y":23.5177},"status":"Idle","orderId":"","robotId":"aksrobot3"}],
        [{"id":"09/30/2018 11:21:18","createdDateTime":"2018-09-30T11:21:18.736736","position":{"x":38.1535,"y":48.0746},"status":"Idle","orderId":"","robotId":"aksrobot4"}],
        [{"id":"09/30/2018 11:21:18","createdDateTime":"2018-09-30T11:21:18.708612","position":{"x":9.11684,"y":24.4549},"status":"Idle","orderId":"","robotId":"aksrobot5"}],
        [{"id":"09/30/2018 11:21:20","createdDateTime":"2018-09-30T11:21:20.183019","position":{"x":40.6739,"y":26.8794},"status":"Idle","orderId":"","robotId":"aksrobot6"}],
        [{"id":"09/30/2018 11:21:19","createdDateTime":"2018-09-30T11:21:19.245554","position":{"x":45.1197,"y":42.3837},"status":"Idle","orderId":"","robotId":"aksrobot7"}],
        [{"id":"09/30/2018 11:21:20","createdDateTime":"2018-09-30T11:21:20.108014","position":{"x":28.8429,"y":42.2006},"status":"Idle","orderId":"","robotId":"aksrobot8"}],
        [{"id":"09/30/2018 11:21:18","createdDateTime":"2018-09-30T11:21:18.604788","position":{"x":55.9058,"y":13.4081},"status":"Idle","orderId":"","robotId":"aksrobot9"}]
        ];
        return stateArray;
    }

    public getRobotStates(stateArray : any) {
        return stateArray.map((data : any) => { 
            return {
                robotId : data[0].robotId,
                status : data[0].status,
                x : data[0].position.x,
                y : data[0].position.y
            }
        }); 
    }

    public getSvgDimensions(mapWidth : number, mapHeight : number, margins : any) {
        const svgDimensions = { 
            height: mapHeight + margins.top + margins.bottom,
            width: mapWidth + margins.left + margins.right
        };
        return svgDimensions;
    }

    public getGridDimensions(mapWidth : number, mapHeight : number) {
        const gridDimensions = { 
            height: mapHeight,
            width: mapWidth
        };
        return gridDimensions;
    }

    public makeGridlinesX(range : AxisScale<AxisDomain>) {
        return d3.axisBottom(range)
                .ticks(10)
    }

    public makeGridlinesY(range : AxisScale<AxisDomain>) {
        return d3.axisLeft(range)
                .ticks(10)
    }

    public drawMap(xRange : any, yRange : any, gridDimensions : any, svg : any, robotStates : any) {
        // Scale the range of the data
        // xRange.domain([0, d3.max(robotStates, (d : any) => d.x)]);
        // yRange.domain([0, d3.max(robotStates, (d : any) => d.y)]);
        xRange.domain([0, 63.9000]);
        yRange.domain([0, 56.3000]);

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

    public drawRobotsOnMap(xRange : any, yRange : any, svg : Selection<d3.BaseType, {}, null, undefined>, robotStates : any) {
        const radius = 3.5;

        const robotElements = svg.selectAll("circle.robot")
                .data(robotStates, (d: any) => d.robotId)
                .enter() // for each robot in robotStates

        robotElements.append("circle")
                .attr("class", "robot")
                .attr("r", radius)
                .attr("pointer-events", "none")
                .attr("cx", (d : any) => xRange(d.x))
                .attr("cy", (d : any) => yRange(d.y))
                .style("fill", (d : any) => this.getStatusColor(d.status));

        robotElements.append("text")
                .text((d) => d.robotId )
                .attr("x", (d : any) => xRange(d.x) + (radius / 2))
                .attr("y", (d : any) => yRange(d.y) - radius)
                .attr("pointer-events", "none")
                .attr("class", "robotLabels")

        return robotElements;
    }

    public drawLegend(xOffset: number, yOffset: number) {

        const robotStatuses : string[] = Object.keys(RobotStatus).map( (key : string) => {
            return RobotStatus[key]
        }).filter(value => typeof value === 'string') as string[];
        
        const legend = this.svg.selectAll("g.legend")
            .data(robotStatuses)
            .enter().append("svg:g")
            .attr("class", "legend")
            .attr("transform", (d : string, i: number) => { 
                return "translate(" + xOffset + "," + (i * 20 + yOffset) + ")"; 
            });

        legend.append("svg:circle")
            .attr("class", "legend")
            .attr("r", 3.5)
            .style("fill", (status : any) => this.getStatusColor(status));

        legend.append("svg:text")
            .attr("class", "legend")
            .attr("x", 12)
            .attr("dy", ".31em")
            .text((status : string) => { 
                return status;
            });

        return legend;
    }

    public drawCursorText(svg : Selection<d3.BaseType, {}, null, undefined>, xOffset: number, yOffset: number, coords : number[]) {
                
        let textbox = svg.selectAll("g.textbox")
            .data(["CursorTextbox"]);
            
        textbox = textbox.enter()
            .append("svg:g")
            .attr("class", "textbox")
            .attr("transform", (d : string) => { 
                return "translate(" + xOffset + ","  + yOffset + ")"; 
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

    public getStatusColor(status : string) {
        if (status === "Idle") {
            return "green";
        } else if(status === "Onboarding") {
            return "orange";
        } else if (status === "Busy") {
            return "red";
        } else {
            return "grey";
        }
    }

    public animateDotMovement(xRange : any, yRange : any, circles : any, positionUpdateIntervalInMs : number) {

        circles.transition()
            .duration(positionUpdateIntervalInMs) 
            .attr("cx", (d : any) => xRange(d.x))
            .attr("cy", (d : any) => yRange(d.y))
            .style("fill", (d : any) => this.getStatusColor(d.status));
        
    }

    public updateRobotStates(robotStates : any) {
        /* TODO: Intent here is to fetch updated position data and update robotStates.
        This fakes it by just randomly updating the position of any active robot */

        robotStates.forEach((d : any) => {
            if(d.status === 2) { 
                // Only busy robots move
                d.x += this.getRandomOffset();
                d.y += this.getRandomOffset();
            }
        });
    }

    public onTimerExpired(xRange : any, yRange : any, robotStates : any, circles : any, positionUpdateIntervalInMs : number) {
        this.updateRobotStates(robotStates);
        this.animateDotMovement(xRange, yRange, circles, positionUpdateIntervalInMs);
    }
}

export default Map;
