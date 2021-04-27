import React, { Component, useState } from 'react';

export const Player = props => {
    const [name, setName] = useState(props.name);
    const [gameId, setId] = useState(props.id);
    const [cash, setCash] = useState(props.type);
    const [risk, setRisk] = useState(props.risk);

    const getName = () => name;
    const getgameId = () => gameId;
    const getCash = () => cash;
    const getRisk = () => risk;
    return { name, gameId, cash, risk };
}