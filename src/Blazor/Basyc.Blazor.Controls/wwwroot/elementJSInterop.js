export function changeStyleById(elementId, cssText) {
	const element = document.getElementById(elementId);
	element.style.cssText = cssText;
}

export function changeStyleByReference(elementReference, cssText) {
	elementReference.style.cssText = cssText;
}

export function getElementById(elementId) {
	const element = document.getElementById(elementId);
	return element;
}


