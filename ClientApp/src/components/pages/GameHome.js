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
    const [player, SETplayer] = useState({});
    const [assets, SETassets] = useState([]);
    const [assetsLoaded, SETassetsLoaded] = useState(false);
    const [playersLoaded, SETplayersLoaded] = useState(false);
    const [gotEm, SETgotEm] = useState(false);
    const [tradeButtonMessageDisplay, SETtradeButtonMessageDisplay] = useState(false);
    const [tradeButtonMessage, SETtradeButtonMessage] = useState("");
    const [isChartOn, SETisChartOn] = useState(false);
    const [chartPane, SETchartPane] = useState(<div />);


    // **********
    // GO GETTERS
    // **********
    const updateState = changeSet => {
        if ((player || changeSet.player) && (cash || changeSet.cash) && (assets.length || changeSet.assets)) SETgotEm(true);
        SETplayer(changeSet.player ? changeSet.player : player.player);
        SETcash(changeSet.csh ? changeSet.csh : cash.cash);
        SETassets(changeSet.asst ? assets.concat(changeSet.asst) : assets.assets);
    };
    // GAME ID **************************************************
    const gameIdRef = useRef();
    const [gameId, SETgameId] = useState();
    useEffect(
        () => {
            if (gameId !== gameIdRef.current) {
                gameIdRef.current = gameId;
                SETviewPane(<ChartContainer gameId={gameIdRef.current} isRunning={isRunningRef.current} />);
            }
            else return;
        },
        [gameId]
    )
    // IS RUNNING ***********************************************
    const isRunningRef = useRef();
    const [isRunning, SETisRunning] = useState();
    useEffect(
        () => {
            isRunningRef.current = isRunning;
        },
        [isRunning]
    )
    // CASH *****************************************************
    const cashRef = useRef();
    const [cash, SETcash] = useState({ cash: {} });
    useEffect(
        () => {
            if(cashRef.current != cash) cashRef.current = cash;
            else if (gameIdRef.current) {
                getCash(gameIdRef.current, cash => {
                });
            }
        }, [gameId]
    );
    const getCash = (csh, cb) => {
        let gmid = "";
        if (gameId) gmid = gameId;
        else if (csh) gmid = csh;
        else return;
        API.asset.getCash(gmid).then(data => {
            cb(data.data)
        });
    }
    // VIEW PANE ************************************************
    const viewPaneRef = useRef();
    const [viewPane, SETviewPane] = useState(<></>);

    // **********
    // GO SETTERS
    // **********
    let state = {
        player: [player, SETplayer],
        cash: [cash, SETcash],
        assets: [assets, SETassets],
        gameId: [gameId, SETgameId],
        assetsLoaded: [assetsLoaded, SETassetsLoaded],
        playersLoaded: [playersLoaded, SETplayersLoaded],
    };

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
        API.gamePlay.initialize("badBoyNeedSpank").then(init => {
            console.log("init: ", init);
        })
        SETplayer({ player: {} });
        SETcash({ cash: {} });
        SETassets({ assets: {} });
        SETgotEm(false);
        SETtradeButtonMessageDisplay(false);
        SETtradeButtonMessage("");
    }
    const gameOver = () => {
        API.gamePlay.gameOver(gameIdRef.current).then(answer => {
        })
    }
    const assetButtonClick = gameid => {
        SETviewPane(<AssetCreate updateState={updateState} playerButtonClick={playerButtonClick} gameId={gameIdRef.current} state={state} />);
        SETisChartOn(false);
    }
    const playerButtonClick = gameid => {
        SETviewPane(<PlayerCreate updateState={updateState} chartButtonClick={chartButtonClick} gameId={gameIdRef.current} state={state} />);
        SETisChartOn(false);
    }
    const tradeButtonClick = () => {
        SETviewPane(<Transaction updateState={updateState} state={state} />);
    }
    const chartButtonClick = () => {
        SETviewPane(<ChartContainer gameId={gameIdRef.current} isRunning={isRunningRef.current} />);
    }
    const newGameClick = () => {
        API.gamePlay.newGame().then(game => {
            SETgameId(game.data);
            SETisRunning(false);
            createAssets(1, game.data);
        })
    }
    const createAssets = (num, gameid) => {
        num--;
        if (num >= 0) API.asset.createAsset({
            Name: "Asset_" + num,
            SharesOutstanding: 100,
            GameId: gameid
        })
    }

    const tradeButtonMouseEnter = () => {
        SETtradeButtonMessage("Create an asset and a player to start");
        setTimeout(() => SETtradeButtonMessage(""), 3333);
    }
    const tradeButtonMouseLeave = () => {
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
    const GameOverButton = () => <Button
        onClick={gameOver}
        variant="light"
    >Game Over</Button>;
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
    const NewGameButton = () => <Button
        onClick={newGameClick}
        variant="light"
    >New Game</Button>

    return <>
        <SelfDestructButton />
        <GameOverButton />
        <NewGameButton />
        {viewPane}
    </>
}

export default GameHome;