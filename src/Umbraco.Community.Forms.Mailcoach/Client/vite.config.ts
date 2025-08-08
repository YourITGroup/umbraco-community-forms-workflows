import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/bundle.manifests.ts", // Bundle registers one or more manifests
      formats: ["es"],
      fileName: "umbraco-community-forms-mailcoach",
    },
    outDir: "../wwwroot/App_Plugins/Umbraco.Forms.Community.Mailcoach", // your web component will be saved in this location
    emptyOutDir: true,
    sourcemap: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
    base: "/App_Plugins/Umbraco.Forms.Community.Mailcoach/", // the base path of the app in the browser (used for assets)
});
