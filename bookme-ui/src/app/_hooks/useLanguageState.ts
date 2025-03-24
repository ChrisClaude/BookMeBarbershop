import { Language, setLanguage } from '@/_lib/features/language/language-slice';
import { currentLanguageSelector } from '@/_lib/selectors/language.selectors';
import { useCallback } from 'react';
import { useSelector } from 'react-redux';

const useLanguageState = () => {
  const language = useSelector(currentLanguageSelector);

  const switchLanguage = useCallback((language: Language) => {
    setLanguage(language);
  }, []);

  return { language, switchLanguage };
};

export default useLanguageState;
