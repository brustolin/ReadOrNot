import { setAuth } from "../state/store.js";
import { showMessage } from "../components/ui.js";

export function renderRegisterPage(app, context) {
  app.innerHTML = `
    <section class="auth-card stack">
      <div>
        <h1 class="auth-title">Create Account</h1>
        <p class="muted">Set up your ReadOrNot workspace and start generating tracking URLs.</p>
      </div>
      <div id="register-message"></div>
      <form id="register-form" class="stack">
        <div class="field">
          <label for="displayName">Display name</label>
          <input id="displayName" name="displayName" type="text" required>
        </div>
        <div class="field">
          <label for="email">Email</label>
          <input id="email" name="email" type="email" required>
        </div>
        <div class="field">
          <label for="password">Password</label>
          <input id="password" name="password" type="password" required>
        </div>
        <div class="field">
          <label for="confirmPassword">Confirm password</label>
          <input id="confirmPassword" name="confirmPassword" type="password" required>
        </div>
        <button class="button" type="submit">Create account</button>
      </form>
      <div class="auth-links">
        <a href="#/login">Already have an account?</a>
      </div>
    </section>
  `;

  app.querySelector("#register-form").addEventListener("submit", async (event) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);

    try {
      const response = await context.apis.auth.register({
        displayName: form.get("displayName"),
        email: form.get("email"),
        password: form.get("password"),
        confirmPassword: form.get("confirmPassword")
      });

      setAuth({
        accessToken: response.accessToken,
        expiresAtUtc: response.expiresAtUtc,
        account: response.account
      });

      window.location.hash = "#/tokens";
    } catch (error) {
      showMessage(app.querySelector("#register-message"), error.message, "error");
    }
  });
}
