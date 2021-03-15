import React, { Component } from 'react';
import AssetCreate from '../forms/AssetCreate';
import './../../game.css';

export class GameHome extends Component {
    static displayName = GameHome.name;

    constructor(props) {
        super(props);
        this.state = {

        };
        this.componentDidMount = this.componentDidMount.bind(this);
    }

    componentDidMount() {
    }

    render = () => {

        return (
            <>
                <AssetCreate />
            </>
        );
    }
}
