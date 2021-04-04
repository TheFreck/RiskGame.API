import React, { useState, useEffect, useRef } from 'react';
import AssetCreate from '../forms/AssetCreate';
import PlayerCreate from './../forms/PlayerCreate';
import Transaction from './../forms/Transaction';
import API from './../../API';
import './../../game.css';
import Button from 'react-bootstrap/Button';
import Chart from './../pageComponents/Chart';
import ChartContainer from './../pageComponents/ChartContainer';

export const GameHome = props => {
    const [gameId, SETgameId] = useState({ gameId: "" });
    const [player, SETplayer] = useState({ player: {} });
    const [cash, SETcash] = useState({ cash: {} });
    const [assets, SETassets] = useState({ assets: [] });
    const [assetsLoaded, SETassetsLoaded] = useState({ assetsLoaded: false });
    const [playersLoaded, SETplayersLoaded] = useState({ playersLoaded: false });
    const [xSeries, SETxSeries] = useState({ xSeries: [] });
    const [showPane, SETshowPane] = useState("");
    const [gotEm, SETgotEm] = useState(false);
    const [tradeButtonMessageDisplay, SETtradeButtonMessageDisplay] = useState(false);
    const [tradeButtonMessage, SETtradeButtonMessage] = useState("");
    const [viewPane, SETviewPane] = useState(<></>);
    const [isRunning, SETisRunning] = useState(false);
    const [isChartOn, SETisChartOn] = useState(false);
    const [chartPane, SETchartPane] = useState(<div />);
    const [activePane, SETactivePane] = useState(<div />);

    let state = {
        player: [player, SETplayer],
        cash: [cash, SETcash],
        assets: [assets, SETassets],
        gameId: [gameId, SETgameId],
        assetsLoaded: [assetsLoaded, SETassetsLoaded],
        playersLoaded: [playersLoaded, SETplayersLoaded],
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
    const getCash = (csh, cb) => {
        let gmid = "";
        if (gameId.gameId) gmid = gameId.gameId;
        else if (csh) gmid = csh;
        else return;
        API.asset.getCash(gmid).then(data => {
            cb(data.data)
        });
    }
    useEffect(() => {
        if (gameId.gameId) {
            getCash(gameId.gameId, cash => {
                if (cash.status == 200) console.log("yay it's cash: ", cash);
            });
        }
        SETviewPane(activePane);
    }, [gameId]);

    // **********
    // GO SETTERS
    // **********

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
    const getState = () => state;

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
    const assetButtonClick = gameid => {
        SETactivePane(<AssetCreate updateState={updateState} playerButtonClick={playerButtonClick} gameId={gameid} state={state} />);
        SETviewPane(<AssetCreate updateState={updateState} playerButtonClick={playerButtonClick} gameId={gameid} state={state} />);
        SETisChartOn(false);
    }
    const playerButtonClick = gameid => {
        SETactivePane(<PlayerCreate updateState={updateState} chartButtonClick={chartButtonClick} gameId={gameid} state={state} />);
        SETviewPane(<PlayerCreate updateState={updateState} chartButtonClick={chartButtonClick} gameId={gameid} state={state} />);
        SETisChartOn(false);
    }
    const tradeButtonClick = () => {
        SETactivePane(<Transaction updateState={updateState} state={state} />);
        SETviewPane(<Transaction updateState={updateState} state={state} />);
    }
    const chartButtonClick = gameid => {
        SETviewPane(<ChartContainer gameId={gameid} />);
        SETisChartOn(true);
    }
    const newGameClick = () => {
        API.gamePlay.newGame().then(game => {
            SETgameId({ gameId: game.data });
            chartButtonClick(game.data);
            //assetButtonClick(game.data);
            //getCash(game.data,cash => {
            //    updateCash(cash.asset);
            //})
        })
    }
    const updateCash = cash => {
        SETcash({ cash: cash });
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
    const NewGameButton = () => <Button
        onClick={newGameClick}
        variant="light"
    >New Game</Button>
    const ShowChart = () => {
        if (gameId) return <ChartContainer gameId={gameId} />;
        else return <NewGameButton />;
    }

    return <ShowChart />

    //return (
    //    <>
    //        {isChartOn ? "" : <NewGameButton />}
    //        <div>{viewPane}</div>
    //    </>
    //);
}
