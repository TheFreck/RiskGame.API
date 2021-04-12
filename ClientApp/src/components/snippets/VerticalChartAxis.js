import React from 'react';
import { useState } from 'react';

export const VerticalChartAxis = props => {

    const [top, SETtop] = useState(props.chartTop);
    const [bottom, SETbottom] = useState(props.chartBottom);
    const TickMark = value => <div className="tick-mark">{value.value ? value.value : "--"}</div>;
    const increments = (props.chartTop - props.chartBottom) / 10;
    let ticks = [];
    for (let i = 0; i < 10; i++) {
        ticks.push(top - increments * i);
    }

    return <>
        <div className="vertical-chart-axis">
            {
                ticks.map(mark => <TickMark value={mark} />)
            }
        </div>
    </>;

}

export default VerticalChartAxis;