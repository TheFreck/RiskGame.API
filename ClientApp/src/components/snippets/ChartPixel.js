import React, { useState, useEffect } from 'react';

export const ChartPixel = props => {
    const chartHeight = props.chartHeight;
    const width = props.chartWidth / props.seriesQty;
    const color = props.value.close > props.value.open ? "green" : "firebrick";
    const border = props.value.close > props.value.open ? "darkgreen" : "maroon";
    const seriesBreadth = props.seriesHigh - props.seriesLow;
    const cleanHigh = (props.value.high - props.seriesLow) / seriesBreadth;
    const cleanLow = (props.value.low - props.seriesLow) / seriesBreadth;
    const cleanOpen = (props.value.open - props.seriesLow) / seriesBreadth;
    const cleanClose = (props.value.close - props.seriesLow) / seriesBreadth;
    const boxTop = Math.max(cleanOpen, cleanClose);
    const boxBottom = Math.min(cleanOpen, cleanClose);
    const candleHeight = chartHeight * (boxTop - boxBottom);

    const pixelStyle = {
        height: `${chartHeight}px`,
        maxHeight: `${chartHeight}px`,
        width: `${width}px`,
        position: "absolute",
        left: `${props.id * width}px`
    }

    const Pixel = () => <div className="pixel" style={pixelStyle} >
        <PixelSpace height={chartHeight * (1 - cleanHigh)} />
        <CandleStickPixel height={chartHeight * (cleanHigh - cleanLow)} />
        <PixelSpace height={chartHeight * cleanLow} />
        <AxisLabel value={props.id} />
    </div>;
    const PixelSpace = pixSpProps => <div className="pixelSpace" style={{ height: pixSpProps.height }}></div>;
    const CandleStickPixel = candlePixProps => <div className="candlestick" style={{ height: candlePixProps.height }} >
        <Wick height={chartHeight * (cleanHigh - boxTop)} />
        <Candle />
        <Wick height={chartHeight * (boxBottom - cleanLow)} />
    </div>;
    const Wick = wickProps => <div className="wick" style={{ height: wickProps.height, width: '100%' }}>
        <LeftWick />
        <RightWick />
    </div>;
    const Candle = () => <div style={{ height: candleHeight, background: `${color}`, border: "solid", borderColor: `${border}` }} className="candle" >{props.value.volume}</div>;
    const LeftWick = () => <div style={{ borderColor: `${border}`, border: 'none solid none none', height: '100%' }} className="left-wick" ></div>;
    const RightWick = () => <div style={{ borderColor: `${border}`, border: 'none none none solid', height: '100%' }} className="right-wick" ></div>;
    const AxisLabel = () => <div className="x-label">{props.id}</div>
    return <Pixel />;
}

export default ChartPixel;