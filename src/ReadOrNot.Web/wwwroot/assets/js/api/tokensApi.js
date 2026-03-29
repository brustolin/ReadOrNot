export function createTokensApi(http) {
  return {
    list: () => http.get("/tokens"),
    create: (payload) => http.post("/tokens", payload),
    get: (tokenId, query = "") => http.get(`/tokens/${tokenId}${query}`),
    update: (tokenId, payload) => http.put(`/tokens/${tokenId}`, payload),
    enable: (tokenId) => http.post(`/tokens/${tokenId}/enable`, {}),
    disable: (tokenId) => http.post(`/tokens/${tokenId}/disable`, {}),
    remove: (tokenId) => http.delete(`/tokens/${tokenId}`),
    report: (tokenId, query = "") => http.get(`/reports/tokens/${tokenId}${query}`)
  };
}
