import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export type LanguageState = {
  language: 'en' | 'pl';
};

const initialState: LanguageState = {
  language: 'en',
};

const languageSlice = createSlice({
  name: 'languageState',
  initialState,
  reducers: {
    setLanguage: (state, action: PayloadAction<'en' | 'pl'>) => {
      state.language = action.payload;
    },
  },
});

export const { setLanguage } = languageSlice.actions;

export default languageSlice.reducer;
