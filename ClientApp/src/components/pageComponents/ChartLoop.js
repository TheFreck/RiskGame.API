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
    const [series, SETseries] = useState([{}]);
    // APPEND SERIES *******************************
    const seriesAppendRef = useRef();
    const [seriesAppend, SETseriesAppend] = useState([]);
    const openRef = useRef([]);
    const [open, SETopen] = useState([]);
    const closeRef = useRef([]);
    const [close, SETclose] = useState([]);
    const highRef = useRef([]);
    const [high, SEThigh] = useState([]);
    const lowRef = useRef([]);
    const [low, SETlow] = useState([]);
    useEffect(
        () => {
            seriesAppendRef.current = seriesAppend;
            let altSeries = series;
            altSeries.push(seriesAppend);
            SETseries(series);
            seriesRef.current = series;
            openRef.current.push(series.cleanOpen);
            closeRef.current.push(series.cleanClose);
            highRef.current.push(series.cleanHigh);
            lowRef.current.push(series.cleanLow);
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
        getData();
        if (isRunningRef.current) setTimeout(() => {
            if (isRunningRef.current) bounceBack();
        }, 1000);
    }
    const bounceBack = () => {
        getData();
        if (isRunningRef.current) setTimeout(() => {
            if (isRunningRef.current) bounce();
        }, 1000);
    }

    // **********
    // GO GETTERS
    // **********
    const getData = () => props.getData(data => {
        console.log("gotten data: ", data);
        SETseries(data);
        // is setting the random really necessary to cause a re-render?
        SETrender(Math.random());
    });

    // ********
    // SERVICES
    // ********
    const sizeChart = seriesIn => {
        let open, close, high, low = 0;
        for (let frame of seriesIn) {
            open = frame.open > open ? frame.open : open;
            close = frame.close > close ? frame.close : close;
            high = frame.high > high ? frame.high : high;
            low = frame.low > low ? frame.low : low;
        }
        let newSeries = [];
        for (let x of seriesIn) {
            newSeries.push({
                cleanOpen: Math.floor((x.open - low) / (high - low) * 100),
                cleanClose: Math.floor((x.close - low) / (high - low) * 100),
                cleanHigh: Math.floor((x.high - low) / (high - low) * 100),
                cleanLlow: Math.floor((x.low - low) / (high - low) * 100),
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