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
    useEffect(
        () => {
            isRunningRef.current = props.isRunning;
        },
        [props.isRunning]
    )
    // VIEW ****************************************
    const [view, SETview] = useState(<div />);


    // **********
    // GO GETTERS
    // **********
    const getData = cb => {
        return console.log("got data");
        API.gamePlay.getData(props.gameId).then(data => {
            console.log("got data: ", data);
            if (data.status === 200) cb(data.data);
        });
    }

    const ChartComponent = () => <ChartLoop
        isRunning={isRunningRef.current}
        gameId={gameIdRef.current}
        getData={getData}
    />

    return <>
        <h1>Chart</h1>
        <ChartComponent />
    </>;
}

export default ChartContainer;

