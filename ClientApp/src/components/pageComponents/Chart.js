import React, { useState, useEffect, useRef } from 'react';
import ChartPixel from './../snippets/ChartPixel';

export const Chart = props => {
    console.log("chart props: ", props);

    var theProps = {
        xSeries: props.xSeries,
        height: props.height,
        width: props.width,
    }
    const val = useRef();
    useEffect(
        () => {
            val.current = props;
            theProps = val.current;
        },
        [props]
    );


    const sizeChart = xseries => {
        debugger;
        let xMax = Math.max(...xseries);
        let xMin = Math.min(...xseries);
        let newSeries = [];
        for (let x of xseries) {
            newSeries.push(Math.floor((x - xMin)/(xMax - xMin)*100));
        }
        return newSeries;
    }

    const [xSeries, SETxSeries] = useState(sizeChart(theProps.xSeries));
    const [height, SETheight] = useState(theProps.height);
    const [width, SETwidth] = useState(theProps.width);

    useEffect(() => {
        console.log("xSeries size chart: ", sizeChart(xSeries));
    }, [xSeries]);

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

