import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export type Language = 'en' | 'pl';

export type LanguageState = {
  language: Language;
};

const initialState: LanguageState = {
  language: 'en',
};

const languageSlice = createSlice({
  name: 'languageState',
  initialState,
  reducers: {
    setLanguage: (state, action: PayloadAction<Language>) => {
      state.language = action.payload;
    },
  },
});

export const { setLanguage } = languageSlice.actions;

export default languageSlice.reducer;
