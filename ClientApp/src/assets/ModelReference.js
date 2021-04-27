import React, { Component, useState } from 'react';

export const ModelReference = props => {
    const [name, setName] = useState(props.name);
    const [id, setId] = useState(props.id);
    const [type, setType] = useState(props.type);

    const getName = () => name;
    const getId = () => id;
    const getType = () => type;
    return { name, id, type };
}