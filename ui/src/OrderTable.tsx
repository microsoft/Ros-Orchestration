import * as React from 'react';
import {Button, Col, Glyphicon, Grid, Row} from 'react-bootstrap';
import * as ReactDataGrid from 'react-data-grid';
import EmptyRowsView from "./EmptyTable";
import Order from './Order';
import OrderManagerClient from "./OrderManagerClient";
import Position2d from './Position2d';

class OrderTable extends React.Component <any, any>{
    public state: any;

    private columns: any;
  
    private orders : Order[];
    private orderManagerClient : OrderManagerClient;
    
    constructor(props: any, context: any) {
        super(props, context);

        this.columns = [];
  
        this.state = {rows : []};
   
        this.orderManagerClient = new OrderManagerClient();
    
        this.columns = [
            { key: 'Id', name: 'ID', resizable: true },
            { key: 'StartPosition', name: 'StartPosition', resizable: true  },
            { key: 'EndPosition', name: 'EndPosition', resizable: true  },
            { key: 'Status', name: 'Status', resizable: true  },
            { key: 'JobStatus', name: 'Jobs', resizable: true  }, 
            { key: 'CreatedDateTime', name: 'CreatedDateTime', resizable: true  }, 
            { key: 'Message', name: 'Message', resizable: true  }   
        ];

        this.onclickAsync = this.onclickAsync.bind(this);
    }

    public rowGetter = (i : number) => {
        return this.state.rows[i];
    };

    public async onclickAsync(){
        await this.getOrdersAsync();
        this.createRows();
    }

    public render() {
        return (
            <Grid id={"wrapper"}>
                <Row id={"orders-wrapper"}>
                    <Col xs={10} md={10} />
                    <Col xs={2} md={2} id={"refresh-wrapper"}>
                        <Button
                        onClick={this.onclickAsync} bsStyle={"primary"}> 
                        Orders  <Glyphicon glyph="refresh"/>
                        </Button>
                    </Col>
                    <Col xs={12} md={12} id={"orders-table-wrapper"}>
                        <div>
                            <ReactDataGrid
                                        columns={this.columns}
                                rowGetter={this.rowGetter}
                                rowsCount={this.state.rows.length}
                                minHeight={500}
                                emptyRowsView = {EmptyRowsView} />
                        </div>
                    </Col>
                </Row>
            </Grid>
        );
    }

    private async getOrdersAsync(){
        await this.orderManagerClient.getOrdersAsync();
        const orders : Order[] = this.orderManagerClient.response;

        this.orders = orders;
    }

    private createRows = () => {
        const rows = [];

        if(null != this.orders){
            for (const order of this.orders) {
                rows.push({
                    CreatedDateTime : JSON.stringify(order.createdDateTime), 
                    EndPosition : this.formatPosition(order.endPosition),
                    Id: order.id.toString(),
                    Jobs : JSON.stringify(order.jobs),
                    Message : order.message,
                    StartPosition : this.formatPosition(order.startPosition),
                    Status : order.status
                });
            }
        }

        this.setState({rows})
    };

    private formatPosition(position : Position2d) {
        return `X: ${position.x.toFixed(3)}, Y: ${position.y.toFixed(3)}`
    }
}

export default OrderTable;