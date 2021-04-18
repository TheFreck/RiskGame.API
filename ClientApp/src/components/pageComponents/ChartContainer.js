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
    const isRunningRef = useRef(false);
    const [isRunning, SETisRunning] = useState(false);
    useEffect(
        () => {
            isRunningRef.current = isRunning;
            console.log("isRunning: ", isRunning);
            API.gamePlay.onOff({ gameId: props.gameId, isRunning }).then(outcome => {
                console.log("server running: ", outcome.data);
            });
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


    // **********
    // GO GETTERS
    // **********
    const getData = cb => {
        API.gamePlay.getData({ gameId: gameIdRef.current, lastFrame: lastFrameRef.current }).then(data => {
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

