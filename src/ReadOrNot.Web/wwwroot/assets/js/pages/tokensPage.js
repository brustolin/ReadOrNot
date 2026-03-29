import { copyToClipboard, showMessage } from "../components/ui.js";
import { formatUtc } from "../utils/formatters.js";

export async function renderTokensPage(app, context) {
  const tokens = await context.apis.tokens.list();

  app.innerHTML = `
    <section class="stack">
      <div class="hero">
        <span class="badge">Opens, not guaranteed reads</span>
        <h1>Tracking tokens</h1>
        <p>Create public image URLs for emails, then review each open or image load with filterable event history.</p>
        <div class="actions-inline">
          <a class="button" href="#/tokens/new">Create token</a>
        </div>
      </div>
      <div id="tokens-message"></div>
      <section class="table-shell">
        <div class="row" style="justify-content: space-between; align-items: center; margin-bottom: 16px;">
          <div>
            <h2 class="table-title">Your tokens</h2>
            <p class="table-meta">${tokens.length} token${tokens.length === 1 ? "" : "s"} ready for use.</p>
          </div>
        </div>
        ${tokens.length === 0 ? `<div class="empty-state">No tokens yet. Create one to generate a tracking image URL.</div>` : `
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Status</th>
                <th>Created</th>
                <th>Last open</th>
                <th>Total opens</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              ${tokens.map(token => `
                <tr>
                  <td>
                    <strong>${token.name}</strong>
                    <div class="muted">${token.description ?? "No description"}</div>
                  </td>
                  <td><span class="badge ${token.isEnabled ? "success" : "warn"}">${token.isEnabled ? "Enabled" : "Disabled"}</span></td>
                  <td>${formatUtc(token.createdAtUtc)}</td>
                  <td>${formatUtc(token.lastOpenUtc)}</td>
                  <td>${token.totalOpens}</td>
                  <td class="actions-inline">
                    <a class="ghost-button" href="#/tokens/${token.id}">Report</a>
                    <button class="ghost-button" data-copy="${token.trackingImageUrl}">Copy URL</button>
                  </td>
                </tr>
              `).join("")}
            </tbody>
          </table>
        `}
      </section>
    </section>
  `;

  app.querySelectorAll("[data-copy]").forEach((button) => {
    button.addEventListener("click", async () => {
      try {
        await copyToClipboard(button.getAttribute("data-copy"));
        showMessage(app.querySelector("#tokens-message"), "Tracking URL copied to your clipboard.", "notice");
      } catch {
        showMessage(app.querySelector("#tokens-message"), "Clipboard copy failed in this browser.", "error");
      }
    });
  });
}
