// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as React from 'react';
import {Button, Glyphicon} from 'react-bootstrap';
import * as ReactDataGrid from 'react-data-grid';
import EmptyRowsView from "./EmptyTable";
import Order from './Order';
import OrderManagerClient from "./OrderManagerClient";

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
            <div>
                <Button
                    onClick={this.onclickAsync} bsStyle={"primary"}> 
                    Orders  <Glyphicon glyph="refresh"/>
                </Button>
                <ReactDataGrid
                    columns={this.columns}
            rowGetter={this.rowGetter}
            rowsCount={this.state.rows.length}
            minHeight={500}
            emptyRowsView = {EmptyRowsView} />
        </div>);
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
                    EndPosition : JSON.stringify(order.endPosition),
                    Id: order.id.toString(),
                    Jobs : JSON.stringify(order.jobs),
                    Message : JSON.stringify(order.message),
                    StartPosition :JSON.stringify(order.startPosition),
                    Status : order.status
                });
            }
        }

        this.setState({rows})
    };
}

export default OrderTable;