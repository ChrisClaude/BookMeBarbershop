'use client';
import React from 'react';
import { logInfo } from '@/_lib/utils/logging.utils';
import { useEffect } from 'react';
import { useRouter } from 'next/navigation';

const Page = () => {
  const router = useRouter();

  useEffect(() => {
    logInfo('You are being redirected to /', 'RedirectPage');
    router.push('/');
  }, [router]);

  return <div>RedirectPage</div>;
};

export default Page;
