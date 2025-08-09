import { defineConfig } from '@hey-api/openapi-ts';
import { defaultPlugins } from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'https://localhost:44368/umbraco/swagger/forms-community-management-workflows/swagger.json',
  output: {
    format: 'prettier',
    lint: 'eslint',
    path: 'src/generated'
  },
  plugins: [
    ...defaultPlugins,
		{
			name: '@hey-api/client-fetch',
			exportFromIndex: true,
			throwOnError: true,
		},
		{
			name: '@hey-api/typescript',
			enums: 'typescript',
			readOnlyWriteOnlyBehavior: 'off',
		},
		{
			name: '@hey-api/sdk',
			asClass: true,
		},
  ],
});
