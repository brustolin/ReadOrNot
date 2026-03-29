export async function loadClientConfig() {
  const response = await fetch("/client-config", { credentials: "same-origin" });
  if (!response.ok) {
    throw new Error("Unable to load client configuration.");
  }

  return response.json();
}
