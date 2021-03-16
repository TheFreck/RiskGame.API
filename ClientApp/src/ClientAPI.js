import React, { Component } from 'react';

export default GlobalState = {
    state: {
        player: { },
        cash: { },
        asset: { }
    },
    setPlayer: player =>{ this.state.player = player },
    getPlayer: () => { return this.state.player },
    setCash: cash => { this.state.cash = cash },
    getCash: () => { return this.state.cash },
    setAsset: asset => { this.state.asset = asset },
    getAsset: () => {return this.state.asset}
}