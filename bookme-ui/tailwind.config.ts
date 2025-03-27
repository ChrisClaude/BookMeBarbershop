import type { Config } from 'tailwindcss';
import { heroui } from '@heroui/react';

/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/app/pages/**/*.{js,ts,jsx,tsx,mdx}',
    './src/app/components/**/*.{js,ts,jsx,tsx,mdx}',
    './src/app/**/*.{js,ts,jsx,tsx,mdx}',
    './node_modules/@heroui/theme/dist/**/*.{js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        background: 'var(--background)',
        foreground: 'var(--foreground)',
        accent: 'var(--accent)',
        primary: {
          DEFAULT: '#171717',
        },
      },
      keyframes: {
        bounce: {
          '0%, 100%': {
            transform: 'translateY(0)',
            opacity: '0.5',
          },
          '50%': {
            transform: 'translateY(-1rem)',
            opacity: '1',
          },
        },
      },
    },
  },
  darkMode: 'class',
  plugins: [heroui()],
} satisfies Config;
