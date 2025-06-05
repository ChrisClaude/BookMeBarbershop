import { Link, Tooltip } from "@heroui/react";
import React from "react";
import { CgProfile } from "react-icons/cg";
import { FaInstagram } from "react-icons/fa6";
import { useAuth } from "@/_hooks/useAuth";
import Image from "next/image";
import useLanguageState from "@/_hooks/useLanguageState";

const NavLinksExtension = ({
  handleOpenSidebarNav,
}: {
  handleOpenSidebarNav: () => void;
}) => {
  const { status } = useAuth();
  const { language } = useLanguageState();

  return (
    <div className="flex items-center gap-x-6 xl:gap-x-36">
      <div className="flex gap-x-5 items-center">
        <a
          href="https://www.instagram.com/sans_tache_papi?utm_source=ig_web_button_share_sheet&igsh=ZDNlZDc0MzIxNw=="
          target="_blank"
        >
          <FaInstagram size={26} />
        </a>
      </div>
      <div>
        <Link href={`/${language === "en" ? "pl" : "en"}`}>
          <Image
            src={
              language === "pl"
                ? "/img/flag_poland.svg"
                : "/img/flag_united_kingdom.svg"
            }
            alt="English Flag"
            width={35}
            height={30}
            className="inline-block"
          />
        </Link>
      </div>
      {status === "authenticated" && (
        <div>
          <Tooltip content="Click to open sidebar">
            <button onClick={() => handleOpenSidebarNav()}>
              <CgProfile size={30} className="text-primary" />
            </button>
          </Tooltip>
        </div>
      )}
    </div>
  );
};
export default NavLinksExtension;
