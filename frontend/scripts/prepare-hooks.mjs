import { existsSync } from 'node:fs';
import { execFileSync } from 'node:child_process';

// A downloaded ZIP contains no .git directory. Avoid failing npm install.
if (!existsSync('.git')) {
  console.info('Git is not initialized; run git init && npm run prepare to activate Husky.');
} else {
  execFileSync(process.execPath, ['node_modules/husky/bin.mjs'], { stdio: 'inherit' });
}
