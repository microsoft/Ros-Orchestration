// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as React from 'react';
import { Tab, Tabs } from 'react-bootstrap';
import './App.css';
import Configuration from './Configuration';
import OrderTable from './OrderTable';
import RobotView from './RobotView';

class App extends React.Component<any, any>{

  constructor(props: any, context: any) {
    super(props, context);

    this.handleSelect = this.handleSelect.bind(this);

    this.state = {
      configLoaded: false,
      key: 1
    };
  }

  public componentWillMount() {
    Configuration.initialize().then(() => {
      this.setState({ configLoaded: true });
    });
  }

  public render() {
    return (
      <div className="App">
        <header className="App-header">
          <h1 className="App-title">Robot Orchestrator Panel</h1>
        </header>

        {this.state && this.state.configLoaded &&
          <Tabs animation={false}
            mountOnEnter={true}
            activeKey={this.state.key}
            onSelect={this.handleSelect}
            id="table-selection-tabs" >
            <Tab eventKey={1} title="Map">
              <RobotView />
            </Tab>
            <Tab eventKey={2} title="Orders">
              <OrderTable />
            </Tab>
          </Tabs>
        }
      </div>
    );
  }

  private handleSelect(key: any) {
    this.setState({ key });
  }
}

export default App;
