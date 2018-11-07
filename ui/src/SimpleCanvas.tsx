// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

export default class SimpleCanvas {
    private canvas: HTMLCanvasElement;
    private context: CanvasRenderingContext2D;

    constructor(container: string | HTMLElement) {
        let containerElement: HTMLElement;

        if (typeof container === "string"){
            containerElement = document.getElementById(container) as HTMLElement;
        } else {
            containerElement = container;
        }

        if (containerElement.tagName === "CANVAS") {
            this.canvas = containerElement as HTMLCanvasElement;
        } else {
            this.canvas = document.createElement("canvas");
            const dimensions = containerElement.getBoundingClientRect()
            this.canvas.width = dimensions.width;
            this.canvas.height = dimensions.height;
        }
        this.context = this.canvas.getContext("2d") as CanvasRenderingContext2D;
    }

    public clear(){
        const context = this.canvas.getContext('2d');

        if(context != null){
            context.clearRect(0, 0, this.canvas.width, this.canvas.height);
        }
    }

    public drawRect(x: number, y: number, width: number, height: number, strokeColor: string = "black", lineWidth: number = 1, fillColor: string = "", alpha: number = 1) {
        const context = this.context;
        context.globalAlpha = alpha;
        if (fillColor) {
            context.fillStyle = fillColor;
            context.fillRect(x, y, width, height);
        }
        if (lineWidth) {
            context.strokeStyle = strokeColor;
            context.lineWidth = lineWidth;
            context.strokeRect(x, y, width, height);
        }
    }
    
    public drawLine(x1: number, y1: number, x2: number, y2: number, color: string = "black", lineWidth: number = 1, alpha: number = 1) {
        const context = this.context;
        context.globalAlpha = alpha;
        context.beginPath();
        context.moveTo(x1, y1);
        context.lineTo(x2, y2);
        if (lineWidth) {
            context.strokeStyle = color;
            context.lineWidth = lineWidth;
            context.stroke();
        }
    }
    
    public drawCircle(x: number, y: number, radius: number, strokeColor: string = "black", lineWidth: number = 1, fillColor: string = "", alpha: number = 1) {
        const context = this.context;
        context.beginPath();
        context.lineWidth = lineWidth;
        context.globalAlpha = alpha;
        context.arc(x, y, radius, 0, Math.PI * 2, true);
        context.closePath();
        if (lineWidth) {
            context.strokeStyle = strokeColor;
            context.stroke();
        }
        if (fillColor) {
            context.fillStyle = fillColor;
            context.fill();
        }
    }
    
    public async drawImage(src: string, x: number, y: number, width: number, height: number, alpha: number = 1) {

        return new Promise((resolve, reject) => {
            const context = this.context;
            const img = new Image();
            img.src = src;

            console.log("In the func of drawImage:" + src);

            context.globalAlpha = alpha;
            img.onload = () => {
                console.log("Begin to draw image");

                context.drawImage(img, x, y, width, height);
                resolve();
            };
        }); 
    }
}