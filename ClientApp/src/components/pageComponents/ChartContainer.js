import React, { useState, useEffect, useRef } from 'react';
import Chart from './Chart';
import API from './../../API';

export const ChartContainer = props => {
    console.log("chart container props: ", props);

    var theProps = {
        gameId: props.gameId,
    }
    const val = useRef();
    useEffect(
        () => {
            val.current = props;
            console.log("chart container val.current: ", val.current);
            SETgameId(val.current);
            getChartData(val.current.gameId, data => {
                console.log("chart container use effect data: ", data);
                SETxSeries(data);
            })
        },
        [props]
    );

    const [xSeries, SETxSeries] = useState([]);
    const [isRunning, SETisRunning] = useState(false);
    const [gameId, SETgameId] = useState(theProps.gameId);

    // **********
    // GO GETTERS
    // **********
    const isOn = cb => API.gamePlay.isGameOn(gameId.gameId).then(gameOn => cb(gameOn.data));
    const getChartData = (gameid, cb) => {
        API.gamePlay.getData(gameid).then(data => {
            console.log("game play get data: ", data);
            if (data.status === 200) SETxSeries({ xSeries: data.data });
            let series = [];
            for (let point of data.data) {
                series.push(point.assets[0].value);
            }
            cb(series);
        })
    }

    // **********
    // GO SETTERS
    // **********

    // ********
    // SERVICES
    // ********
    const startChart = () => {
        SETisRunning(!isRunning);
        console.log("start chart gameId: ", gameId);
        API.gamePlay.onOff(gameId.gameId).then(outcome => {
            console.log("outcome: ", outcome);
            if (outcome.status === 200 && outcome.data) runChart();
        });
        
    }


    const runChart = () => {
        console.log("run chart");
        API.gamePlay.isGameOn(gameId.gameId).then(gameOn => {
            debugger;
            console.log("gameOn: ", gameOn);
            if (gameOn.status === 200 && gameOn.data) gameLoop();
            else return;
        })
    }

    const gameLoop = () => {
        console.log("beginning of do loop. time: ", Date.now());
        API.gamePlay.getData(gameId.gameId).then(game => {
            debugger;
            setTimeout(() => {
                debugger;
                    SETxSeries(game.data);
                    console.log("chart data: ", game.data);
                    runChart();
                },1000);
        })
    }

    let TheChart = props => {
        if (xSeries.length > 0) {
            return <Chart
                xSeries={xSeries}
                height={high}
                width={wide}
            />;
        }
        else return "";
    }

    let wide = window.visualViewport.width * .8;
    let high = window.visualViewport.height * .8;

    return (
        <>
            <h1>Work on the JS game loop</h1>
            <button id="start-stop" onClick={startChart}>{isRunning ? "Stop" : "Start"}</button>
            <TheChart />
        </>
    );
}

export default ChartContainer;

