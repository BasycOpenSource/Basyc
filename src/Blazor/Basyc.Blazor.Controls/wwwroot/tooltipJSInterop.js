
export function moveToRoot(query) {
	let toolTip = document.querySelector(query);
	let body = document.querySelector('body');
	body.after(toolTip);
}
