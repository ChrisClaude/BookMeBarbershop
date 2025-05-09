import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'
import tsconfigPaths from 'vite-tsconfig-paths'

export default defineConfig({
  plugins: [tsconfigPaths(), react()],
  test: {
    environment: 'jsdom',
    include: [
      'src/app/**/__tests__/*.test.{js,jsx,ts,tsx}',
    ],
    coverage: {
      include: [
        'src/app/**/*.{js,jsx,ts,tsx}',
      ],
      exclude: [
        'src/app/**/__tests__/*.{js,jsx,ts,tsx}',
        'src/app/**/index.{js,jsx,ts,tsx}',
        'src/app/**/styles.{js,jsx,ts,tsx}',
        'src/app/**/types.{js,jsx,ts,tsx}',
        'src/app/lib/codegen/**/*',
      ],
      reporter: ['cobertura', 'text', 'json', 'html'],
    },
  },
})