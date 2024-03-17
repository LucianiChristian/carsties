import * as React from "react"

import { PageSize } from "@/types";
import { useSearchParamsStore } from "@/hooks/useSearchParamsStore";
import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group";

export function PageSizeToggle() {
    const pageSize = useSearchParamsStore(state => state.pageSize);
    const setParams = useSearchParamsStore(state => state.setParams);

    function handleToggleChange(value: string) {
        setParams({pageSize: value as PageSize});
    }

    return (
        <div className="flex items-center gap-2">
            <span id="pageSizeLabel">Page Size</span>
            <ToggleGroup type="single" aria-labelledby="pageSizeLabel" value={pageSize} onValueChange={handleToggleChange}>
                <ToggleGroupItem key={PageSize.Four}
                                 value={PageSize.Four}
                                 aria-label={`Toggle ${PageSize.Four}`}
                                 variant="outline"
                                 className="w-10">
                    {PageSize.Four}
                </ToggleGroupItem> 
                <ToggleGroupItem key={PageSize.Eight}
                                 value={PageSize.Eight}
                                 aria-label={`Toggle ${PageSize.Eight}`}
                                 variant="outline"
                                 className="w-10">
                    {PageSize.Eight}
                </ToggleGroupItem> 
                <ToggleGroupItem key={PageSize.Twelve}
                                 value={PageSize.Twelve}
                                 aria-label={`Toggle ${PageSize.Twelve}`}
                                 variant="outline"
                                 className="w-10">
                    {PageSize.Twelve}
                </ToggleGroupItem> 
            </ToggleGroup>
        </div>
    )
}