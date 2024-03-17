'use client'

import { Button } from '@/components/ui/button'
import React from 'react'
import { FaSignOutAlt } from 'react-icons/fa'
import { FaCar, FaSpaghettiMonsterFlying, FaTrophy, FaUser } from 'react-icons/fa6'
import { User } from 'next-auth'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu'
import Link from 'next/link'
import { signOut } from 'next-auth/react'

type Props = {
  user: Partial<User>;
};

export default async function UserActions({ user }: Props) {
  return (
      <DropdownMenu>

        <DropdownMenuTrigger asChild>
            <Button variant="outline">Welcome {user?.name}</Button>
        </DropdownMenuTrigger>

        <DropdownMenuContent>
            <MenuItem href="/">
                <FaUser className="mt-[2px]" />
                My Auctions
            </MenuItem>
            <MenuItem href="">
                <FaTrophy className="mt-[2px]" />
                Auctions Won
            </MenuItem>
            <MenuItem href="">
                <FaCar className="mt-[2px]" />
                Sell My Car
            </MenuItem>
            <MenuItem href="/session">
                <FaSpaghettiMonsterFlying className="mt-[2px]" />
                Session (dev only)
            </MenuItem>
            <MenuItem href="" onClick={signOut}>
                <FaSignOutAlt className="mt-[2px]" />
                Sign Out
            </MenuItem>
        </DropdownMenuContent>

  </DropdownMenu>
  )
}

function MenuItem({
  href, 
  onClick,
  children
}: {
  href: string, 
  onClick?: () => void,
  children: React.ReactNode
}) {
  return (
      <DropdownMenuItem>
          <Link href={href} onClick={onClick}>
              <div className="flex align-items-center gap-[0.75em]">
                {children}
              </div>
          </Link>
      </DropdownMenuItem>
  )
}