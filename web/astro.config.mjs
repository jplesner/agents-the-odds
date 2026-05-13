// @ts-check
import { defineConfig } from 'astro/config';
import tailwindcss from '@tailwindcss/vite';
import { copyFileSync, createReadStream, existsSync, mkdirSync, readdirSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import { join, resolve } from 'node:path';

function agentAvatars() {
  const dataAgentsDir = () => resolve(process.cwd(), '..', 'data', 'agents');

  return {
    name: 'agent-avatars',
    hooks: {
      'astro:server:setup': ({ server }) => {
        server.middlewares.use('/agents', (req, res, next) => {
          const parts = req.url.replace(/^\//, '').split('/');
          const filePath = resolve(dataAgentsDir(), ...parts);
          if (existsSync(filePath) && filePath.endsWith('.png')) {
            res.setHeader('Content-Type', 'image/png');
            createReadStream(filePath).pipe(res);
          } else {
            next();
          }
        });
      },
      'astro:build:done': ({ dir }) => {
        const agentsDir = dataAgentsDir();
        if (!existsSync(agentsDir)) return;
        const outBase = fileURLToPath(dir);
        for (const agentId of readdirSync(agentsDir)) {
          const src = resolve(agentsDir, agentId, 'avatar.png');
          if (!existsSync(src)) continue;
          const outDir = join(outBase, 'agents', agentId);
          mkdirSync(outDir, { recursive: true });
          copyFileSync(src, join(outDir, 'avatar.png'));
        }
      },
    },
  };
}

// https://astro.build/config
export default defineConfig({
  integrations: [agentAvatars()],
  vite: {
    plugins: [tailwindcss()],
  },
});