import React, { useState, useEffect, useRef, useContext } from 'react';
import Chart from './Chart';
import ChartLoop from './ChartLoop';
import API from './../../API';
import { Context } from './../../stateManagement/Store';

export const ChartContainer = props => {
    console.log("Chart container load");
    // *****
    // STATE
    // *****
    const [state, dispatch] = useContext(Context);
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
    const [assets, SETassets] = useState(state.assets);
    useEffect(
        () => {
            assetsRef.current = assets;
            console.log("getting the gotten assets: ", assetsRef.current);
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
                    startTrading();
                });
                setTimeout(() => {
                    setChartView();
                }, 1000);
                let setChartView = () => SETview(<ChartLoop
                    isRunning={isRunning}
                    getData={getData}
                />);
            }
            if (isRunningRef.current != isRunning) onOff();
            //console.log("isRunning: ", isRunning);
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
    const startTrading = () => API.gamePlay.tradingOnOff({ gameId, isRunning });

    // **********
    // GO GETTERS
    // **********
    const justaSecRef = useRef(true);
    const [justaSec, SETjustaSec] = useState();
    useEffect(
        () => {
            justaSecRef.current = justaSec;
        },
        [justaSec]
    )
    const getData = cb => {
        console.log(`gameId: ${gameIdRef.current}; assetId: ${assetsRef.current}; lastFrame: ${lastFrameRef.current}`);
        debugger;
        API.gamePlay.getData({ gameId: gameIdRef.current, assetId: assetsRef.current[0], lastFrame: lastFrameRef.current }).then(data => {
            console.log("volume: ", data.data.volume);
            debugger;
            SETlastFrame(data.data.lastFrame)
            if (data.status === 200 && data.data.volume > 0) cb(data.data);
            else console.log("nothing came back");
        });
    }
    // **************
    // EVENT HANDLING
    // **************
    const startButtonClick = () => {
        //console.log("start button: ", isRunningRef.current);
        SETisRunning(!isRunningRef.current);
        
    }

    return <>
        <button id="start-stop" onClick={startButtonClick}>Start/Stop</button>
        {view}
    </>;
}

export default ChartContainer;

