document.getElementById("startProcessingButton").addEventListener("click", () => {
    const heaterCapacity = parseInt(document.getElementById("heaterCapSlider").value);
    const coolerCapacity = parseInt(document.getElementById("freezerCapSlider").value);
    const shelfCapacity = parseInt(document.getElementById("shelfCapSlider").value);
    const freshnessMin = parseInt(document.getElementById("freshnessMinSlider").value);
    const freshnessMax = parseInt(document.getElementById("freshnessMaxSlider").value);
    const pickupTimeMin = parseInt(document.getElementById("pickupTimeMinSlider").value);
    const pickupTimeMax = parseInt(document.getElementById("pickupTimeMaxSlider").value);
    const orderCount = parseInt(document.getElementById("orderCountSlider").value);

    const payload = {
        heaterCapacity: heaterCapacity,
        coolerCapacity: coolerCapacity,
        shelfCapacity: shelfCapacity,
        freshnessMin: freshnessMin,
        freshnessMax: freshnessMax,
        pickupTimeMin: pickupTimeMin,
        pickupTimeMax: pickupTimeMax,
        orderCount: orderCount
    };

    fetch("/api/Processing/start", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
    })
        .then(response => {
            if (response.ok) {
                console.log("Processing finished successfully.");
            } else {
                response.text().then(text => alert(`Error: ${text}`));
            }
        })
        .catch(err => console.error("Error starting processing:", err));
});

document.getElementById("clearButton").addEventListener("click", () => {
    // Clear all tables
    const tableBodies = [
        "processOrderTable",
        "newOrdersTableBody",
        "cookOrderTable",
        "placeOrderTable",
        "deliveredOrdersTableBody",
        "moveTableBody",
        "discardedTableBody",
        "otherLogsTableBody"
    ];

    tableBodies.forEach(bodyId => {
        const tableBody = document.getElementById(bodyId);
        tableBody.innerHTML = "";
    });

    // Clear storage buckets
    for (const [bucketName, orders] of Object.entries(buckets)) {
        orders.clear();
        updateBucketCount(bucketName);
        const bucketElement = document.getElementById(bucketName + "Bucket");
        bucketElement.innerHTML = ""; // Clear the displayed orders in the bucket
    }
});
