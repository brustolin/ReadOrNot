import { clearAuth, getState, setAuth } from "../state/store.js";
import { showMessage } from "../components/ui.js";

export async function renderAccountPage(app, context) {
  const profile = await context.apis.account.getProfile();
  const state = getState();

  app.innerHTML = `
    <section class="layout-grid">
      <aside class="panel stack">
        <h2>Account</h2>
        <p>Manage your profile and password. Tokens and reports are always scoped to your own account.</p>
        <div class="code-box">${profile.email}</div>
      </aside>
      <section class="stack">
        <section class="panel stack">
          <h2>Profile</h2>
          <div id="profile-message"></div>
          <form id="profile-form" class="stack">
            <div class="field">
              <label for="displayName">Display name</label>
              <input id="displayName" name="displayName" type="text" value="${profile.displayName}" required>
            </div>
            <div class="field">
              <label for="timeZone">Time zone</label>
              <input id="timeZone" name="timeZone" type="text" value="${profile.timeZone ?? ""}">
            </div>
            <button class="button" type="submit">Save profile</button>
          </form>
        </section>
        <section class="panel stack">
          <h2>Change password</h2>
          <div id="password-message"></div>
          <form id="password-form" class="stack">
            <div class="field">
              <label for="currentPassword">Current password</label>
              <input id="currentPassword" name="currentPassword" type="password" required>
            </div>
            <div class="field">
              <label for="newPassword">New password</label>
              <input id="newPassword" name="newPassword" type="password" required>
            </div>
            <div class="field">
              <label for="confirmPassword">Confirm new password</label>
              <input id="confirmPassword" name="confirmPassword" type="password" required>
            </div>
            <button class="button" type="submit">Update password</button>
          </form>
        </section>
        <section class="panel stack">
          <h2>Session</h2>
          <button id="logout-button" class="ghost-button">Log out</button>
        </section>
      </section>
    </section>
  `;

  app.querySelector("#profile-form").addEventListener("submit", async (event) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);

    try {
      const updatedProfile = await context.apis.account.updateProfile({
        displayName: form.get("displayName"),
        timeZone: form.get("timeZone") || null
      });

      setAuth({
        ...state.auth,
        account: updatedProfile
      });

      showMessage(app.querySelector("#profile-message"), "Profile updated.", "notice");
    } catch (error) {
      showMessage(app.querySelector("#profile-message"), error.message, "error");
    }
  });

  app.querySelector("#password-form").addEventListener("submit", async (event) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);

    try {
      await context.apis.account.changePassword({
        currentPassword: form.get("currentPassword"),
        newPassword: form.get("newPassword"),
        confirmPassword: form.get("confirmPassword")
      });

      showMessage(app.querySelector("#password-message"), "Password updated.", "notice");
    } catch (error) {
      showMessage(app.querySelector("#password-message"), error.message, "error");
    }
  });

  app.querySelector("#logout-button").addEventListener("click", async () => {
    try {
      await context.apis.auth.logout();
    } finally {
      clearAuth();
      window.location.hash = "#/login";
    }
  });
}
