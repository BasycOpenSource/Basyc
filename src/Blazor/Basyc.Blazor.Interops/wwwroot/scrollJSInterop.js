//https://discourse.wicg.io/t/drag-to-scroll-a-simple-way-to-scroll-sideways-on-desktop/3627

export function addDragToScroll(elementId) {
	const slider = document.getElementById(elementId);
	if (slider.classList.contains("horizontalScroll"))
		return;
	slider.classList.add("horizontalScroll");
	let isDown = false;
	let startX;
	let startY;
	let scrollLeft;
	let scrollTop;

	slider.addEventListener("mousedown", e => {
		isDown = true;
		slider.classList.add("active");
		startX = e.pageX - slider.offsetLeft;
		startY = e.pageY - slider.offsetTop;
		scrollLeft = slider.scrollLeft;
		scrollTop = slider.scrollTop;
	});
	slider.addEventListener("mouseleave", () => {
		isDown = false;
		slider.classList.remove("active");
	});
	slider.addEventListener("mouseup", () => {
		isDown = false;
		slider.classList.remove("active");
	});
	slider.addEventListener("mousemove", e => {
		if (!isDown) return;
		e.preventDefault();
		const x = e.pageX - slider.offsetLeft;
		const y = e.pageY - slider.offsetTop;
		const walkX = x - startX;
		const walkY = y - startY;
		slider.scrollLeft = scrollLeft - walkX;
		slider.scrollTop = scrollTop - walkY;
	});
}
