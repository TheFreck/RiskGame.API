const ChartPixel = require("./ChartPixel");
const dataPacket = {
    open: 100.1,
    close: 100.2,
    high: 101.2,
    low: 99.9
}
const testProps = {
    key: 7,
    id: 7,
    value: dataPacket,
    seriesHigh: 101.8,
    seriesLow: 99.2,
    chartHeight: 900,
    chartWidth: 1000,
    seriesQty: 56
}

const ChartPixelTest = testProps => <ChartPixel
    key={testProps.key}
    id={testProps.id}
    value={testProps.value}
    seriesHigh={testProps.seriesHigh}
    seriesLow={testProps.seriesLow}
    chartHeight={testProps.chartHeight}
    chartWidth={testProps.chartWidth}
    sereisQty={testProps.seriesQty}
/>;

const Pixel = () => <div className="pixel" style={pixelStyle} >
    <div style={{ height: pixelProps.first }} />
    <div height={{ height: pixelProps.second }} />
    <div height={{ height: pixelProps.third }} />
</div>;

const pixelProps = {
    first: `${testProps.chartHeight * cleanHigh}px`,
    second: `${testProps.chartHeight * (cleanHigh - cleanLow)}px`,
    third: `${testProps.chartHeight * cleanLow}px`
};

const pixelStyle = {
    height: `900px`,
    width: `1000px`,
    position: "absolute",
    left: `${props.id * width}px`
}

const cleanLow = (dataPacket.low - testProps.seriesLow) / (testProps.seriesHigh - testProps.seriesLow);
const cleanHigh = (dataPacket.high - testProps.seriesLow) / (testProps.seriesHigh - testProps.seriesLow);


test("that it rendered correctly", () => {
    expect(<ChartPixelTest testProps={testProps} />).toBe(<Pixel />)
})