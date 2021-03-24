import React, { Component } from 'react';

export const SharesResults = props => {

    //let item = {
    //    body: ["string"]
    //}
    //let theProps = {
    //    columns: ["string"],
    //    items: [item]
    //}

    let TableRow = items => {
        return items.map((body, j) =>
            <tr key={'row' + j}>
                <p>table row</p>
                {<RowItems row={body.items} />}
            </tr>
        )
    }
    let RowItems = row => {
        return row.map((body, k) =>
            <td key={'item' + k}>
                <p>row items</p>
                {body}
            </td>)
    }

    return (
        <div>
            <table>
                <thead>
                    <tr>
                        {props.columns.map((header, i) => <td id={"header" + i} key={i}>{header}</td>)}
                    </tr>
                </thead>
                <tbody>
                    {
                        props.items.map((item, j) =>
                            <tr key={'row' + j}>
                                {item.body}
                                
                            </tr>)
                    }
                </tbody>
            </table>
        </div>
    );
}

export default SharesResults;