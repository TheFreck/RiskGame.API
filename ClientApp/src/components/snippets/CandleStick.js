import React, { useState, useEffect } from 'react';

export const CandleStick = props => {

    const [id, SETid] = useState(props.id);
    const [height, SETheight] = useState(props.value);
    const [width, SETwidth] = useState(findWidth());
    const [open, SETopen] = useState();
    const [close, SETclose] = useState();
    const [high, SEThigh] = useState();
    const [low, SETlow] = useState();
    useEffect(() => {
        setWidth();
    }, []);

    const findWidth = () => props.chartWidth / props.seriesQty;
    const setWidth = () => SETwidth(findWidth());
    let color = open > close ? "red" : "green";

    let style = {
        
        candlestick: {
            top: 0,
            background: "blue",
            width: `${width}px`,
            display: "grid",
            gridTemplateColumns: "50% 50%",
            color: color
        },
        candlestickTop: {
            width: `${width}px`,
            height: `${open > close ? high - open : high - close}%`
        },
        candlestickTopLeft: {
            borderRight: "solid",
            borderColor: `dark${color}`
        },
        candlestickTopRight: {
            borderLeft: "solid",
            borderColor: `dark${color}`
        },
        candlestickMiddle: {
            width: "100%",
            color: "inherit",
            borderColor: `dark${color}`
        },
        candlestickBottom: {
            width: "100%",
            color: "inherit"
        },
        candlestickBottomLeft: {
            borderRight: "solid",
            borderColor: `dark${color}`
        },
        candlestickBottomRight: {
            borderLeft: "solid",
            borderColor: `dark${color}`
        },
        pixelBottom: {
            height: `100%`,
            width: `${width}px`,
            bottom: 0,
            opacity: .9,
        }
    }

    return <>
        <div className="candlestick pixel-top" style={style.pixelTop}>
            <div className="candlestick-top" style={style.candlestickTop}>
                <div className="candlestick-top-left" style={style.candlestickTopLeft}>
                </div>
                <div className="candlestick-top-right" style={style.candlestickTopRight}>
                </div>
            </div>
            <div className="candlestick-middle">
                <div className="candlestick-middle-body" style={style.candlestickMiddle}>
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
        </div>
    </>
}

export default CandleStick;