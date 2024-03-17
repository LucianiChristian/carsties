'use client'

import { useSearchParamsStore } from '@/hooks/useSearchParamsStore'
import Link from 'next/link'
import { usePathname } from 'next/navigation'
import React from 'react'
import { AiOutlineCar } from 'react-icons/ai'

export default function Logo() {
    const pathname = usePathname();
    const searchParamsStore = useSearchParamsStore();

    function handleClick() {
       if(pathname === "/") searchParamsStore.reset() 
    }

    return (
        <Link href="/" onClick={handleClick}>
            <div className='flex items-center gap-2 text-3xl font-semibold text-red-500'>
                    <AiOutlineCar size={34}/>
                    <div>Carsties Auctions</div>
            </div>
        </Link>
    )
}
