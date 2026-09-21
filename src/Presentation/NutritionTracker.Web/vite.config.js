import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

// https://vitejs.dev/config/
export default defineConfig(({ command }) => {
    const target = env.VITE_API_TARGET || env.VITE_API_URL || 'https://localhost:7155';

    const config = {
        plugins: [plugin()],
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url))
            }
        }
    };

    // The ASP.NET dev certificate and HTTPS dev server are only needed for local `vite dev`.
    // Skip them for `vite build` so CI/CD and Oryx-based build containers (which don't have
    // the `dotnet` CLI available) don't fail trying to generate a local dev certificate.
    if (command === 'serve') {
        const baseFolder =
            env.APPDATA !== undefined && env.APPDATA !== ''
                ? `${env.APPDATA}/ASP.NET/https`
                : `${env.HOME}/.aspnet/https`;

        const certificateName = "nutrition-tracker-web";
        const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
        const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

        if (!fs.existsSync(baseFolder)) {
            fs.mkdirSync(baseFolder, { recursive: true });
        }

        if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
            console.log('Creating development certificate...');
            const result = child_process.spawnSync('dotnet', [
                'dev-certs',
                'https',
                '--export-path',
                certFilePath,
                '--format',
                'Pem',
                '--no-password',
            ], { stdio: 'pipe' });

            if (result.status !== 0) {
                console.error('Failed to create certificate:', result.stderr?.toString());
                console.log('You may need to run: dotnet dev-certs https --trust');
                process.exit(1);
            }
            console.log('Certificate created successfully!');
        }

        config.server = {
            proxy: {
                '^/api': {
                    target,
                    secure: false,
                    changeOrigin: true
                }
            },
            port: 5173,
            https: {
                key: fs.readFileSync(keyFilePath),
                cert: fs.readFileSync(certFilePath),
            }
        };
    }

    return config;
});
