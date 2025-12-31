const connection = new signalR.HubConnectionBuilder()
    .withUrl("/logHub")
    .build();

connection.on("ReceiveLog", (logType, message) => {
    let targetTableBodyId;

    if (message.includes("[Main Thread]: Processed Order Id:")) {
        targetTableBodyId = "newOrdersTableBody";
        const orderId = getOrderId(message);
        const bucketName = getStorageBucketFromTemp(message);
        if (orderId && bucketName) {
            console.log("placing in bucket");
            putOrderInBucket(orderId, bucketName);
        }
    } else if (message.includes("PreprocessSingleOrderState")) {
        targetTableBodyId = "processOrderTable";
    } else if (message.includes("CookOrderState")) {
        targetTableBodyId = "cookOrderTable";
    } else if (message.includes("PlaceOrderState")) {
        targetTableBodyId = "placeOrderTable";
    } else if (message.includes("routed to ideal location successfully.")) {
        console.log("A")
    } else if (message.includes("is given to the delivery person.")) {
        targetTableBodyId = "deliveredOrdersTableBody";
        const orderId = getOrderId(message);
        removeOrderFromBucket(orderId);
    } else if (message.includes("MoveOrderState")) {
        targetTableBodyId = "moveTableBody";
    } else if (message.includes("After RELOCATING items")) {
        console.log("YE");
    } else if (message.includes("DiscardOrderState") || message.includes(" Discarding Highest Pickup Order from the shelf") || message.includes("Discarding hot/cold item with longest pickup time")) {
        targetTableBodyId = "discardedTableBody";
        console.log("removing in bucket");
        const orderId = getOrderId(message);
        removeOrderFromBucket(orderId);
    } else {
        targetTableBodyId = "otherLogsTableBody";
    }

    message = stripSingleLeadingBracketPrefix(message);

    const targetTableBody = document.getElementById(targetTableBodyId);
    const row = document.createElement("tr");

    const messageCell = document.createElement("td");
    messageCell.textContent = message;
    row.appendChild(messageCell);

    targetTableBody.appendChild(row);
});

connection.start().catch(err => console.error(err));
