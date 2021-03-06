import React, { useState, useEffect, useRef } from 'react';
import ChartPixel from './../snippets/ChartPixel';
import VerticalChartAxis from './../snippets/VerticalChartAxis';

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
        },
        [props]
    );

    const [height, SETheight] = useState(window.visualViewport.height * .7);
    const [width, SETwidth] = useState(window.visualViewport.width * .8);


    let style = {
        chartContainer: {
            background: "darkgrey",
            width: "100vw",
            height: `${height}px`,
            position: "relative",
            padding: 0,
        }
    }

    return (
        <>
            <div className="chart-row">
                <VerticalChartAxis
                    chartTop={props.seriesHigh}
                    chartBottom={props.seriesLow}
                />
                <div className="chart" style={style.chartContainer}>
                    {
                        series.map((pixel, i) => 
                            <ChartPixel
                                key={i}
                                id={i}
                                value={pixel}
                                seriesHigh={props.seriesHigh}
                                seriesLow={props.seriesLow}
                                chartHeight={height}
                                chartWidth={width}
                                seriesQty={series.length}
                            />
                        )
                    }
                </div>
            </div>
        </>
    );
}

export default Chart;

