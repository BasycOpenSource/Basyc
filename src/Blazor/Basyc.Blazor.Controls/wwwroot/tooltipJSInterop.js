
//export function hideTooltip(elementToMoveId, targetElementId) {
//	let toMoveElement = document.getElementById(elementToMoveId);
//	let targetElement = document.getElementById(targetElementId);
//	targetElement.after(toMoveElement);
//	toMoveElement.classList.remove("tooltip--visible")
//	toMoveElement.classList.add("tooltip--hidden")

//}

//export function showTooltip(elementToMoveId, targetElementQuerySelector) {
//	let toMoveElement = document.getElementById(elementToMoveId);
//	let targetElement = document.querySelector(targetElementQuerySelector);
//	targetElement.after(toMoveElement);
//	toMoveElement.classList.remove("tooltip--hidden")
//	toMoveElement.classList.add("tooltip--visible")
//	document.addEventListener("keydown", keyDown);
//}

window.hideTooltip = (dotNetHelper, elementToMoveId, targetElementId) => {
	let toMoveElement = document.getElementById(elementToMoveId);
	let targetElement = document.getElementById(targetElementId);
	targetElement.after(toMoveElement);
	toMoveElement.classList.remove("tooltip--visible")
	toMoveElement.classList.add("tooltip--hidden")
	//dotNetHelper.dispose();
}

window.showTooltip = (dotNetHelper, elementToMoveId, targetElementQuerySelector) => {
	let toMoveElement = document.getElementById(elementToMoveId);
	let targetElement = document.querySelector(targetElementQuerySelector);
	targetElement.after(toMoveElement);
	toMoveElement.classList.remove("tooltip--hidden")
	toMoveElement.classList.add("tooltip--visible")
	document.addEventListener("keydown", keyDown);
	document.addEventListener("keyup", keyUp);
	let frozen = false;
	function keyDown(e) {

		if (frozen == false) {
			dotNetHelper.invokeMethodAsync('ChangeFreeze', true);
			frozen = true;
		}
	}

	function keyUp(e) {
		if (frozen == true) {
			dotNetHelper.invokeMethodAsync('ChangeFreeze', false);
			frozen = false;
		}
	}
	//dotNetHelper.dispose();
}

//window.hideTooltip = (dotNetHelper) => {
//	prompt("test");
//	dotNetHelper.dispose();
//}

//window.showTooltip = (dotNetHelper) => {
//	prompt("test");
//	dotNetHelper.dispose();
//}

