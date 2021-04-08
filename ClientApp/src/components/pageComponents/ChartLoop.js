import React, { useState, useEffect, useRef } from 'react';
import Chart from './Chart';

export const ChartLoop = props => {
    console.log("chart loop props.isRunning: ", props.isRunning);

    // *****
    // STATE
    // *****
    // IS RUNNING **********************************
    const isRunningRef = useRef();
    useEffect(
        () => {
            isRunningRef.current = props.isRunning;
            if (props.isRunning) bounce();
        },
        [props.isRunning]
    )
    // SERIES **************************************
    const seriesRef = useRef();
    const [series, SETseries] = useState([]);
    // APPEND SERIES *******************************
    const seriesAppendRef = useRef();
    const [seriesAppend, SETseriesAppend] = useState();
    // OPEN ****************************************
    const openRef = useRef([]);
    const [open, SETopen] = useState();
    // CLOSE ***************************************
    const closeRef = useRef([]);
    const [close, SETclose] = useState();
    // HIGH ****************************************
    const highRef = useRef([]);
    const [high, SEThigh] = useState();
    // LOW *****************************************
    const lowRef = useRef([]);
    const [low, SETlow] = useState();
    // ON SERIES APPEND UPDATE *********************
    useEffect(
        () => {
            console.log("series append: ", seriesAppend);
            seriesAppendRef.current = seriesAppend;
            let altSeries = series ? series : [];
            if (seriesAppend) {
                altSeries.push(seriesAppend);
                SETseries(altSeries);
                seriesRef.current = altSeries;
                openRef.current.push(seriesAppend.cleanOpen);
                closeRef.current.push(seriesAppend.cleanClose);
                highRef.current.push(seriesAppend.cleanHigh);
                lowRef.current.push(seriesAppend.cleanLow);
            }
        },
        [seriesAppend]
    )
    // VIEW ****************************************
    const [view, SETview] = useState(<div />);
    const [render, SETrender] = useState(Math.random());

    // *********
    // GAME LOOP
    // *********
    const bounce = running => {
        console.log("bounce");
        getData();
        if (isRunningRef.current) setTimeout(() => {
            return bounceBack();
        }, 1000);
    }
    const bounceBack = () => {
        console.log("bounce back");
        getData();
        if (isRunningRef.current) setTimeout(() => {
            return bounce();
        }, 1000);
    }

    // **********
    // GO GETTERS
    // **********
    const getData = () => props.getData(data => {
        console.log("gotten data: ", data);
        let sizeChartData = sizeChart(data);
        console.log("sizeChartData: ", sizeChartData);
        SETseriesAppend(sizeChartData);
        // is setting the random really necessary to cause a re-render?
        SETrender(Math.random());
    });

    // ********
    // SERVICES
    // ********
    const sizeChart = incoming => {
        let newHigh = high > incoming.high ? high : incoming.high;
        let newLow = low < incoming.low ? low : incoming.low;
        console.log("size chart new high: ", newHigh);
        console.log("size chart new low: ", newLow);
        let newSeries = [];
        for (let x of series) {
            let cleanOpen = Math.floor((x.open - newLow) / (newHigh - newLow) * 100);
            let cleanClose = Math.floor((x.close - newLow) / (newHigh - newLow) * 100);
            let cleanHigh = Math.floor((x.high - newLow) / (newHigh - newLow) * 100);
            let cleanLow = Math.floor((x.low - newLow) / (newHigh - newLow) * 100);
            newSeries.push({
                cleanOpen,
                cleanClose,
                cleanHigh,
                cleanLow,
                open: x.open,
                close: x.close,
                high: x.high,
                low: x.low
            });
        }
        return newSeries;
    }

    const ChartRender = () => {
        if (seriesRef.current !== undefined)
            return <Chart series={seriesRef.current} />;
        else
            return <Chart series={[]} />
    }

    return <>
        <p>{render}</p>
        <ChartRender />
    </>;
}

export default ChartLoop;