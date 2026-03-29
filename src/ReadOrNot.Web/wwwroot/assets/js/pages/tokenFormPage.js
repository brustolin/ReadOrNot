import { showMessage } from "../components/ui.js";
import { formatUtc } from "../utils/formatters.js";

export async function renderTokenFormPage(app, context, route) {
  const editing = Boolean(route.params.id);
  const token = editing ? await context.apis.tokens.get(route.params.id) : null;

  app.innerHTML = `
    <section class="layout-grid">
      <aside class="panel stack">
        <h2>${editing ? "Update token" : "Create token"}</h2>
        <p>${editing ? "Adjust naming, state, and expiration without changing the public identifier." : "Create a new unguessable identifier and reusable tracking URL."}</p>
        ${editing ? `<div class="code-box">${token.trackingImageUrl}</div><div class="muted">Created ${formatUtc(token.createdAtUtc)}</div>` : ""}
      </aside>
      <section class="panel stack">
        <div id="token-form-message"></div>
        <form id="token-form" class="stack">
          <div class="field">
            <label for="name">Name</label>
            <input id="name" name="name" type="text" required value="${token?.name ?? ""}">
          </div>
          <div class="field">
            <label for="description">Description</label>
            <textarea id="description" name="description">${token?.description ?? ""}</textarea>
          </div>
          <div class="field">
            <label for="expiresAtUtc">Expiration (UTC)</label>
            <input id="expiresAtUtc" name="expiresAtUtc" type="datetime-local" value="${token?.expiresAtUtc ? token.expiresAtUtc.slice(0, 16) : ""}">
          </div>
          ${editing ? `
            <div class="field">
              <label for="isEnabled">State</label>
              <select id="isEnabled" name="isEnabled">
                <option value="true" ${token.isEnabled ? "selected" : ""}>Enabled</option>
                <option value="false" ${token.isEnabled ? "" : "selected"}>Disabled</option>
              </select>
            </div>` : ""}
          <button class="button" type="submit">${editing ? "Save changes" : "Create token"}</button>
        </form>
      </section>
    </section>
  `;

  app.querySelector("#token-form").addEventListener("submit", async (event) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);

    const payload = {
      name: form.get("name"),
      description: form.get("description") || null,
      expiresAtUtc: form.get("expiresAtUtc") ? new Date(form.get("expiresAtUtc")).toISOString() : null,
      isEnabled: form.get("isEnabled") !== "false"
    };

    try {
      const response = editing
        ? await context.apis.tokens.update(route.params.id, payload)
        : await context.apis.tokens.create(payload);

      window.location.hash = `#/tokens/${response.id}`;
    } catch (error) {
      showMessage(app.querySelector("#token-form-message"), error.message, "error");
    }
  });
}
