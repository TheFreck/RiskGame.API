import React, { useState, useEffect, useRef } from 'react';
import Chart from './Chart';
import ChartLoop from './ChartLoop';
import API from './../../API';

export const ChartContainer = props => {

    // *****
    // STATE
    // *****
    // GAME ID *************************************
    const gameIdRef = useRef();
    const [gameId, SETgameId] = useState(props.gameId);
    useEffect(
        () => {
            gameIdRef.current = gameId;
        },
        []
    );
    const assetsRef = useRef();
    const [assets, SETassets] = useState(props.getAssets);
    useEffect(
        () => {
            assetsRef.current = assets;
        },
        [assets]
    )
    // IS RUNNING **********************************
    const isRunningRef = useRef(false);
    const [isRunning, SETisRunning] = useState(false);
    useEffect(
        () => {
            const onOff = () => {
                isRunningRef.current = isRunning
                API.gamePlay.onOff({ gameId: props.gameId, isRunning }).then(outcome => {
                    console.log("server running: ", outcome.data);
                });
                setTimeout(() => startTrading(), 1000);
            }
            if (isRunningRef.current != isRunning) onOff();
            console.log("isRunning: ", isRunning);
            SETview(<ChartLoop
                isRunning={isRunning}
                getData={getData}
            />);
        },
        [isRunning]
    )
    // LAST FRAME
    const lastFrameRef = useRef();
    const [lastFrame, SETlastFrame] = useState(0);
    useEffect(
        () => {
            lastFrameRef.current = lastFrame+1;
        },
        [lastFrame]
    )
    // VIEW ****************************************
    const [view, SETview] = useState(<div />);

    // ********
    // SERVICES
    // ********
    const startTrading = () => {
        var query = {};
        query.gameId = gameId;
        query.isRunning = isRunning;
        API.gamePlay.tradingOnOff(query);
    }

    // **********
    // GO GETTERS
    // **********
    const getData = cb => {
        API.gamePlay.getData({ gameId: gameIdRef.current, assetId: assetsRef.current[0], lastFrame: lastFrameRef.current }).then(data => {
            SETlastFrame(data.data.lastFrame)
            if (data.status === 200 && data.data.volume > 0) cb(data.data);
            else console.log("nothing came back");
        });
    }
    // **************
    // EVENT HANDLING
    // **************
    const startButtonClick = () => {
        console.log("start button: ", isRunningRef.current);
        SETisRunning(!isRunningRef.current);
        
    }

    return <>
        <button id="start-stop" onClick={startButtonClick}>Start/Stop</button>
        {view}
    </>;
}

export default ChartContainer;

