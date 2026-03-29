import { getState, isAuthenticated } from "../state/store.js";

export function renderNavigation() {
  const nav = document.querySelector("#nav");
  const route = getState().route;

  if (!nav) {
    return;
  }

  const auth = isAuthenticated();
  const links = auth
    ? [
        ["#/tokens", "Tokens"],
        ["#/tokens/new", "Create"],
        ["#/account", "Account"],
        ["#/logout", "Logout"]
      ]
    : [
        ["#/login", "Login"],
        ["#/register", "Register"]
      ];

  nav.innerHTML = links
    .map(([href, label]) => `<a class="nav-link ${route.startsWith(href.replace("#", "")) ? "active" : ""}" href="${href}">${label}</a>`)
    .join("");
}
