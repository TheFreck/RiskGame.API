import React, { useState, useEffect } from 'react';

export const ChartPixel = props => {
    var theProps = {
        id: Number,
        value: Number,
        chartHeight: Number,
        chartWidth: Number,
        seriesQty: Number,
    }

    const [id, SETid] = useState(props.id);
    const [value, SETvalue] = useState(props.value);
    const [height, SETheight] = useState(props.value);
    const [width, SETwidth] = useState(2);
    const [display, SETdisplay] = useState(0);

    useEffect(() => {
        setWidth();
    }, []);

    const findWidth = () => props.chartWidth / props.seriesQty;
    const setWidth = () => SETwidth(findWidth());

    let style = {
        pixel: {
            height: `${height}%`,
            width: `${width}px`,
            display: "inline-block",
            position: "absolute",
            bottom: 0,
            left: `${id * width}px`
        },
        pixelTop: {
            height: "2px",
            top: 0,
            background: "blue",
            width: `${width}px`,
        },
        pixelBottom: {
            height: `100%`,
            width: `${width}px`,
            bottom: 0,
            background: "orange",
            opacity: .9,
        }
    }

    return (
        <>
            <div className="chart-pixel" style={style.pixel} id={props.id}>
                <div className="pixel-top" style={style.pixelTop}></div>
                <div className="pixel-bottom" style={style.pixelBottom}></div>
            </div>
        </>
        );
}

export default ChartPixel;