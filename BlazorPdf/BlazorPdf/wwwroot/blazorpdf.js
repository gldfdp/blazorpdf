"strict mode";

import * as pdfjsLib from './pdf.mjs';
import * as pdfjsWorker from './pdf.worker.mjs';

export function init(dotnetHelper, element, options) {

    var viewer = new PdfViewer(dotnetHelper, element, options);

    return viewer.initialize();
}

const instances = [];

class PdfViewer {

    constructor(dotnetHelper, element, options) {
        this.dotnetHelper = dotnetHelper;
        this.element = element;
        this.options = options;
    }

    scale = 1;

    initialize() {
        return new Promise(async (resolve, reject) => {

            console.log("initialize pdf viewer", this.dotnetHelper, this.element, this.options);
            instances[this.element.id] = this;

            this.pdf = await pdfjsLib.getDocument(this.options.url).promise;
            this.pageCount = this.pdf.numPages;

            this.canvas = document.createElement("canvas");
            this.canvas.id = "pdf-canvas-" + this.element.id;
            this.element.appendChild(this.canvas);

            this.context = this.canvas.getContext("2d");

            await this.renderPage(1);

            resolve({ elementId: this.element.id, pageCount: this.pageCount });
        });
    }

    async renderPage(pageNumber) {

        console.log("render page", pageNumber, this.pdf);

        const page = await this.pdf.getPage(pageNumber);

        console.log("page loaded", page);

        const viewport = page.getViewport({ scale: this.scale });

        var outputScale = window.devicePixelRatio || 1;

        this.canvas.width = Math.floor(viewport.width * outputScale);
        this.canvas.height = Math.floor(viewport.height * outputScale);
        this.canvas.style.width = Math.floor(viewport.width) + "px";
        this.canvas.style.height = Math.floor(viewport.height) + "px";

        var transform = outputScale !== 1
            ? [outputScale, 0, 0, outputScale, 0, 0]
            : null;

        var renderContext = {
            canvasContext: this.context,
            transform: transform,
            viewport: viewport
        };

        await page.render(renderContext);

        console.log("page rendered", pageNumber);
    }
}