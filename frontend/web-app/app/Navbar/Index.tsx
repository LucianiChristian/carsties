import React from 'react'
import Search from './Search'
import Logo from './Logo'

export default function Navbar() {
  return (
    <header className='sticky top-0 z-50 p-5 flex justify-between items-center bg-whitetext-gray-800 shadow-md bg-white'>
      <Logo />
      <Search />
      <div>Login</div>
    </header>
  )
}