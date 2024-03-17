import React from 'react'
import Search from './Search'
import Logo from './Logo'
import LoginButton from './LoginButton'
import { getCurrentUser } from '../actions/authActions'
import UserActions from './UserActions'

export default async function Navbar() {
  const user = await getCurrentUser();

  return (
    <header className='sticky top-0 z-50 p-5 flex justify-between items-center bg-whitetext-gray-800 shadow-md bg-white'>
      <Logo />
      <Search />
      {user ? <UserActions user={user}/> : <LoginButton />}
    </header>
  )
}