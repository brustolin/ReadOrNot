export function createAuthApi(http) {
  return {
    register: (payload) => http.post("/auth/register", payload),
    login: (payload) => http.post("/auth/login", payload),
    forgotPassword: (payload) => http.post("/auth/forgot-password", payload),
    resetPassword: (payload) => http.post("/auth/reset-password", payload),
    logout: () => http.post("/auth/logout", {})
  };
}
