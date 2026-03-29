import { copyToClipboard, showMessage } from "../components/ui.js";
import { formatBool, formatUtc } from "../utils/formatters.js";

export async function renderTokenDetailsPage(app, context, route) {
  const searchParams = new URLSearchParams();
  const currentQuery = route.query;

  if (currentQuery.fromUtc) {
    searchParams.set("fromUtc", currentQuery.fromUtc);
  }

  if (currentQuery.toUtc) {
    searchParams.set("toUtc", currentQuery.toUtc);
  }

  searchParams.set("includeLikelyBots", currentQuery.includeLikelyBots ?? "true");

  const token = await context.apis.tokens.get(route.params.id, `?${searchParams.toString()}`);
  const report = token.report;
  const maxCount = Math.max(...report.dailyCounts.map(point => point.count), 1);

  app.innerHTML = `
    <section class="stack">
      <div class="hero">
        <span class="badge ${token.isEnabled ? "success" : "warn"}">${token.isEnabled ? "Enabled" : "Disabled"}</span>
        <h1>${token.name}</h1>
        <p>${token.description ?? "No description yet."}</p>
        <div class="actions-inline">
          <button class="button" id="copy-url">Copy URL</button>
          <button class="button secondary" id="copy-tag">Copy img tag</button>
          <a class="ghost-button" href="#/tokens/${token.id}/edit">Edit</a>
          <button class="ghost-button" id="toggle-token">${token.isEnabled ? "Disable" : "Enable"}</button>
          <button class="ghost-button" id="delete-token">Delete</button>
        </div>
      </div>
      <div id="token-message"></div>

      <section class="panel stack">
        <h2>Embed this URL</h2>
        <div class="code-box">${token.trackingImageUrl}</div>
        <div class="code-box">${token.suggestedImgTag.replace(/</g, "&lt;").replace(/>/g, "&gt;")}</div>
      </section>

      <section class="stats-grid">
        <article><span class="stat-label">Total opens</span><div class="stat-value">${report.totalOpens}</div></article>
        <article><span class="stat-label">Unique opens approx.</span><div class="stat-value">${report.uniqueOpensApproximation}</div></article>
        <article><span class="stat-label">First open</span><div class="stat-value">${formatUtc(report.firstOpenUtc)}</div></article>
        <article><span class="stat-label">Last open</span><div class="stat-value">${formatUtc(report.lastOpenUtc)}</div></article>
      </section>

      <section class="panel stack">
        <h2>Filters</h2>
        <form id="report-filter-form" class="row">
          <div class="field">
            <label for="fromUtc">From (UTC)</label>
            <input id="fromUtc" name="fromUtc" type="datetime-local" value="${currentQuery.fromUtc ? currentQuery.fromUtc.slice(0, 16) : ""}">
          </div>
          <div class="field">
            <label for="toUtc">To (UTC)</label>
            <input id="toUtc" name="toUtc" type="datetime-local" value="${currentQuery.toUtc ? currentQuery.toUtc.slice(0, 16) : ""}">
          </div>
          <div class="field">
            <label for="includeLikelyBots">Include likely bots</label>
            <select id="includeLikelyBots" name="includeLikelyBots">
              <option value="true" ${(currentQuery.includeLikelyBots ?? "true") === "true" ? "selected" : ""}>Yes</option>
              <option value="false" ${(currentQuery.includeLikelyBots ?? "true") === "false" ? "selected" : ""}>No</option>
            </select>
          </div>
          <div class="field" style="justify-content: end;">
            <label>&nbsp;</label>
            <button class="button" type="submit">Apply filters</button>
          </div>
        </form>
      </section>

      <section class="chart-card stack">
        <h2>Daily activity</h2>
        ${report.dailyCounts.length === 0 ? `<div class="empty-state">No activity in the selected range.</div>` : `
          <div class="chart">
            ${report.dailyCounts.map(point => `
              <div class="chart-bar">
                <div class="chart-bar-fill" style="height: ${(point.count / maxCount) * 180}px;"></div>
                <div class="chart-bar-label">${point.day}</div>
              </div>
            `).join("")}
          </div>`}
      </section>

      <section class="table-shell">
        <div class="row" style="justify-content: space-between; align-items: center; margin-bottom: 16px;">
          <div>
            <h2 class="table-title">Open events</h2>
            <p class="table-meta">${report.events.length} event${report.events.length === 1 ? "" : "s"} in this report window.</p>
          </div>
        </div>
        ${report.events.length === 0 ? `<div class="empty-state">No opens matched these filters.</div>` : `
          <table>
            <thead>
              <tr>
                <th>Timestamp</th>
                <th>Bot</th>
                <th>Requester</th>
                <th>User agent</th>
                <th>Referer</th>
              </tr>
            </thead>
            <tbody>
              ${report.events.map(event => `
                <tr>
                  <td>${formatUtc(event.occurredAtUtc)}</td>
                  <td>${formatBool(event.isLikelyBot, "Likely bot", "Likely human")}<div class="muted">${event.botSignals ?? ""}</div></td>
                  <td>
                    <div>${event.ipAddress ?? event.ipAddressHash ?? "Not stored"}</div>
                    <div class="muted">${event.acceptLanguage ?? ""}</div>
                  </td>
                  <td>${event.userAgent ?? ""}</td>
                  <td>${event.referer ?? ""}</td>
                </tr>`).join("")}
            </tbody>
          </table>`}
      </section>
    </section>
  `;

  app.querySelector("#copy-url").addEventListener("click", () => handleCopy(app.querySelector("#token-message"), token.trackingImageUrl, "Tracking URL copied."));
  app.querySelector("#copy-tag").addEventListener("click", () => handleCopy(app.querySelector("#token-message"), token.suggestedImgTag, "Image tag copied."));

  app.querySelector("#report-filter-form").addEventListener("submit", (event) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    const query = new URLSearchParams();

    if (form.get("fromUtc")) {
      query.set("fromUtc", new Date(form.get("fromUtc")).toISOString());
    }

    if (form.get("toUtc")) {
      query.set("toUtc", new Date(form.get("toUtc")).toISOString());
    }

    query.set("includeLikelyBots", form.get("includeLikelyBots"));
    window.location.hash = `#/tokens/${route.params.id}?${query.toString()}`;
  });

  app.querySelector("#toggle-token").addEventListener("click", async () => {
    await (token.isEnabled ? context.apis.tokens.disable(token.id) : context.apis.tokens.enable(token.id));
    window.location.reload();
  });

  app.querySelector("#delete-token").addEventListener("click", async () => {
    if (!window.confirm("Delete this token and all associated open events?")) {
      return;
    }

    await context.apis.tokens.remove(token.id);
    window.location.hash = "#/tokens";
  });
}

async function handleCopy(target, text, message) {
  try {
    await copyToClipboard(text);
    showMessage(target, message, "notice");
  } catch {
    showMessage(target, "Clipboard copy failed in this browser.", "error");
  }
}
