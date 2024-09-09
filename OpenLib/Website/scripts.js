function serializeForm() {
    const form = document.getElementById('configForm');
    const elements = form.elements;
    let result = [];

    for (let element of elements) {
                if (element.name) {
                    if (element.type === 'radio') {
                        if (element.checked) {
        result.push(`${element.name}:${element.value}`);
                        }
                    } else if(element.type === 'checkbox') {
                        if (element.checked) {
        result.push(`${element.name}:true`);
                        } else {
        result.push(`${element.name}:false`);
                        }
                    } else {
        result.push(`${element.name}:${element.value}`);
                    }
                }
    }

    const compressedData = compressData(result.join(';:; '));
    document.getElementById('rawData').textContent = result.join(';:; ');
    document.getElementById('compressedData').textContent = compressedData;
}

//MinCodes = 4
function parseConfig(text) {
    const lines = text.split('\n');
    const notConfig = ['#', '['];

    lines.forEach(str => {
        if (!notConfig.some(char => str.startsWith(char))) {
            const pair = str.split(" = ");
            console.log("attempting to update config item on site for " + str);
            if(pair[0] && pair[1]) //truthy
                updateConfig(pair[0], pair[1]);
        }
        else {
            console.log("below line is not a config item\n" + str);
        }
    });
}

function updateConfig(key, value) {
    if (key === null || value === null) {
        console.warn("Cannot update key-value pair, one item is NULL");
        return;
    }

    key = key.trim();
    value = value.trim();

    const matching = document.getElementsByName(key);
    if (matching.length === 1) {
        const element = matching[0];
        let typ = matching[0].getAttribute("type");
        let next = element.nextElementSibling;
        if (typ === "checkbox") {
            if (value === "true")
                element.setAttribute("checked", "checked");
            else
                element.removeAttribute("checked");
        } else if (element.hasAttribute("value")) {
            if (typ === "range") {
                if (next !== null) {
                    next.setAttribute("value", value);
                    next.textContent = value;
                }
                element.setAttribute("value", value);
            } else {
                element.setAttribute("value", value);
                element.textContent = value;
                console.log("not a range setting values");
            }
            
        } else {
            console.warn("Unable to find attribute to update for: " + key + value);
        }
    } else {
        matching.forEach(doc => {
            if (doc.hasAttribute("type") && doc.hasAttribute("value")) {
                let atr = doc.getAttribute("value");
                let typ = doc.getAttribute("type");
                if (typ === "radio") {
                    if (atr === value) {
                        doc.setAttribute("checked", "checked");
                    } else {
                        console.log("not the button we are looking for, skipping & removing attribute");
                        doc.removeAttribute("checked");
                    }
                }
            }
        });
    }
}


function loadFileAsText() {
    var fileToLoad = document.getElementById("fileInput").files[0];

    var fileReader = new FileReader();
    fileReader.onload = function (fileLoadedEvent) {
        var textFromFileLoaded = fileLoadedEvent.target.result;
        parseConfig(textFromFileLoaded);
    };

    fileReader.readAsText(fileToLoad, "UTF-8");
}


function clearText() {
    document.getElementById('rawData').textContent = '';
    document.getElementById('compressedData').textContent = '';
		
}


function compressData(data) {

		// Convert query string to a Uint8Array
		const uint8Array = new TextEncoder().encode(data);

    // Compress using pako
    const compressed = pako.gzip(uint8Array);

    // Convert compressed data to Base64
    return btoa(String.fromCharCode(...new Uint8Array(compressed)));
}
