import { readdirSync, readFileSync, statSync } from 'node:fs';
import { resolve, relative } from 'node:path';
import assert from 'node:assert/strict';

const files=(directory)=>readdirSync(directory).flatMap((entry)=>{
  const file=resolve(directory,entry);
  return statSync(file).isDirectory()?files(file):[file];
});
const routes=files('src/pages').filter(file=>file.endsWith('.tsx'));
const source=files('src').filter(file=>/\.(tsx?|mjs)$/.test(file));
assert.ok(routes.length>=18, 'Expected independently routed feature screens');
for(const file of routes){
  const code=readFileSync(file,'utf8');
  assert.ok(code.includes('createFileRoute')||code.includes('createRootRoute'),`Not a route declaration: ${file}`);
  assert.ok(!/use(Get|Create|Update|Delete|Increase|Decrease|Login|Register).*Query|Mutation\s*\(/.test(code),`API logic found inside routing layer: ${file}`);
}
for(const file of source){
  const code=readFileSync(file,'utf8');
  const imports=[...code.matchAll(/(?:from\s*|import\s*)['"]([^'"]+)['"]/g)].map(match=>match[1]);
  for(const module of imports){
    if(!module.startsWith('@/')&&!module.startsWith('./')&&!module.startsWith('../'))continue;
    if(module==='./routeTree.gen'&&file.endsWith('/src/main.tsx'))continue; // router generates this file
    const base=module.startsWith('@/')?resolve('src',module.slice(2)):resolve(file,'..',module);
    assert.ok(['','.tsx','.ts','.js','.mjs','.css','/index.ts','/index.tsx'].some(ext=>{
      try{return statSync(`${base}${ext}`).isFile();}catch{return false;}
    }),`Unresolved import ${module} in ${relative(process.cwd(),file)}`);
  }
}
console.info(`PASS: ${routes.length} thin route files and ${source.length} source files resolved`);
