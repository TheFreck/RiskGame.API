import React, { useState, useEffect } from 'react';
import AssetCreate from '../forms/AssetCreate';
import PlayerCreate from './../forms/PlayerCreate';
import Transaction from './../forms/Transaction';
import API from './../../API';
import './../../game.css';
import Button from 'react-bootstrap/Button';
import Chart from './../pageComponents/Chart';

export const GameHome = props => {

    const [player, SETplayer] = useState({ player: {} });
    const [cash, SETcash] = useState({ cash: {} });
    const [asset, SETasset] = useState({ asset: {} });
    const [xSeries, SETxSeries] = useState({ xSeries: [] });
    const [showPane, SETshowPane] = useState("");
    const [gotEm, SETgotEm] = useState(false);
    const [tradeButtonMessageDisplay, SETtradeButtonMessageDisplay] = useState(false);
    const [tradeButtonMessage, SETtradeButtonMessage] = useState("");
    const [viewPane, SETviewPane] = useState(<></>);

    let state = {
        player: [player, SETplayer],
        cash: [cash, SETcash],
        asset: [asset, SETasset]
    };

    // **********
    // GO GETTERS
    // **********
    const updateState = changeSet => {
        let gotEm = false;
        if ((player || changeSet.player) && (cash || changeSet.cash) && (asset || changeSet.asset)) {
            gotEm = true;
        }
        console.log("state set: ", changeSet);
        SETplayer({ player: changeSet.player ? changeSet.player : player })
        SETcash({ cash: changeSet.csh ? changeSet.csh : cash });
        SETasset({ asset: changeSet.asst ? changeSet.asst : asset });
        SETgotEm(gotEm);
    };
    const getChartData = cb => {
        API.gamePlay.isGameOn().then(gameOn => {
            console.log("gameon: ", gameOn);
            if (!gameOn.data) {
                API.gamePlay.getData().then(data => {
                    if (data.status === 200) SETxSeries({ xSeries: data.data });
                    console.log("data: ", data);
                    console.log("data.data: ", data.data);
                    let series = [];
                    for (let point of data.data) {
                        series.push(point.assets[0].value);
                    }
                    console.log("gameon series: ", series);
                    cb(series);
                })
            }
            else {
                cb([]);
            }
        })
    }

    // ********
    // SERVICES
    // ********
    const hoverMessage = () => {
        setTimeout(() => {
            if (tradeButtonMessageDisplay) {
                return ""
            }
        }, 3000)

    }

    // **************
    // EVENT HANDLING
    // **************
    const handleReset = () => {
        Array.from(document.querySelectorAll("input")).forEach(
            input => {
                (input.value = "");
            }
        );
        Array.from(document.querySelectorAll("textarea")).forEach(
            textarea => {
                (textarea.value = "");
            }
        );
    };
    const selfDestruct = () => {
        console.log(API.gamePlay.initialize("Playa101"));
        SETplayer({ player: {} });
        SETcash({ cash: {} });
        SETasset({ asset: {} });
        SETgotEm(false);
        SETtradeButtonMessageDisplay(false);
        SETtradeButtonMessage("");
    }
    const assetButtonClick = () => SETviewPane(<AssetCreate updateState={updateState} retrieveState={state} />);
    const playerButtonClick = () => SETviewPane(<PlayerCreate updateState={updateState} retrieveState={state} />);
    const tradeButtonClick = () => SETviewPane(<Transaction updateState={updateState} retrieveState={state} />);
    const chartButtonClick = () => {
        getChartData(series => {
            console.log("chartButtonClick series: ", series);
            SETviewPane(<Chart xSeries={series} height={high} width={wide} />);
        })
    }
    const tradeButtonMouseEnter = () => {
        console.log("enter");
        SETtradeButtonMessage("Create an asset and a player to start");
        setTimeout(() => SETtradeButtonMessage(""), 3333);
    }
    const tradeButtonMouseLeave = () => {
        console.log("exit");
        SETtradeButtonMessage("");
    }

    // *********************
    // FUNCTIONAL COMPONENTS
    // *********************
    let initStyle = {
        "borderRadius": "50%",
        "borderColor": "darkred",
        "borderWidth": "5px"
    }
    const SelfDestructButton = () => <Button
        onClick={selfDestruct}
        style={initStyle}
        variant="danger"
    >Don't<br /> Press!<br />Or Else!!!</Button>;
    const TradeButton = gotEm => {
        if (gotEm.gotEm) {
            return <Button onClick={tradeButtonClick} variant="dark">Place a Trade</Button>;
        }
        else {
            return <Button onMouseEnter={tradeButtonMouseEnter} variant="secondary" disabled>Place a Trade</Button>;
        }
    }
    const AssetButton = () => <Button
        onClick={assetButtonClick}
        variant="light"
    >Create an Asset</Button>
    const PlayerButton = () => <Button
        onClick={playerButtonClick}
        variant="light"
    >Create a Player</Button>
    const ChartButton = () => <Button
        onClick={chartButtonClick}
        variant="light"
    >ViewChart</Button>
    var wide = window.visualViewport.width * .8;
    var high = window.visualViewport.height * .8;

    return (
        <>
            <SelfDestructButton />
            <AssetButton />
            <PlayerButton />
            <TradeButton gotEm={gotEm} />
            <ChartButton />
            <div>{viewPane}</div>
        </>
    );
}
