import React from 'react'
import { PageSizeToggle } from './PageSizeToggle'
import { FiltersDropdown } from './FiltersDropdown'

export default function ListingsToolBar() {
  return (
    <div className="flex justify-between align-center">
      <FiltersDropdown />
      <PageSizeToggle />
    </div>
  )
}
