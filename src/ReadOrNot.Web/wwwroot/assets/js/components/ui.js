export function showMessage(target, message, type = "notice") {
  target.innerHTML = `<div class="${type}">${message}</div>`;
}

export async function copyToClipboard(text) {
  await navigator.clipboard.writeText(text);
}
