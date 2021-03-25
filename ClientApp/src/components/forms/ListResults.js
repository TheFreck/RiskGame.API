import React, { useState } from 'react';

export const ListResults = props => {
    console.log("listResults props: ", props);

    const [tableName, SETtableName] = useState(props.tableName);
    //let item = {
    //    body: ["string"]
    //}
    //let theProps = {
    //    columns: ["string"],
    //    items: [item]
    //}

    let style = {
        tableRow: {
            width: "100vw",
            margin: "0 2vw",
        },
        tableData: {
            //width: props.style.dataWidth ? props.style.dataWidth : "20vw",
            background: props.style.background ? props.style.background : "rgb(252, 252, 252)",
            color: props.style.color ? props.style.color : "rgb(23, 31, 16)",
            borderLeft: props.style.borderLeft ? props.style.borderLeft : "inset",
            borderBottom: props.style.borderBottom ? props.style.borderBottom : "inset",
            borderWidth: props.style.borderWidth ? props.style.borderWidth : "3px",
            paddingLeft: props.style.paddingLeft ? props.style.paddingLeft : "5vw",
            paddingTop: props.style.paddingTop ? props.style.paddingTop : "0px",
            paddingRight: props.style.paddingRight ? props.style.paddingRight : "4vw",
        },
        tableHeaders: {
            borderLeft: props.style.borderLeft ? props.style.borderLeft : "inset",
            borderBottom: props.style.borderBottom ? props.style.borderBottom : "inset",
            borderWidth: props.style.borderWidth ? props.style.borderWidth : "3px",
            paddingLeft: props.style.paddingLeft ? props.style.paddingLeft : "5vw",
            paddingTop: props.style.paddingTop ? props.style.paddingTop : "0px",
            paddingRight: props.style.paddingRight ? props.style.paddingRight : "4vw",
        }
    }

    let TableFill = rows => {
        console.log("tabfill rows: ", rows.rows);
        return(<>
            {
                rows.rows.map((row, j) =>
                    <tr className="table-row" style={style.tableRow} id={tableName + "-row-" + j} key={'row' + j}>
                    <TableRow
                        row={row}
                    />
                </tr>)
        }
        </>)
    }

    let TableRow = row => {
        console.log("table row row: ", row.row.body);
        return (<>
            {
                row.row.body.map((data, k) =>
                    <td className="table-data" style={style.tableData} id={tableName + "-td-" + k} key={'data' + k}>{data}</td>)
            }
        </>);
    }


    return (
        <div>
            <table>
                <thead>
                    <tr>
                        {props.columns.map((header, i) => <td style={style.tableHeaders} className="table-headers" id={"header" + i} key={i}>{header}</td>)}
                    </tr>
                </thead>
                <tbody>
                    <TableFill
                        rows={props.items}
                    />
                </tbody>
            </table>
        </div>
    );
}

export default ListResults;