"use client"

import { useSearchParamsStore } from '@/hooks/useSearchParamsStore';
import React from 'react'
import Heading from './Heading';
import { Button } from '@/components/ui/button';
import { signIn } from 'next-auth/react';

type Props = {
    title?: string;
    subtitle?: string;
    showReset?: boolean;
    showLogin?: boolean;
    callbackUrl?: string;
};

export default function EmptyFilter({
    title = 'No matches for this filter', 
    subtitle = 'Try changing or resetting the filter', 
    showReset,
    showLogin,
    callbackUrl
}: Props) {
    const reset = useSearchParamsStore(state => state.reset);

    return (
        <div className='h-[40vh] flex flex-col gap-2 justify-center items-center shadow-lg'>
            <Heading title={title} subtitle={subtitle} center />
            <div className="mt-4">
                {showReset && (
                    <Button variant={'outline'} onClick={reset}>
                        Remove Filters
                    </Button>
                )}
                {showLogin && (
                    <Button variant={'outline'} onClick={() => signIn("id-server", {callbackUrl})}>
                        Login
                    </Button>
                )}
            </div>
        </div>
    )
}
