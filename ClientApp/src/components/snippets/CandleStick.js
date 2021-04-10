import React from 'react';

export const CandleStick = props => {

    const height = props.chartHeight;
    const width = props.chartWidth / props.seriesQty;
    const color = props.value.close > props.value.open ? "green" : "firebrick";
    const border = props.value.close > props.value.open ? "darkgreen" : "maroon";
    const seriesBreadth = props.seriesHigh - props.value.seriesLow;
    const cleanHigh = (props.value.high - props.seriesLow) / seriesBreadth;
    const cleanLow = (props.value.low - props.seriesLow) / seriesBreadth;
    const cleanOpen = (props.value.open - props.seriesLow) / seriesBreadth;
    const cleanClose = (props.value.close - props.seriesLow) / seriesBreadth;
    const boxTop = Math.max(cleanOpen, cleanClose);
    const boxBottom = Math.min(cleanOpen, cleanClose);

    const pixelStyle = {
        height: `${height}px`,
        width: `${width}px`,
        position: "absolute",
        left: `${props.id * width}px`
    }

    const Pixel = () => <div className="pixel" style={pixelStyle} >
        <PixelSpace height={height * (1-cleanHigh)} />
        <CandleStickPixel height={ height * (cleanHigh-cleanLow)} />
        <PixelSpace height={height * cleanLow} />
    </div>;
    const PixelSpace = props => <div className="pixelSpace" style={{ height: `${props.height}px` }}></div>;
    const CandleStickPixel = () => <div className="candlestick">
        <Wick height={height * (cleanHigh - boxTop} />
        <Candle />
        <Wick height={height * (boxBottom - cleanLow} />
    </div>;
    const Wick = props => <div className="top-wick" style={{height: `${props.height}`, width: '100%'}}>
        <LeftWick />
        <RightWick />
    </div>;
    const Candle = () => <div style={{ height: `${height * (boxTop - boxBottom)}px`, background: `${color}`, borderColor: `${border}` }} className="candle" ></div>;
    const LeftWick = () => <div style={{ borderColor: `${border}`, border: 'none', borderRight: 'solid', height: '100%' }} className="left-wick" ></div>;
    const RightWick = () => <div style={{ borderColor: `${border}`, border: 'none', borderLeft: 'solid', height: '100%' }} className="right-wick" ></div>;
    return <Pixel />;
}

export default CandleStick;