export function addBusMangerStaticFiles() {
    var head = document.getElementsByTagName('HEAD')[0];
    var link = document.createElement('link');
    link.rel = 'stylesheet';
    link.type = 'text/css';
    link.href = '_content/MudBlazor/MudBlazor.min.css';
    head.appendChild(link);

    var body = document.getElementsByTagName('body')[0];
    var script = document.createElement('script');
    script.src = '_content/MudBlazor/MudBlazor.min.js';
    body.appendChild(script);
}

//addBusMangerStaticFiles();

export function showPrompt(message) {
	return prompt(message, 'Type anything here');
}

export function addDragToScroll(query) {
	const slider = document.querySelector(query);
	let isDown = false;
	let startX;
	let scrollLeft;

	slider.addEventListener("mousedown", e => {
		isDown = true;
		slider.classList.add("active");
		startX = e.pageX - slider.offsetLeft;
		scrollLeft = slider.scrollLeft;
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
		const walk = x - startX;
		slider.scrollLeft = scrollLeft - walk;
	});
}
