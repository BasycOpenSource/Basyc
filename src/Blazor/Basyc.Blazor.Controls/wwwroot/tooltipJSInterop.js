
export function hideTooltip(elementToMoveId, targetElementId) {
	let toMoveElement = document.getElementById(elementToMoveId);
	let targetElement = document.getElementById(targetElementId);
	targetElement.after(toMoveElement);
	toMoveElement.classList.remove("tooltip--visible")
	toMoveElement.classList.add("tooltip--hidden")
	//document.removeEventListener("keydown", keyDown);

}

export function showTooltip(elementToMoveId, targetElementQuerySelector) {
	let toMoveElement = document.getElementById(elementToMoveId);
	let targetElement = document.querySelector(targetElementQuerySelector);
	targetElement.after(toMoveElement);
	toMoveElement.classList.remove("tooltip--hidden")
	toMoveElement.classList.add("tooltip--visible")
	//document.addEventListener("keydown", keyDown);
}

//function keyDown(e) {

//}
