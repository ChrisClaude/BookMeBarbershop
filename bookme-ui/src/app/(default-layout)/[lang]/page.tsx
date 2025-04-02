"use client";
import HomePageContent from "@/_components/HomePageContent";
import useLanguageState from "@/_hooks/useLanguageState";
import { useRouter } from "next/navigation";
import { use, useEffect } from "react";

const HomeLang = ({ params }: { params: Promise<{ lang: string }> }) => {
  const router = useRouter();
  const { lang } = use(params);
  const { language, switchLanguage } = useLanguageState();

  useEffect(() => {
    if (lang === "en" || lang === "pl") {
      switchLanguage(lang);
      return;
    }
    router.push("/");
  }, [lang, router, switchLanguage]);

  return <HomePageContent language={language} />;
};

export default HomeLang;
