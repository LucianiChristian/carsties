import * as React from "react"

import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { AuctionFilter, AuctionOrdering } from "@/types";
import { useSearchParamsStore } from "@/hooks/useSearchParamsStore";
import { FaFilter } from "react-icons/fa6";

export function FiltersDropdown() {
    const { orderBy, filterBy } = useSearchParamsStore(state => state);
    const setParams = useSearchParamsStore(state => state.setParams);

    function handleOrderChange(value: string) {
        setParams({ orderBy: value as AuctionOrdering}) ;
    }

    function handleFilterChange(value: string) {
        setParams({ filterBy: value as AuctionFilter}) ;
    }

    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button variant="outline" className="flex justify-between align-center gap-[0.7em]"> <FaFilter/>Filters</Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent>
                <DropdownMenuLabel>Order By</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuRadioGroup value={ orderBy ?? "" } onValueChange={ handleOrderChange }>
                    <OrderButtons />
                </DropdownMenuRadioGroup>
                <DropdownMenuLabel>Filter By</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuRadioGroup value={ filterBy ?? "" } onValueChange={ handleFilterChange }>
                    <FilterButtons />
                </DropdownMenuRadioGroup>
            </DropdownMenuContent>
        </DropdownMenu>
    )
}

function OrderButtons() {
    return (
        <>
            <DropdownMenuRadioItem key={ AuctionOrdering.Make } value={ AuctionOrdering.Make }>
                Alphabetical
            </DropdownMenuRadioItem>
            <DropdownMenuRadioItem key={ AuctionOrdering.CreatedAt} value={ AuctionOrdering.CreatedAt}>
                Recently Added
            </DropdownMenuRadioItem>
            <DropdownMenuRadioItem key={ AuctionOrdering.AuctionEnd} value={ AuctionOrdering.AuctionEnd}>
                End Date
            </DropdownMenuRadioItem>
        </>
    )
}

function FilterButtons() {
    return (
        <>
            <DropdownMenuRadioItem key={ AuctionFilter.EndingSoon } value={ AuctionFilter.EndingSoon }>
                {'Ending < 6 Hours'}
            </DropdownMenuRadioItem>
            <DropdownMenuRadioItem key={ AuctionFilter.Finished } value={ AuctionFilter.Finished }>
                Completed
            </DropdownMenuRadioItem>
            <DropdownMenuRadioItem key={ AuctionFilter.Live } value={ AuctionFilter.Live }>
                Live Auctions
            </DropdownMenuRadioItem>
        </>
    )
}