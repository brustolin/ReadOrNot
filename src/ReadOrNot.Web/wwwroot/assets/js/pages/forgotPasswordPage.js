import { showMessage } from "../components/ui.js";

export function renderForgotPasswordPage(app, context) {
  app.innerHTML = `
    <section class="auth-card stack">
      <div>
        <h1 class="auth-title">Forgot Password</h1>
        <p class="muted">Request a reset token. In development, the backend returns a preview token and reset URL.</p>
      </div>
      <div id="forgot-message"></div>
      <form id="forgot-form" class="stack">
        <div class="field">
          <label for="email">Email</label>
          <input id="email" name="email" type="email" required>
        </div>
        <button class="button" type="submit">Request reset</button>
      </form>
    </section>
  `;

  app.querySelector("#forgot-form").addEventListener("submit", async (event) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);

    try {
      const response = await context.apis.auth.forgotPassword({ email: form.get("email") });
      const extra = response.developmentResetUrl
        ? `<p><a href="${response.developmentResetUrl}">Open reset page</a></p><div class="code-box">${response.developmentResetToken}</div>`
        : "<p>If the account exists, a reset step has been prepared.</p>";

      showMessage(app.querySelector("#forgot-message"), extra, "notice");
    } catch (error) {
      showMessage(app.querySelector("#forgot-message"), error.message, "error");
    }
  });
}
