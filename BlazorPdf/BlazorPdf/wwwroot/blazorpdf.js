"strict mode";

import * as pdfjsLib from './pdf.mjs';
import * as pdfjsWorker from './pdf.worker.mjs';

export function init(dotnetHelper, element, options) {

    // we check if the fontawesome css is already loaded
    const fontAwesomeCheckSpan = document.createElement("span");
    fontAwesomeCheckSpan.className = "fa fa-regular";
    fontAwesomeCheckSpan.style.display = "none";
    document.body.appendChild(fontAwesomeCheckSpan);

    const fontAwesomeLoaded = window.getComputedStyle(fontAwesomeCheckSpan).fontFamily.indexOf("FontAwesome") > -1;

    if (!fontAwesomeLoaded) {

        const fontAwesomeLink = document.createElement("link");
        fontAwesomeLink.rel = "stylesheet";
        fontAwesomeLink.href = "/_content/BlazorPdf/fontawesome.min.css";
        document.head.appendChild(fontAwesomeLink);

        const fontAwesomeRegularLink = document.createElement("link");
        fontAwesomeRegularLink.rel = "stylesheet";
        fontAwesomeRegularLink.href = "/_content/BlazorPdf/solid.min.css";
        document.head.appendChild(fontAwesomeRegularLink);
    }

    document.body.removeChild(fontAwesomeCheckSpan);

    // we we load the css
    const styleLink = document.createElement("link");
    styleLink.rel = "stylesheet";
    styleLink.href = "/_content/BlazorPdf/blazorpdf.css?v=" + new Date().getTime();
    document.head.appendChild(styleLink);

    var viewer = new PdfViewer(dotnetHelper, element, options);

    return viewer.initialize();
}

export function renderPage(elementId, pageNumber) {
    var viewer = instances[elementId];

    if (!viewer) {
        console.error("Viewer not found", elementId, instances);
        return;
    }

    viewer.renderPage(pageNumber);
}

export function startSignaturePosition(elementId, signatureIndex) {
    var viewer = instances[elementId];
    if (!viewer) {
        console.error("Viewer not found", elementId, instances);
        return;
    }

    viewer.startSignaturePosition(signatureIndex);
}

const instances = [];

class PdfViewer {

    signatures = [];

    /** @type {{ "none" | "signature"}} **/
    currentAction = "none";

    constructor(dotnetHelper, element, options) {
        this.dotnetHelper = dotnetHelper;
        this.element = element;
        this.options = options;
    }

    /** @type {HTMLCanvasElement} **/
    canvas = null;

    scale = 1;

    initialize() {
        return new Promise(async (resolve, reject) => {

            console.log("initialize pdf viewer", this.dotnetHelper, this.element, this.options);
            instances[this.element.id] = this;

            this.pdf = await pdfjsLib.getDocument(this.options.url).promise;
            this.pageCount = this.pdf.numPages;

            console.log("canvas", this.element.querySelector("canvas"));

            this.canvas = this.element.querySelector("canvas");

            this.context = this.canvas.getContext("2d");

            this.canvas.addEventListener("click", (e) => {
                console.log("click", e);
                this.handleCanvasClick(e);
            });

            if (this.options.signatures) {
                this.signatures = this.options.signatures.map(signature => {
                    return {
                        displayName: signature.displayName,
                        height: signature.height,
                        index: signature.index,
                        page: signature.page,
                        width: signature.width,
                        x: signature.x,
                        y: signature.y
                    };
                });
            }

            await this.renderPage(1);

            resolve({ elementId: this.element.id, pageCount: this.pageCount });
        });
    }

    async handleCanvasClick(e) {

        switch (this.currentAction) {
            case "signature":
                this.handleSignaturePosition(e.offsetX, e.offsetY);
                break;
        }

        this.dotnetHelper.invokeMethodAsync("OnPageClick", { x: e.offsetX, y: e.offsetY });
    }

    async renderPage(pageNumber) {

        console.log("render page", pageNumber, this.pdf);

        this.page = await this.pdf.getPage(pageNumber);

        console.log("page loaded", this.page);

        const viewport = this.page.getViewport({ scale: this.scale });

        console.log("viewport", viewport);

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

        await this.page.render(renderContext);

        setTimeout(() => {

            var pageSignatures = this.signatures.filter(s => s.page === pageNumber);

            pageSignatures.forEach(signature => this.renderSignature(signature));
        }, 100);

        console.log("page rendered", pageNumber);

        this.currentPage = pageNumber;
    }

    async startSignaturePosition(signatureIndex) {
        this.currentAction = "signature";
        this.currentSignature = this.signatures[signatureIndex];
        this.canvas.style.cursor = "crosshair";
    }

    async handleSignaturePosition(x, y) {
        this.currentSignature.x = this.xToMillimeters(x);
        this.currentSignature.y = this.yToMillimeters(y);
        this.currentSignature.page = this.currentPage;

        this.canvas.style.cursor = "auto";
        this.currentAction = "none";

        console.log("signature position", this.currentSignature);

        await this.renderPage(this.currentPage);

        this.dotnetHelper.invokeMethodAsync("OnSignaturePosition", this.currentSignature);
    }

    xToMillimeters(x) {

        var width = this.page.view[2] - this.page.view[0];
        var widthInches = width / 72;
        var widthMillimeters = widthInches * 25.4;

        return Math.round((x / this.canvas.width) * widthMillimeters);
    }

    yToMillimeters(y) {
        var height = this.page.view[3] - this.page.view[1];
        var heightInches = height / 72;
        var heightMillimeters = heightInches * 25.4;
        return Math.round((y / this.canvas.height) * heightMillimeters);
    }

    xToCanvasX(x) {
        var width = this.page.view[2] - this.page.view[0];
        var widthInches = width / 72;
        var widthMillimeters = widthInches * 25.4;

        return (x / widthMillimeters) * this.canvas.width;
    }

    yToCanvasY(y) {
        var height = this.page.view[3] - this.page.view[1];
        var heightInches = height / 72;
        var heightMillimeters = heightInches * 25.4;
        return (y / heightMillimeters) * this.canvas.height;
    }

    renderSignature(signature) {
       
        const x = this.xToCanvasX(signature.x);
        const y = this.yToCanvasY(signature.y);

        console.log("render signature", signature, x, y);


        const width = signature.width;
        const height = signature.height;

        this.context.strokeStyle = "red";
        this.context.lineWidth = 2;
        this.context.strokeRect(x, y, width, height);
        this.context.fillText(signature.displayName, x, y);
    }
}