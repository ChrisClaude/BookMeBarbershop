"use client";
import { withAuth } from '@/_components/auth/AuthGuard';
import { PagedListDtoOfUserDto } from '@/_lib/codegen';
import { ROLES } from '@/_lib/enums/constant';
import { useGetAllUsersQuery } from '@/_lib/queries';
import { QueryResult } from '@/_lib/queries/rtk.types';
import { Button, Input, Pagination, Spinner, Table, TableBody, TableCell, TableColumn, TableHeader, TableRow } from '@heroui/react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import React, { useCallback, useMemo, useState } from 'react';

const UserAdminPage = () => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const pathname = usePathname();
  const [searchTerm, setSearchTerm] = useState('');

  const pageIndex = useMemo(() => {
    const page = searchParams.get("page");
    return Number(page) || 1;
  }, [searchParams]);

  const request = useMemo(() => ({
    page: pageIndex - 1, // API uses 0-based indexing
    pageSize: 10
  }), [pageIndex]);

  const {
    data: users,
    isFetching,
    error
  } = useGetAllUsersQuery<QueryResult<PagedListDtoOfUserDto>>(request);

  const handlePageChange = useCallback(
    (page: number) => {
      router.push(`${pathname}?page=${page}`);
    },
    [router, pathname]
  );

  const filteredUsers = useMemo(() => {
    if (!users?.items) return [];

    return users.items.filter(user =>
      user.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.surname?.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [users, searchTerm]);

  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">User Management</h1>
        <Input
          placeholder="Search users..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="max-w-xs"
          startContent={
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5 text-gray-400">
              <path strokeLinecap="round" strokeLinejoin="round" d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z" />
            </svg>
          }
        />
      </div>

      {isFetching ? (
        <div className="flex justify-center items-center h-64">
          <Spinner size="lg" />
        </div>
      ) : error ? (
        <div className="p-4 border rounded-lg bg-red-50 text-center">
          <p className="text-red-500">Error loading users</p>
          <Button color="primary" className="mt-4" onPress={() => router.refresh()}>
            Try Again
          </Button>
        </div>
      ) : (
        <>
          <Table aria-label="Users table" className="min-w-full">
            <TableHeader>
              <TableColumn>NAME</TableColumn>
              <TableColumn>EMAIL</TableColumn>
              <TableColumn>ROLES</TableColumn>
              <TableColumn>ACTIONS</TableColumn>
            </TableHeader>
            <TableBody emptyContent="No users found">
              {filteredUsers.map((user) => (
                <TableRow key={user.id}>
                  <TableCell>
                    <div className="font-medium">
                      {user.name} {user.surname}
                    </div>
                  </TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>
                    <div className="flex gap-1 flex-wrap">
                      {user.roles?.map((role, index) => (
                        <span key={index} className="px-2 py-1 text-xs rounded-full bg-primary-100 text-primary-700">
                          {role.role?.name}
                        </span>
                      ))}
                    </div>
                  </TableCell>
                  <TableCell>
                    <Button size="sm" color="primary" variant="light">
                      View Details
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>

          {users?.totalPages && users.totalPages > 1 && (
            <div className="flex justify-center mt-6">
              <Pagination
                total={users.totalPages}
                initialPage={pageIndex}
                page={pageIndex}
                onChange={handlePageChange}
              />
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default withAuth(UserAdminPage, ROLES.ADMIN, {
  fallbackPath: "/unauthorized",
});
