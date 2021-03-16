import React, { Component } from 'react';
import AssetCreate from '../forms/AssetCreate';
import PlayerCreate from './../forms/PlayerCreate';
import Transaction from './../forms/Transaction';
import './../../game.css';

export class GameHome extends Component {
    static displayName = GameHome.name;

    constructor(props) {
        super(props);
        this.state = {
            player: {},
            cash: {},
            asset: {},
        };
    }

    componentDidMount() {
    }

    updateState = changeSet => {
        debugger;
        this.setState({
            player: changeSet.player ? changeSet.player : this.state.player,
            cash: changeSet.cash ? changeSet.cash : this.state.cash,
            asset: changeSet.asset ? changeSet.asset : this.state.asset
        });
        console.log("game home state: ", this.state);
    };

    render = () => {

        return (
            <>
                <AssetCreate
                    updateState={this.updateState}
                    retrieveState={this.state}
                />
                <hr />
                <hr />
                <PlayerCreate
                    updateState={this.updateState}
                    retrieveState={this.state}
                />
                <hr />
                <hr />
                <hr />
                <hr />
                {
                    // create a button to load the transaction component
                    // the button only shows once the asset and the player have been created
                }
                <Transaction
                    updateState={this.updateState}
                    retrieveState={this.state}
                />
            </>
        );
    }
}
