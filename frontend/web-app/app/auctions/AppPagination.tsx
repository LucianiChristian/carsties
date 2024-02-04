'use client'

import { Pagination, PaginationContent, PaginationLink, PaginationNext, PaginationPrevious } from '@/components/ui/pagination';

type Props = {
    pageCount: number;
    currentPage: number;
    pageChanged: (pageNumber: number) => void;
};

export default function AppPagination({ pageCount, currentPage , pageChanged } : Props) {
    function updatePage(pageNumber: number) {
        if(pageNumber <= 0 || pageNumber > pageCount) return;

        pageChanged(pageNumber);
    }

    const previousPage = () => updatePage(currentPage - 1);
    const nextPage = () => updatePage(currentPage + 1);

    function getPaginationLinks(pageCount: number) {
        const paginationNumbers = [];

        for(let pageNumber = 1; pageNumber <= pageCount; pageNumber++) {
            paginationNumbers[pageNumber] = (
                <PaginationLink 
                    key={pageNumber}
                    onClick={ () => pageChanged(pageNumber) }
                    isActive={ currentPage === pageNumber }>
                    { pageNumber }
                </PaginationLink> 
            )
        }

        return paginationNumbers;
    }

    return (
        <Pagination className="p-4">
            <PaginationContent>
                <PaginationPrevious onClick={ () => previousPage() }/>
                { getPaginationLinks(pageCount) } 
                <PaginationNext onClick={ () => nextPage() }/>
            </PaginationContent>
        </Pagination>
    )
}
