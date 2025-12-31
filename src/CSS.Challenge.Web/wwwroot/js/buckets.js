function stripSingleLeadingBracketPrefix(message) {
    return message.replace(/^\[[^\]]*\]:\s*/, "");
}

// Storage bucket tracking
const buckets = {
    heater: new Map(),
    cooler: new Map(),
    shelf: new Map()
};

function getOrderId(message) {
    const match = message.match(/Order Id:\s*(\S+)/);
    return match ? match[1] : null;
}

function getStorageBucket(message) {
    if (message.includes("Heater")) {
        return "heater";
    } else if (message.includes("Cooler") || message.includes("Freezer")) {
        return "cooler";
    } else if (message.includes("Shelf")) {
        return "shelf";
    }
    return null;
}

function getStorageBucketFromTemp(message) {
    if (message.includes("Hot")) {
        return "heater";
    } else if (message.includes("Cold")) {
        return "cooler";
    } else if (message.includes("Room")) {
        return "shelf";
    }
    return null;
}
function updateBucketCount(bucketName) {
    const countElement = document.getElementById(bucketName + "Count");
    if (countElement) {
        countElement.textContent = buckets[bucketName].size;
    }
}

function putOrderInBucket(orderId, bucketName) {
    if (!bucketName || !orderId) return;

    // Remove from other buckets if it exists
    for (const [bucket, orders] of Object.entries(buckets)) {
        if (bucket !== bucketName && orders.has(orderId)) {
            orders.delete(orderId);
            updateBucketCount(bucket);
        }
    }

    // Add to correct bucket
    buckets[bucketName].set(orderId, true);
    const bucketElement = document.getElementById(bucketName + "Bucket");
    
    const badge = document.createElement("div");
    badge.id = "order-" + orderId;
    badge.className = "order-badge " + bucketName;
    badge.textContent = "Order #" + orderId;
    bucketElement.appendChild(badge);
    updateBucketCount(bucketName);
}

function removeOrderFromBucket(orderId) {
    for (const [bucketName, orders] of Object.entries(buckets)) {
        if (orders.has(orderId)) {
            const badgeElement = document.getElementById("order-" + orderId);
            if (badgeElement) {
                badgeElement.classList.add("removing");
                setTimeout(() => {
                    badgeElement.remove();
                    orders.delete(orderId);
                    updateBucketCount(bucketName);
                }, 400);
            }
            return;
        }
    }
}
