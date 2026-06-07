// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Displaying page loading time
const observer = new PerformanceObserver((list) => {
    list.getEntries().forEach((entry) => {
        if (entry.entryType === "navigation") {
            const loadTime = entry.domContentLoadedEventEnd - entry.startTime;
            try {
                document.getElementById("loadTime").innerText = "Betöltési idő: " + Math.round(loadTime) + " ms";
            }
            catch (TypeError) {

            }
        }
    });
});

observer.observe({ type: "navigation", buffered: true });

// When a new status selected in Details view, an extra field appears
function getActionExtra(event) {
    // Get the select menu and input fields
    var actionExtras = document.getElementsByClassName('action-extra');

    // Change the display
    for (var i = 0; i < actionExtras.length; i++) {
        if (event.target.value == actionExtras[i].id) {
            actionExtras[i].style.display = 'block';
        } else {
            actionExtras[i].style.display = 'none';
        }
    }
}

// Adding a new select menu when click on the button
function addSelect(button) {
    // Get the select element to duplicate
    const container = button.previousElementSibling;
    var select = container.lastElementChild;

    if (select.value != "0") {
        var clone = select.cloneNode(true);
        clone.id = ""; // Remove id to avoid duplicates
        container.appendChild(clone);
        clone.focus();
    }
}

// Adding a new input field when clicked on the button
function addInputField(button) {
    // Get the input element to duplicate
    const container = button.previousElementSibling;
    var field = container.lastElementChild;

    if (field.value != "") {
        var clone = field.cloneNode(true);
        clone.id = ""; // Remove id to avoid duplicates
        clone.value = "";
        container.appendChild(clone);
        clone.focus();
    }
}

// Adding a new form block (div) when click on the button
function addFormDiv(button) {
    // Get the div element to duplicate
    var container = button.previousElementSibling;
    var itemIndex = container.children.length;
    var lastItem = container.lastElementChild;
    var clone = lastItem.cloneNode(true);
    container.appendChild(clone);
}


//document.getElementById('addItemButton').addEventListener('click', function () {
//    var container = document.getElementById('itemsContainer');
//    var itemIndex = container.children.length;
//    var newItem = document.createElement('div');
//    newItem.innerHTML = ` 
//        <label for="Items_${itemIndex}__Property">Property</label> 
//        <input id="Items_${itemIndex}__Property" name="Items[${itemIndex}].Property" /> 
//        `;
//    container.appendChild(newItem);
//});


//var field = container.lastElementChild;

//if (field.value != "") {
//    var clone = field.cloneNode(true);
//    clone.id = ""; // Remove id to avoid duplicates
//    clone.value = "";
//    container.appendChild(clone);
//    clone.focus();
//}