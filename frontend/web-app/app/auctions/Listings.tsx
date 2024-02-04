'use client'

import React, { useEffect, useState } from 'react'
import AuctionCard from './AuctionCard';
import AppPagination from './AppPagination';
import { getData } from '../actions/auctionActions';
import { Auction, PagedResult } from '@/types';
import { useParamsStore } from '@/hooks/useParamsStore';
import queryString from 'query-string';
import ListingsToolBar from './ListingsToolBar/Index';
import EmptyFilter from '../components/EmptyFilter';

export default function Listings() {
    const [ data, setData ] = useState<PagedResult<Auction>>();

    const params = useParamsStore(state => ({
        pageNumber: state.pageNumber,
        pageSize: state.pageSize,
        orderBy: state.orderBy,
        filterBy: state.filterBy,
        searchTerm: state.searchTerm,
    })); 

    const setParams = useParamsStore(state => state.setParams);
    const url = queryString.stringifyUrl({url: '', query: params});

    useEffect(() => {
        getData(url).then(data => {
            setData(data);
        });
    }, [url]);

    return (
        <>
            <div className="p-4">
                <ListingsToolBar/>
            </div>
            <ListingsResultView data={data} />
            <AppPagination 
                pageCount={ data?.pageCount ?? 0 } 
                currentPage={ params.pageNumber } 
                pageChanged={ pageNumber => setParams({ pageNumber }) } />
        </>
    )
}

function ListingsResultView({ data } : { data: PagedResult<Auction> | undefined }) {
    if(!data) return <h3>Loading...</h3>

    return ( 
        data.results.length === 0 ? 
            <EmptyFilter showReset /> : 
            <div className='grid grid-cols-2 md:grid-cols-4 gap-6'>
                {data.results.map(listing => 
                    <AuctionCard auction={ listing } key={ listing.id }/>
                )}
            </div>
    )
}