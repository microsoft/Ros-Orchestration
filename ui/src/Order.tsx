import { Guid } from "guid-typescript";
import Job from "./Job";
import OrderStatus from "./OrderStatus";
import Position2d from "./Position2d";

class Order
{
    public id: Guid;

    public startPosition : Position2d;
    public endPosition : Position2d;

    public status : OrderStatus;

    public jobs : Job[];

    public createdDateTime : Date;

    public message : string;
}

export default Order;