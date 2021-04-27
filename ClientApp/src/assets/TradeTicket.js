import React, { Component, useState } from 'react';

export const TradeTicket = props => {
    const [buyer, setBuyer] = useState(props.buyer);
    const [seller, setSeller] = useState(props.seller);
    const [cash, setCash] = useState(props.cash);
    const [shares, setShares] = useState(props.shares);

    const getBuyer = () => buyer;
    const getSeller = () => seller;
    const getCash = () => cash;
    const getShares = () => shares;
    return { buyer, seller, cash, shares };
}

export default TradeTicket;