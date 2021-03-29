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
    const [assets, SETassets] = useState({ assets: [] });
    const [xSeries, SETxSeries] = useState({ xSeries: [] });
    const [showPane, SETshowPane] = useState("");
    const [gotEm, SETgotEm] = useState(false);
    const [tradeButtonMessageDisplay, SETtradeButtonMessageDisplay] = useState(false);
    const [tradeButtonMessage, SETtradeButtonMessage] = useState("");
    const [viewPane, SETviewPane] = useState(<></>);
    const [isRunning, SETisRunning] = useState({ isRunning: false });

    let state = {
        player: [player, SETplayer],
        cash: [cash, SETcash],
        assets: [assets, SETassets]
    };

    // **********
    // GO GETTERS
    // **********
    const updateState = changeSet => {
        if ((player.player || changeSet.player) && (cash.cash || changeSet.cash) && (assets.assets.length || changeSet.assets)) SETgotEm(true);
        console.log("state set: ", changeSet);
        SETplayer({ player: changeSet.player ? changeSet.player : player.player })
        SETcash({ cash: changeSet.csh ? changeSet.csh : cash.cash });
        SETassets({ assets: changeSet.asst ? assets.concat(changeSet.asst) : assets.assets });
    };
    const getChartData = cb => {
        API.gamePlay.getData().then(data => {
            if (data.status === 200) SETxSeries({ xSeries: data.data });
            let series = [];
            for (let point of data.data) {
                series.push(point.assets[0].value);
            }
            cb(series);
        })
    }
    const moveNext = cb => API.gamePlay.next({ frames: 5, trendiness: 5 }).then(data => cb(data.data));
    const getCash = cb => API.asset.getCash().then(data => cb(data.data));
    useEffect(() => getCash(cash => SETcash({ cash: cash.asset })),[]);

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
        SETassets({ assets: {} });
        SETgotEm(false);
        SETtradeButtonMessageDisplay(false);
        SETtradeButtonMessage("");
    }
    const assetButtonClick = () => SETviewPane(<AssetCreate updateState={updateState} state={state} />);
    const playerButtonClick = () => SETviewPane(<PlayerCreate updateState={updateState} state={state} />);
    const tradeButtonClick = () => SETviewPane(<Transaction updateState={updateState} state={state} />);
    const chartButtonClick = () => {
        getChartData(series => SETviewPane(<Chart xSeries={series} height={high} width={wide} state={state} />));
    }
    const startButtonCLick = () => {
        SETisRunning({ isRunning: !isRunning.isRunning });
        console.log("start button: ", isRunning.isRunning);
        do {
            console.log("pre-data");
            setTimeout(() => moveNext(data => {
                console.log("data: ", data);
            }),1000)
        } while (isRunning.isRunning);
        console.log("done");
    }
    const updateSeries = () => {
        console.log("asking");
        moveNext(series => {
            var newSeries = xSeries.xSeries;
            for (let entry of series) {
                newSeries.push(entry);
            }
            console.log("newSeries: ", newSeries);
        });

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
    const StartButton = () => <Button
        onClick={startButtonCLick}
        variant="light"
    >Start</Button>
    var wide = window.visualViewport.width * .8;
    var high = window.visualViewport.height * .8;

    return (
        <>
            <SelfDestructButton />
            <AssetButton />
            <PlayerButton />
            <TradeButton gotEm={gotEm} />
            <ChartButton />
            <StartButton />
            <div>{viewPane}</div>
        </>
    );
}
