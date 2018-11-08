// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as React from 'react';
import { Nav, Navbar, NavItem } from 'react-bootstrap'
import { NavLink, Redirect, Route, Switch } from 'react-router-dom'; // import the react-router-dom components
import './App.css';
import Configuration from './Configuration';
import { Orders, Visualization } from './Pages'

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
        <this.Header />
        {this.state && this.state.configLoaded &&
         <this.Main />
        }
      </div>
    );
  }

  private handleSelect(key: any) {
    this.setState({ key });
  }

  private Header = () => (
    <Navbar inverse={true} collapseOnSelect={true}>
        <Navbar.Header>
            <Navbar.Brand>
                <a href="#"><span id="brand">Ros-Orchestrator</span></a>
            </Navbar.Brand>
            <Navbar.Toggle />
        </Navbar.Header>
        <Navbar.Collapse>
            <Nav>
                <NavItem eventKey={1} componentClass='span'>
                    <NavLink className="nav-link" to="/Visualization" replace={true} ><span>Visualization</span></NavLink>
                </NavItem>
                <NavItem eventKey={2} componentClass='span'>
                    <NavLink className="nav-link" to="/Orders" replace={true} ><span>Orders</span></NavLink>
                </NavItem>
            </Nav>
            <Nav pullRight={true}>
                <NavItem eventKey={3} href="https://github.com/Microsoft/Ros-Orchestration">
                    <span>GitHub Repo</span>
                </NavItem>
            </Nav>
        </Navbar.Collapse>
    </Navbar>
  )

  private Main = () => (
    <main>
        <Switch>
            <Route exact={true} path='/' render={RedirectVisualization} />
            <Route exact={true} path='/Visualization' render={Visualization} />
            <Route exact={true} path='/Orders' render={Orders}/>
        </Switch>
    </main>
  );
}

const RedirectVisualization = () => {
    return <Redirect to='/Visualization'/>
}

export default App;
