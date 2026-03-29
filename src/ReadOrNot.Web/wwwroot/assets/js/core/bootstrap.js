import { createAccountApi } from "../api/accountApi.js";
import { createAuthApi } from "../api/authApi.js";
import { createTokensApi } from "../api/tokensApi.js";
import { loadClientConfig } from "./config.js";
import { createHttpClient } from "./http.js";
import { handleRoute } from "./router.js";

async function start() {
  const app = document.querySelector("#app");

  try {
    const config = await loadClientConfig();
    const http = createHttpClient(config.apiBaseUrl);
    const context = {
      config,
      apis: {
        auth: createAuthApi(http),
        account: createAccountApi(http),
        tokens: createTokensApi(http)
      }
    };

    window.addEventListener("hashchange", () => {
      handleRoute(app, context).catch(renderFatalError);
    });

    await handleRoute(app, context);
  } catch (error) {
    renderFatalError(error);
  }
}

function renderFatalError(error) {
  const app = document.querySelector("#app");
  app.innerHTML = `
    <section class="auth-card stack">
      <h1 class="auth-title">Unable to start ReadOrNot</h1>
      <div class="error">${error.message}</div>
    </section>
  `;
}

start();
