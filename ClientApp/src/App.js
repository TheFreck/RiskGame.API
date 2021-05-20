import React, { Component } from 'react';
import { Router, Route } from 'react-router';
import { Layout } from './components/Layout';
import { createBrowserHistory as createHistory } from "history";
import $ from 'jquery';
import { GameHome } from './components/pages/GameHome';
import Store from './stateManagement/Store';
import { PlayerLoopFlowChart } from './ProcessFlows/PlayerLoopFlowChart';
import { AssetLoopFlowChart } from './ProcessFlows/AssetLoopFlowChart';

import './custom.css'
const history = createHistory();

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
        this.state = {
            now: 0
        };
        this.componentDidMount = this.componentDidMount.bind(this);
    }

    componentDidMount() {
        $(window).on("scroll", () => {
            this.amountscrolled();
        });

    }



    amountscrolled = () => {
        var winheight = $(window).height();
        var docheight = $(document).height();
        var scrollTop = $(window).scrollTop();
        var trackLength = docheight - winheight;
        var pctScrolled = Math.floor(scrollTop / trackLength * 100);
        this.setState({
            now: pctScrolled
        })
    }

    mouseMove = event => {
        const cursor = document.querySelector(".cursor");
        cursor.style.left = `${event.pageX}px`;
        cursor.style.top = `${event.pageY}px`;
    }

    render() {
        return (
            <Store>
                <Router
                    onMouseMove={this.mouseMove}
                    history={history}
                >
                    <Route exact path='/' component={GameHome} />
                    <Route path='/game' component={GameHome} />
                    <Route path='/player-flow' component={PlayerLoopFlowChart} />
                    <Route path='/asset-flow' component={AssetLoopFlowChart} />
                </Router>
            </Store>
        );
    }
}
