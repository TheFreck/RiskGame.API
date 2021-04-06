import React, { useState, useEffect, useRef } from 'react';
import ChartPixel from './../snippets/ChartPixel';

export const Chart = props => {
    console.log("chart props: ", props);

    var theProps = {
        series: props.series,
    }
    const sizeChart = series => {
        let xMax = Math.max(...series);
        let xMin = Math.min(...series);
        let newSeries = [];
        for (let x of series) {
            newSeries.push(Math.floor((x - xMin) / (xMax - xMin) * 100));
        }
        return newSeries;
    }
    // *****
    // STATE
    // *****
    // SERIES **********************************
    const seriesRef = useRef();
    const [series, SETseries] = useState(sizeChart(props.series));
    useEffect(
        () => {
            seriesRef.current = sizeChart(props.series);
            console.log("seriesRef.current: ", seriesRef.current);
        },
        [props]
    );


    const [height, SETheight] = useState(window.visualViewport.height * .8);
    const [width, SETwidth] = useState(window.visualViewport.width * .8);


    let style = {
        background: "darkgrey",
        width: "100vw",
        height: `${height}px`,
        position: "relative",
        padding: 0,
    }

    return (
        <>
            <div className="chart-container" style={style}>
                {
                    series.map((pixel, i) => 
                        <ChartPixel
                            key={i}
                            id={i}
                            value={pixel}
                            chartHeight={height}
                            chartWidth={width}
                            seriesQty={series.length}
                        />
                    )
                }
            </div>
        </>
    );
}

export default Chart;

