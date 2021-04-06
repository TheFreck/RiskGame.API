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
    const [series, SETseries] = useState([]);
    useEffect (
        () => {
            console.log("series updating: ", series);
            seriesRef.current = series;
        },
        [series]
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
    const getData = () => props.getData(data => {
        console.log("gotten data: ", data);
        SETseries(parseData(data));
        console.log("series set");
        SETrender(Math.random());
    });

    // ********
    // SERVICES
    // ********
    const parseData = dataArray => {
        debugger;
        let industries = [];
        let assetValue = [];
        for (let data of dataArray) {
            industries.push({
                red: data.red,
                orange: data.orange,
                yellow: data.yellow,
                green: data.green,
                blue: data.blue,
                violet: data.violet
            });

            let assetValues = [];
            let parsedAssets = [];
            for (let asset of data.assets) {
                // let name = asset.name; // name is not there yet
                let primary = asset.primaryIndustry;
                let secondary = asset.secondaryIndustry;
                let value = asset.value;
                assetValues.push(value);
                parsedAssets.push({ primary, secondary, value });
            }
            assetValue.push(data.assets[0].value);
        }

        return assetValue;
    }

    const ChartRender = () => {
        if (seriesRef.current !== undefined)
            return <Chart series={seriesRef.current} />;
        else
            return <Chart series={[0]} />
    }

    return <>
        <p>{render}</p>
        <ChartRender />
    </>;
}

export default ChartLoop;