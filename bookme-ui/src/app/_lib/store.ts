import { configureStore } from '@reduxjs/toolkit';
import languageReducer from './features/language/language-slice';
import authReducer from './features/auth/auth-slice';

export const makeStore = () => {
  return configureStore({
    reducer: {
      languageReducer: languageReducer,
      authReducer: authReducer,
    },
  });
};

// Infer the type of makeStore
export type AppStore = ReturnType<typeof makeStore>;
// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<AppStore['getState']>;
export type AppDispatch = AppStore['dispatch'];
