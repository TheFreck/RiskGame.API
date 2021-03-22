import React, { Component } from 'react';
import AssetCreate from '../forms/AssetCreate';
import PlayerCreate from './../forms/PlayerCreate';
import Transaction from './../forms/Transaction';
import API from './../../API';
import './../../game.css';
import Button from 'react-bootstrap/Button';

export class GameHome extends Component {
    static displayName = GameHome.name;

    constructor(props) {
        super(props);
        this.state = {
            player: {},
            cash: {},
            asset: {},

            gotEm: false,
            tradeTicket: false,
            tradeButtonMessageDisplay: false,

            tradeButtonMessage: "",
        };
    }

    componentDidMount() {
    }

    updateState = changeSet => {
        let gotEm = false;
        if ((this.state.player || changeSet.player) && (this.state.cash || changeSet.cash) && (this.state.asset || changeSet.asset)) {
            gotEm = true;
        }
        console.log("state set");
        this.setState({
            player: changeSet.player ? changeSet.player : this.state.player,
            cash: changeSet.cash ? changeSet.cash : this.state.cash,
            asset: changeSet.asset ? changeSet.asset : this.state.asset,
            gotEm
        });
    };
    tradeButtonClick = () => this.setState({ tradeTicket: !this.state.tradeTicket });
    tradeButtonMouseEnter = () => {
        console.log("enter");
        this.setState({ tradeButtonMessage: "Create an asset and a player to start" });
        setTimeout(() => this.setState({ tradeButtonMessage: "" }), 3333);
    }
    tradeButtonMouseLeave = () => {
        console.log("exit");
        this.setState({ tradeButtonMessage: "" });
    }
    hoverMessage = () => {
        setTimeout(() => {
            if (this.state.tradeButtonMessageDisplay) {
                return ""
            }},3000)
        
    }
    TradeButton = gotEm => {
        if (gotEm.gotEm) {
            return <Button onClick={this.tradeButtonClick} variant="dark">Place a Trade</Button>;
        }
        else {
            return <Button onMouseEnter={this.tradeButtonMouseEnter} variant="secondary" disabled>Place a Trade</Button>;
        }
    }
    selfDestruct = () => {
        API.initialize('playa101').then(answer => {
            console.log(answer);
            console.log(answer.data);
        })
    }

    render = () => {

        return (
            <>
                <Button onClick={this.selfDestruct} variant="danger">Don't<br/> Press!</Button>
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
                <this.TradeButton
                    gotEm={this.state.gotEm}
                />
                {this.state.tradeButtonMessage}
                {this.state.tradeTicket ?
                    <Transaction
                    updateState={() => this.updateState}
                    retrieveState={this.state}
                    />
                    : ""}
                
            </>
        );
    }
}
