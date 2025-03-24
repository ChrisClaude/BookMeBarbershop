import { createSelector } from '@reduxjs/toolkit';
import { RootState } from '../store';

export const languageSelector = (state: RootState) => state.languageReducer;

export const currentLanguageSelector = createSelector(
  languageSelector,
  languageState => languageState.language
);
