import { Guid } from "guid-typescript";
import JobStatus from "./JobStatus";
import Position2d from "./Position2d";

class Job {
    public Id : Guid;

    public RobotId : string;

    public OrderId : Guid;

    public Status : JobStatus;

    public StartPosition : Position2d;
    public EndPosition : Position2d;
}
     
export default Job;
