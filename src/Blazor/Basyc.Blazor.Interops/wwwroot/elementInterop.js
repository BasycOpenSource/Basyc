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

export function getCssVariable(elementReference, name) {
    var cssVarName = "--" + name;
    var value = elementReference.style.getPropertyValue(cssVarName);
    var computedStyle = getComputedStyle(elementReference);
    value = computedStyle.getPropertyValue(cssVarName);
    return value;
}

export function setCssVariable(elementReference, name, newValue) {
    var cssVarName = "--" + name;
    //var style = elementReference.style;
    //try {

    //    style.setProperty(name, newValue);
    //}
    //catch {

    //}
    elementReference.style.setProperty(cssVarName, newValue);
}

export function setCssProperty(elementReference, name, newValue) {
    elementReference.style.setProperty(name, newValue);
}


