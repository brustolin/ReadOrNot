export function createAccountApi(http) {
  return {
    getProfile: () => http.get("/account"),
    updateProfile: (payload) => http.put("/account", payload),
    changePassword: (payload) => http.post("/account/change-password", payload)
  };
}
