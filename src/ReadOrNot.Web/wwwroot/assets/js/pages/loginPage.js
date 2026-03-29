import { setAuth } from "../state/store.js";
import { showMessage } from "../components/ui.js";

export function renderLoginPage(app, context) {
  app.innerHTML = `
    <section class="auth-card stack">
      <div>
        <h1 class="auth-title">Log In</h1>
        <p class="muted">Sign in to manage tracking tokens and review image-load activity.</p>
      </div>
      <div id="auth-message"></div>
      <form id="login-form" class="stack">
        <div class="field">
          <label for="email">Email</label>
          <input id="email" name="email" type="email" required>
        </div>
        <div class="field">
          <label for="password">Password</label>
          <input id="password" name="password" type="password" required>
        </div>
        <button class="button" type="submit">Log in</button>
      </form>
      <div class="auth-links">
        <a href="#/register">Create account</a>
        <a href="#/forgot-password">Forgot password?</a>
      </div>
    </section>
  `;

  app.querySelector("#login-form").addEventListener("submit", async (event) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);

    try {
      const response = await context.apis.auth.login({
        email: form.get("email"),
        password: form.get("password")
      });

      setAuth({
        accessToken: response.accessToken,
        expiresAtUtc: response.expiresAtUtc,
        account: response.account
      });

      window.location.hash = "#/tokens";
    } catch (error) {
      showMessage(app.querySelector("#auth-message"), error.message, "error");
    }
  });
}
