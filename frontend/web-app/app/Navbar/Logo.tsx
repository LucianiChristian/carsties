'use client'

import { useParamsStore } from '@/hooks/useParamsStore'
import React from 'react'
import { AiOutlineCar } from 'react-icons/ai'

export default function Logo() {
    const resetParams = useParamsStore(state => state.reset);

    return (
        <div className='flex items-center gap-2 text-3xl font-semibold text-red-500'
            onClick={resetParams}>
            <AiOutlineCar size={34}/>
            <div>Carsties Auctions</div>
        </div>
    )
}