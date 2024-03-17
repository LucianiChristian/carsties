'use client'

import { Button } from '@/components/ui/button'
import React, { useState } from 'react'
import { updateAuctionTest } from '../actions/auctionActions'

export default function AuthTest() {
    const [resultMsg, setResultMsg] = useState<string | null>(null);

    async function handleClick() {
        const message = await updateAuctionTest();
        setResultMsg(message);
    }

    return (
        <div className='flex items-center gap-4 mt-4 outline-'>
            <Button onClick={handleClick}>Send Auction Update Test</Button>
            <p>{resultMsg}</p>
        </div>
    )
}
