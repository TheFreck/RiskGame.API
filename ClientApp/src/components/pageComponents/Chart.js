import React, { useState, useEffect, useRef } from 'react';
import ChartPixel from './../snippets/ChartPixel';

export const Chart = props => {
    console.log("chart props: ", props);

    var theProps = {
        series: props.series,
    }
    // *****
    // STATE
    // *****
    // SERIES **********************************
    const seriesRef = useRef();
    const [series, SETseries] = useState([]);
    useEffect(
        () => {
            seriesRef.current = props.series;
        },
        [props]
    );

    const sizeChart = series => {
        let xMax = Math.max(...series);
        let xMin = Math.min(...series);
        let newSeries = [];
        for (let x of series) {
            newSeries.push(Math.floor((x - xMin)/(xMax - xMin)*100));
        }
        return newSeries;
    }

    const [height, SETheight] = useState(window.visualViewport.height * .8);
    const [width, SETwidth] = useState(window.visualViewport.width * .8);

    useEffect(() => {
        console.log("series size chart: ", sizeChart(series));
    }, [series]);

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

