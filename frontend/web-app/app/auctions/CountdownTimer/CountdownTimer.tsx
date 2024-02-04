'use client'

import React from 'react'
import Countdown from 'react-countdown';
import Renderer from './Renderer';

type Props = {
    auctionEnd: string
};

export default function CountdownTimer({ auctionEnd }: Props) {
  return (
    <Countdown date={ auctionEnd } renderer={ Renderer }/>
  )
}