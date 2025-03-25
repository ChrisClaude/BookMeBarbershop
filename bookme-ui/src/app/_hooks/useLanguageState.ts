import { Language, setLanguage } from '@/_lib/features/language/language-slice';
import { useAppDispatch } from '@/_lib/hooks';
import { currentLanguageSelector } from '@/_lib/selectors/language.selectors';
import { useCallback } from 'react';
import { useSelector } from 'react-redux';

const useLanguageState = () => {
  const language = useSelector(currentLanguageSelector);
  const dispatch = useAppDispatch();

  const switchLanguage = useCallback(
    (language: Language) => {
      dispatch(setLanguage(language));
    },
    [dispatch]
  );

  return { language, switchLanguage };
};

export default useLanguageState;
