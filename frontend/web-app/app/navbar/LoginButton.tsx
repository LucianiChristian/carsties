'use client'

import { Button } from '@/components/ui/button'
import { signIn } from 'next-auth/react'
import React from 'react'

export default function LoginButton() {
  return (
    <Button variant={"outline"} onClick={() => signIn("id-server", { callbackUrl: '/' })}>
        Login
    </Button>
  )
}