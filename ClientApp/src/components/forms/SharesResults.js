import React, { Component } from 'react';

export const SharesResults = props => {
    return (
        <div>
            <table>
                <thead>
                    <tr>
                        <td>Share Name</td>
                        <td>History</td>
                        <td>Current Owner</td>
                    </tr>
                </thead>
                <tbody>
                    {props.shares.map(share =>
                        <tr key={share.id}>
                            <td>{share.name}</td>
                            <td>{share.history}</td>
                            <td>{share.currentOwner}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
        );
}

export default SharesResults;