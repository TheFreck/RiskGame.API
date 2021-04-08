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
    // IS RUNNING **********************************
    const isRunningRef = useRef();
    const [isRunning, SETisRunning] = useState(true);
    useEffect(
        () => {
            console.log("setting isRunningRef: ", isRunningRef.current);
            isRunningRef.current = isRunning;
            console.log("set isRunningRef: ", isRunningRef.current);
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


    // **********
    // GO GETTERS
    // **********
    const getData = cb => {
        API.gamePlay.getData({ gameId: gameIdRef.current, lastFrame: lastFrameRef.current }).then(data => {
            //console.log("got data: ", data.data);
            SETlastFrame(data.data.lastFrame)
            if (data.status === 200) cb(data.data);
        });
    }
    // **************
    // EVENT HANDLING
    // **************
    const startButtonClick = () => {
        console.log("flipping start switch: ", isRunningRef.current);
        SETisRunning(!isRunningRef.current);
        console.log("flipped start switch: ", isRunningRef.current);
        API.gamePlay.onOff(props.gameId).then(outcome => {
            console.log("on or off: ", outcome);
        });
    }

    return <>
        <h1>Chart</h1>
        <button id="start-stop" onClick={startButtonClick}>Start/Stop</button>
        <ChartLoop
            isRunning={isRunningRef.current}
            getData={getData}
        />
    </>;
}

export default ChartContainer;

