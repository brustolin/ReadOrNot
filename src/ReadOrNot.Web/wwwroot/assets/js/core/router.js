import { clearAuth, isAuthenticated, setRoute } from "../state/store.js";
import { renderNavigation } from "../components/layout.js";
import { renderAccountPage } from "../pages/accountPage.js";
import { renderForgotPasswordPage } from "../pages/forgotPasswordPage.js";
import { renderLoginPage } from "../pages/loginPage.js";
import { renderRegisterPage } from "../pages/registerPage.js";
import { renderResetPasswordPage } from "../pages/resetPasswordPage.js";
import { renderTokenDetailsPage } from "../pages/tokenDetailsPage.js";
import { renderTokenFormPage } from "../pages/tokenFormPage.js";
import { renderTokensPage } from "../pages/tokensPage.js";

const routes = [
  { pattern: /^\/login$/, auth: false, render: renderLoginPage },
  { pattern: /^\/register$/, auth: false, render: renderRegisterPage },
  { pattern: /^\/forgot-password$/, auth: false, render: renderForgotPasswordPage },
  { pattern: /^\/reset-password$/, auth: false, render: renderResetPasswordPage },
  { pattern: /^\/logout$/, auth: true, render: (_, __) => { clearAuth(); window.location.hash = "#/login"; } },
  { pattern: /^\/tokens$/, auth: true, render: renderTokensPage },
  { pattern: /^\/tokens\/new$/, auth: true, render: renderTokenFormPage },
  { pattern: /^\/tokens\/(?<id>\d+)$/, auth: true, render: renderTokenDetailsPage },
  { pattern: /^\/tokens\/(?<id>\d+)\/edit$/, auth: true, render: renderTokenFormPage },
  { pattern: /^\/account$/, auth: true, render: renderAccountPage }
];

export async function handleRoute(app, context) {
  const parsedRoute = parseRoute(window.location.hash);
  setRoute(parsedRoute.path);
  renderNavigation();

  const route = routes.find(candidate => candidate.pattern.test(parsedRoute.path));
  if (!route) {
    window.location.hash = isAuthenticated() ? "#/tokens" : "#/login";
    return;
  }

  if (route.auth && !isAuthenticated()) {
    window.location.hash = "#/login";
    return;
  }

  if (!route.auth && isAuthenticated() && ["/login", "/register"].includes(parsedRoute.path)) {
    window.location.hash = "#/tokens";
    return;
  }

  const match = route.pattern.exec(parsedRoute.path);
  const routeContext = {
    ...parsedRoute,
    params: match?.groups ?? {}
  };

  await route.render(app, context, routeContext);
  renderNavigation();
}

function parseRoute(hash) {
  const rawValue = hash.startsWith("#") ? hash.slice(1) : hash;
  const [pathPart, queryPart] = (rawValue || "/tokens").split("?");
  const query = Object.fromEntries(new URLSearchParams(queryPart ?? ""));

  return {
    path: pathPart || "/tokens",
    query
  };
}
