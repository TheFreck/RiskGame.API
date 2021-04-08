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
    const [series, SETseries] = useState(props.series);
    useEffect(
        () => {
            seriesRef.current = props.series;
            //console.log("seriesRef.current: ", seriesRef.current);
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

