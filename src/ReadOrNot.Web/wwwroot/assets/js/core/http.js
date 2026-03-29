import { clearAuth, getState } from "../state/store.js";

export function createHttpClient(apiBaseUrl) {
  async function request(path, options = {}) {
    const auth = getState().auth;
    const headers = new Headers(options.headers ?? {});

    if (options.body && !headers.has("Content-Type")) {
      headers.set("Content-Type", "application/json");
    }

    if (auth?.accessToken) {
      headers.set("Authorization", `Bearer ${auth.accessToken}`);
    }

    const response = await fetch(`${apiBaseUrl}${path}`, {
      ...options,
      headers,
      body: options.body ? JSON.stringify(options.body) : undefined
    });

    if (response.status === 401) {
      clearAuth();
      window.location.hash = "#/login";
      throw new Error("Your session has expired. Please sign in again.");
    }

    if (response.status === 204) {
      return null;
    }

    const contentType = response.headers.get("content-type") ?? "";
    const payload = contentType.includes("application/json") ? await response.json() : await response.text();

    if (!response.ok) {
      throw normalizeError(payload);
    }

    return payload;
  }

  return {
    get: (path) => request(path),
    post: (path, body) => request(path, { method: "POST", body }),
    put: (path, body) => request(path, { method: "PUT", body }),
    delete: (path) => request(path, { method: "DELETE" })
  };
}

function normalizeError(payload) {
  if (payload?.errors) {
    const firstEntry = Object.entries(payload.errors)[0];
    const message = firstEntry?.[1]?.[0] ?? payload.title ?? "The request could not be completed.";
    return new Error(message);
  }

  if (payload?.title) {
    return new Error(payload.title);
  }

  return new Error(typeof payload === "string" ? payload : "The request could not be completed.");
}
