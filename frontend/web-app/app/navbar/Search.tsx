'use client'

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input'
import { useSearchParamsStore } from '@/hooks/useSearchParamsStore';
import React, { KeyboardEvent, MouseEvent, useEffect, useState } from 'react'
import { FaSearch } from 'react-icons/fa';

export default function Search() {
    const setParams = useSearchParamsStore(state => state.setParams); 
    const { searchTerm } = useSearchParamsStore(state => state);

    const [ value, setValue ] = useState("");

    useEffect(() => {
        setValue(searchTerm);
    }, [searchTerm])

    function handleButtonSubmit(event: MouseEvent<HTMLButtonElement>): void {
        setParams({searchTerm: value});
    }

    function handleInputSubmit(event: KeyboardEvent<HTMLInputElement>) {
        if(event.key === "Enter") {
            setParams({searchTerm: value});
        }
    }

    return (
        <div className="flex items-center gap-1 w-80">
            <Input type="search" 
                   placeholder="Search by make, model, or color" 
                   onKeyUp={handleInputSubmit}
                   value={value}
                   onChange={e => setValue(e.target.value)}/>  
            <Button variant={"ghost"} size="icon" onClick={handleButtonSubmit}>
                <FaSearch />
            </Button>
        </div>
    )
}
