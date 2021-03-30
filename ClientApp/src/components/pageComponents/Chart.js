import React, { useState, useEffect } from 'react';
import ChartPixel from './../snippets/ChartPixel';

export const Chart = props => {
    var theProps = {
        xSeries: [Number],
        height: Number,
        width: Number
    }

    const sizeChart = () => {
        let xMax = Math.max(...props.xSeries);
        let xMin = Math.min(...props.xSeries);
        let newSeries = [];
        for (let x of props.xSeries) {
            newSeries.push(Math.floor((x - xMin)/(xMax - xMin)*100));
        }
        return newSeries;
    }

    const [xSeries, SETxSeries] = useState(sizeChart);
    const [height, SETheight] = useState(props.height);
    const [width, SETwidth] = useState(props.width);

    useEffect(() => {
        console.log("xSeries: ", sizeChart());
    }, [xSeries]);



    let style = {
        background: "darkgrey",
        //width: `${width}px`,
        width: "100vw",
        height: `${height}px`,
        position: "relative",
        padding: 0,
    }

    return (
        <>
            <div className="chart-container" style={style}>
                {
                    xSeries.map((pixel, i) => 
                        <ChartPixel
                            key={i}
                            id={i}
                            value={pixel}
                            chartHeight={height}
                            chartWidth={width}
                            seriesQty={xSeries.length}
                        />
                    )
                }
            </div>
        </>
    );
}

export default Chart;

