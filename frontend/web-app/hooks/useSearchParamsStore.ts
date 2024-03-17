import { AuctionFilter, AuctionOrdering, PageSize } from "@/types";
import { create } from "zustand";

type State = {
    pageNumber: number;
    pageSize: PageSize;   
    pageCount: number;
    orderBy: AuctionOrdering | null;
    filterBy: AuctionFilter | null;
    searchTerm: string;
};

type Actions = {
    setParams: (params: Partial<State>) => void;
    reset: () => void;
};

const initialState: State = {
    pageNumber: 1,
    pageSize: PageSize.Four,
    pageCount: 1,
    orderBy: null,
    filterBy: null,
    searchTerm: '',
}

export const useSearchParamsStore = create<State & Actions>()(set => ({
    ...initialState,
    setParams: (newParams) => {
        set(state => {
            if(newParams.pageNumber === undefined) {
                newParams.pageNumber = 1;
            }

            return {...state, ...newParams};
        });
    },
    reset: () => set(initialState),
}));