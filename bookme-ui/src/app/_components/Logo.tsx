import React from "react";
import Img from "next/image";

const Logo = () => {
  return (
    <div className="flex flex-col logo justify-center items-center bg-white">
      {/* <span className='text-3xl font-semibold uppercase tracking-[0.18rem]'>Robert</span>
      <span className='text-xs uppercase text-center tracking-[0.18rem]'>Barbershop</span> */}
      <Img
        src="/img/sans_tache_logo.jpeg"
        alt="Logo"
        width={100}
        height={40}
        className="inline-block"
      />
    </div>
  );
};

export default Logo;
