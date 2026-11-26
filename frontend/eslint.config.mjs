import js from '@eslint/js';
import tseslint from 'typescript-eslint';
import reactHooks from 'eslint-plugin-react-hooks';
import react from 'eslint-plugin-react';
import jsxA11y from 'eslint-plugin-jsx-a11y';
import globals from 'globals';

/**
 * Airbnb-inspired formatting, naming and predictable control flow.
 * Airbnb's legacy preset is not silently claimed to be ESLint 9 flat-compatible.
 * TypeScript-specific rules use typescript-eslint; Prettier owns whitespace.
 */
export default tseslint.config(
  {
    ignores: [
      'dist/**', 'coverage/**', 'src/routeTree.gen.ts',
      'src/shared/api/generated/**', 'node_modules/**', 'openapi/**',
    ],
  },
  js.configs.recommended,
  ...tseslint.configs.recommended,
  { files: ['scripts/**/*.mjs', '*.mjs'], languageOptions: { globals: globals.node } },
  {
    files: ['src/**/*.{ts,tsx}'],
    languageOptions: { globals: globals.browser },
    settings: { react: { version: 'detect' } },
    plugins: { 'react-hooks': reactHooks, react, 'jsx-a11y': jsxA11y },
    rules: {
      ...reactHooks.configs.recommended.rules,
      'curly': ['error', 'multi-line'],
      'eqeqeq': ['error', 'always'],
      'no-var': 'error',
      'prefer-const': 'error',
      'object-shorthand': 'error',
      'no-console': ['warn', { allow: ['warn', 'error'] }],
      'react/jsx-key': 'error',
      'jsx-a11y/alt-text': 'error',
      'jsx-a11y/anchor-has-content': 'error',
      '@typescript-eslint/consistent-type-imports': 'error',
      '@typescript-eslint/no-explicit-any': 'warn',
      '@typescript-eslint/no-unused-vars': ['error', { argsIgnorePattern: '^_', varsIgnorePattern: '^_' }],
    },
  },
  { files: ['src/shared/api/demoBaseQuery.ts'], rules: { '@typescript-eslint/no-explicit-any': 'off' } },
);
