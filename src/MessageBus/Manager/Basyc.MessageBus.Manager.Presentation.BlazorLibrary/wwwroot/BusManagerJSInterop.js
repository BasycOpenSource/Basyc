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
