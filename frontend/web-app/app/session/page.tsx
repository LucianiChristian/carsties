import React from 'react'
import { getCurrentUser, getTokenWorkaround } from '../actions/authActions'
import Heading from '../components/Heading';
import AuthTest from './AuthTest';

export default async function Session() {
    const currentUser = await getCurrentUser();
    const token = await getTokenWorkaround();

    return (
        <div>
            <Heading title='Session dashboard'/>
            <h2 className="text-lg font-medium">Session Data</h2>
            <pre>{JSON.stringify(currentUser, null, 2)}</pre>
            <h2 className="text-lg font-medium">JWT Data</h2>
            <pre className='overflow-auto'>{JSON.stringify(token , null, 2)}</pre>
            <AuthTest />
        </div>
    )
}
