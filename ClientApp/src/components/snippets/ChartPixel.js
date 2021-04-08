import React, { useState, useEffect } from 'react';

export const ChartPixel = props => {
    const findWidth = () => props.chartWidth / props.seriesQty;
    const setWidth = () => SETwidth(findWidth());
    
    const [id, SETid] = useState(props.id);
    const [height, SETheight] = useState(props.chartHeight);
    const [width, SETwidth] = useState(findWidth());
    const [open, SETopen] = useState();
    const [close, SETclose] = useState();
    const [high, SEThigh] = useState();
    const [low, SETlow] = useState();
    const [color, SETcolor] = useState();
    const [border, SETborder] = useState();
    useEffect(() => {
        setWidth();
        console.log("props: ", props);
        console.log("props.value: ", props.value);
        debugger;
        if (props.value.cleanOpen > props.value.cleanClose) {
            SETcolor("firebrick");
            SETborder("maroon");
        }
        else {
            SETcolor("green");
            SETborder("darkgreen");
        }
    }, []);

    let style = {
        pixel: {
            height: `${props.value.cleanHigh}%`,
            display: "inline-block",
            position: "relative"
        },
        candlestick: {
            top: 0,
            width: `${width}px`,
            height: `${props.value.cleanHigh - props.value.cleanLow}%`,
            position: "relative",
        },
        candlestickTop: {
            height: `${props.value.cleanOpen > props.value.cleanClose ? props.value.cleanHigh - props.value.cleanOpen : props.value.cleanHigh - props.value.cleanClose}%`,
            width: "100%",
            display: "grid",
            gridTemplateColumns: "50% 50%"
        },
        candlestickTopLeft: {
            borderRight: "solid",
            borderColor: border,
            width: "100%",
        },
        candlestickTopRight: {
            borderLeft: "solid",
            borderColor: border,
            width: "100%",
        },
        candlestickMiddle: {
            width: "100%",
            background: color,
            borderColor: border,
            border: "solid",
            height: `${props.value.cleanOpen > props.value.cleanClose ? props.value.cleanOpen - props.value.cleanClose : props.value.cleanClose - props.value.cleanOpen}%`
        },
        candlestickBottom: {
            width: "100%",
            height: `${props.value.cleanOpen > props.value.cleanClose ? props.value.cleanClose - props.value.cleanLow : props.value.cleanOpen - props.value.cleanLow}%`,
            display: "grid",
            gridTemplateColumns: "50% 50%"
        },
        candlestickBottomLeft: {
            width: "100%",
            borderRight: "solid",
            borderColor: border
        },
        candlestickBottomRight: {
            width: "100%",
            borderLeft: "solid",
            borderColor: border
        },
        pixelBottom: {
            height: `${props.value.cleanLow}%`,
            width: `${width}px`,
            bottom: 0,
            opacity: .9,
            position: "relative",
        }
    }

    return <>
        <div className="pixel" style={style.pixel}>
            <div className="candlestick" style={style.candlestick}>
                <div className="candlestick-top" style={style.candlestickTop}>
                    <div className="candlestick-top-left" style={style.candlestickTopLeft}>
                    </div>
                    <div className="candlestick-top-right" style={style.candlestickTopRight}>
                    </div>
                </div>
                <div className="candlestick-middle" style={style.candlestickMiddle}>
                    <div className="candlestick-middle-body">
                    </div>
                </div>
                <div className="candlestick-bottom" style={style.candlestickBottom}>
                    <div className="candlestick-bottom-left" style={style.candlestickBottomLeft}>
                    </div>
                    <div className="candlestick-bottom-right" style={style.candlestickBottomRight}>
                    </div>
                </div>
            </div>
            <div className="pixel-bottom" style={style.pixelBottom}>
                1--
                -2-
                --3
            </div>
        </div>
    </>
}

export default ChartPixel;