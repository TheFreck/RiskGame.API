import React, { useState, useEffect, useRef } from 'react';
import Chart from './Chart';

export const ChartLoop = props => {
    console.log(`I am an ${props.isRunning ? "" : "in"}active chart loop`);
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
    // HIGH ****************************************
    const highRef = useRef();
    const [high, SEThigh] = useState(Number.MIN_SAFE_INTEGER);
    // LOW *****************************************
    const lowRef = useRef();
    const [low, SETlow] = useState(Number.MAX_SAFE_INTEGER);
    useEffect(
        () => {
            lowRef.current > low ? lowRef.current = low : lowRef.current = lowRef.current;
            highRef.current < high ? highRef.current = high : highRef.current = highRef.current;
        },
        [high,low]
    )
    // ON UPDATING SERIES APPEND *******************
    useEffect(
        () => {
            seriesAppendRef.current = seriesAppend;
            let altSeries = series ? series : [];
            if (seriesAppend) {
                altSeries.push(seriesAppend);
                SETseries(altSeries);
                setHighLow(altSeries, result => {
                    seriesRef.current = altSeries;
                    highRef.current = result.high;
                    lowRef.current = result.low;

                });
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
        getData();
        if (isRunningRef.current) setTimeout(() => {
            return bounceBack();
        }, 1000);
    }
    const bounceBack = () => {
        getData();
        if (isRunningRef.current) setTimeout(() => {
            return bounce();
        }, 1000);
    }

    // **********
    // GO GETTERS
    // **********
    const getData = () => props.getData(data => {
        SETseriesAppend(data);
        //// is setting the random really necessary to cause a re-render?
        //SETrender(Math.random());
    });

    // ********
    // SERVICES
    // ********
    const setHighLow = (incoming,cb) => {
        let newHigh, newLow;
        for (let item of incoming) {
            newHigh = highRef.current > item.high ? highRef.current : item.high;
            newLow = lowRef.current < item.low ? lowRef.current : item.low;
        }
        highRef.current && newHigh > highRef.current ? SEThigh(newHigh) : ChartRender();
        lowRef.current && newLow < lowRef.current ? SETlow(newLow) : ChartRender();
        SEThigh(newHigh);
        SETlow(newLow);
        cb({ high: newHigh, low: newLow });
    }

    const ChartRender = () => {
        if (seriesRef.current !== undefined)
            return <Chart series={seriesRef.current} seriesHigh={highRef.current} seriesLow={lowRef.current} />;
        else
            return <Chart series={[]} />
    }
    //return <></>
    return <>
        <p>{render}</p>
        <ChartRender />
    </>;
}

export default ChartLoop;