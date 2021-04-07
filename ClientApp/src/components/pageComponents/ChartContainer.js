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
            isRunningRef.current = isRunning;
        },
        [props,isRunning]
    )
    // LAST FRAME
    const lastFrameRef = useRef();
    const [lastFrame, SETlastFrame] = useState();
    useEffect(
        () => {
            lastFrameRef.current = lastFrame;
        },
        [lastFrame]
    )
    // VIEW ****************************************
    const [view, SETview] = useState(<div />);


    // **********
    // GO GETTERS
    // **********
    const getData = cb => {
        API.gamePlay.getData(gameIdRef.current, lastFrameRef.current).then(data => {
            console.log("got data: ", data);
            if (data.status === 200) cb(data.data);
        });
    }
    // **************
    // EVENT HANDLING
    // **************
    const startButtonClick = () => {
        SETisRunning(!isRunningRef.current);
        API.gamePlay.onOff(props.gameId).then(outcome => {
            console.log(outcome);
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

