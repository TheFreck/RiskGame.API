import React, { useState, useEffect, useRef, useContext } from 'react';
import AssetCreate from '../forms/AssetCreate';
import PlayerCreate from './../forms/PlayerCreate';
import Transaction from './../forms/Transaction';
import API from './../../API';
import './../../game.css';
import Button from 'react-bootstrap/Button';
import Chart from './../pageComponents/Chart';
import ChartContainer from './../pageComponents/ChartContainer';
import Player from './../../assets/Player';
import { Context } from '../../stateManagement/Store';

export const GameHome = props => {
    const [state, dispatch] = useContext(Context);

    const [player, SETplayer] = useState({});
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
    // ASSET ID *************************************************
    const assetsRef = useRef([]);
    const [assets, SETassets] = useState();
    useEffect(
        () => {
            assetsRef.current = assets;
            dispatch({ type: 'SET_ASSETS', payload: assetsRef.current });
        },
        [assets]
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
    // VIEW PANE ************************************************
    const viewPaneRef = useRef();
    const [viewPane, SETviewPane] = useState(<></>);

    // **********
    // GO SETTERS
    // **********
    const getAssets = () => assetsRef.current;

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
            console.log(init.data);
        })
        SETplayer({ player: {} });
        SETassets({ assets: {} });
        SETgotEm(false);
        SETtradeButtonMessageDisplay(false);
        SETtradeButtonMessage("");
    }
    const gameOver = () => {
        API.gamePlay.gameOver(gameIdRef.current).then(answer => {
            console.log(answer);
        })
    }
    const assetButtonClick = gameid => {
        SETviewPane(<AssetCreate playerButtonClick={playerButtonClick} gameId={gameIdRef.current} state={state} />);
        SETisChartOn(false);
    }
    const playerButtonClick = gameid => {
        SETviewPane(<PlayerCreate chartButtonClick={chartButtonClick} gameId={gameIdRef.current} state={state} />);
        SETisChartOn(false);
    }
    const tradeButtonClick = () => {
        SETviewPane(<Transaction state={state} />);
    }
    const chartButtonClick = () => {
        SETviewPane(<ChartContainer gameId={gameIdRef.current} isRunning={isRunningRef.current} getAssets={getAssets}/>);
    }
    const newGameClick = () => {
        API.gamePlay.newGame(1).then(game => {
            console.log("new: ", game.data);
            SETassets(game.data.assets);
            SETisRunning(false);
            createPlayers(11, game.data.gameId);
        });
    }
    const restartClick = () => {
        // get game id's and restart
    }
    const randy = () =>  Math.random();
    const createPlayers = (num, gameid) => {
        let players = [];
        for (let i = 0; i < num; i++) {
            players.push({
                name: "Player_" + i,
                cash: Math.floor(100 + randy() * 100),
                riskTolerance: Math.floor(randy() * 100) / 100,
                experience: Math.floor(randy() * 2)
            });
        }
        API.player.createPlayers({ gameid, players }).then(answer => {
            SETgameId(answer.data[0].gameId);
        });
    }

    const tradeButtonMouseEnter = () => {
        SETtradeButtonMessage("Create an asset and a player to start");
        setTimeout(() => SETtradeButtonMessage(""), 3333);
    };
    const tradeButtonMouseLeave = () => {
        SETtradeButtonMessage("");
    };

    // ***************
    //  SUB COMPONENTS
    // ***************
    let initStyle = {
        "borderRadius": "50%",
        "borderColor": "darkred",
        "borderWidth": "5px"
    };
    const SelfDestructButton = () => <Button
        onClick={selfDestruct}
        style={initStyle}
        variant="danger"
    >Don't<br /> Press!<br />Or Else!!!</Button>;
    const GameOverButton = () => <Button
        onClick={gameOver}
        variant="light"
    >Game Over</Button>;
    const NewGameButton = () => <Button
        onClick={newGameClick}
        variant="light"
    >New Game</Button>;
    {
        //const TradeButton = gotEm => {
        //    if (gotEm.gotEm) {
        //        return <Button onClick={tradeButtonClick} variant="dark">Place a Trade</Button>;
        //    }
        //    else {
        //        return <Button onMouseEnter={tradeButtonMouseEnter} variant="secondary" disabled>Place a Trade</Button>;
        //    }
        //};
        //const AssetButton = () => <Button
        //    onClick={assetButtonClick}
        //    variant="light"
        //>Create an Asset</Button>;
        //const PlayerButton = () => <Button
        //    onClick={playerButtonClick}
        //    variant="light"
        //>Create a Player</Button>;
        //const RestartGame = () => <Button
        //    onClick={restartClick}
        //    variant="light"
        //>Restart Game</Button>;
    }
    return <>
        <SelfDestructButton />
        <GameOverButton />
        <NewGameButton />
        {viewPane}
    </>;
}

export default GameHome;