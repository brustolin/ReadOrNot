const AUTH_STORAGE_KEY = "readornot.auth";

const state = {
  auth: loadAuthState(),
  route: ""
};

function loadAuthState() {
  const rawValue = localStorage.getItem(AUTH_STORAGE_KEY);
  if (!rawValue) {
    return { accessToken: null, account: null, expiresAtUtc: null };
  }

  try {
    return JSON.parse(rawValue);
  } catch {
    localStorage.removeItem(AUTH_STORAGE_KEY);
    return { accessToken: null, account: null, expiresAtUtc: null };
  }
}

export function getState() {
  return state;
}

export function setRoute(route) {
  state.route = route;
}

export function setAuth(auth) {
  state.auth = auth;
  localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(auth));
}

export function clearAuth() {
  state.auth = { accessToken: null, account: null, expiresAtUtc: null };
  localStorage.removeItem(AUTH_STORAGE_KEY);
}

export function isAuthenticated() {
  return Boolean(state.auth?.accessToken);
}
