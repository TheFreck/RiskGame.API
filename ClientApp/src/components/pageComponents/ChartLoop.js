import React, { useState, useEffect, useRef } from 'react';
import Chart from './Chart';

export const ChartLoop = props => {

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
    const [series, SETseries] = useState(seriesRef);
    useEffect(
        () => {
            seriesRef.current = series;
        },
        [series]
    )
    // VIEW ****************************************
    const [view, SETview] = useState(<div />);

    // *********
    // GAME LOOP
    // *********
    const bounce = running => {
        getData();
        if (isRunningRef.current) setTimeout(() => {
            if (isRunningRef.current) bounceBack();
        }, 500);
    }
    const bounceBack = () => {
        getData();
        if (isRunningRef.current) setTimeout(() => {
            if (isRunningRef.current) bounce();
        }, 500);
    }

    // **********
    // GO GETTERS
    // **********
    const getData = () => props.getData(data => SETseries(data));

    return <>
        <Chart
            series={seriesRef.current}
        />
    </>;
}

export default ChartLoop;