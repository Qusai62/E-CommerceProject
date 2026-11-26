import path from 'node:path';
import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';
import { tanstackRouter } from '@tanstack/router-plugin/vite';

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '');
  return {
  plugins: [tanstackRouter({ target: 'react', routesDirectory: './src/pages', generatedRouteTree: './src/routeTree.gen.ts', autoCodeSplitting: true }), react(), tailwindcss()],
  resolve: { alias: { '@': path.resolve(__dirname, 'src') } },
  server: {
    port: 5173,
    proxy: { '/api': { target: env.API_PROXY_TARGET ?? 'https://localhost:7162', changeOrigin: true, secure: false } },
  },
  };
});
