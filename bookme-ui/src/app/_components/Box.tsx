import React from 'react';

const Box = ({
  children,
  className,
}: {
  children: React.ReactNode;
  className: string;
}) => {
  return <div className={className}>{children}</div>;
};

export default Box;
