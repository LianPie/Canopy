self.addEventListener('push', function (event) {
    if (!event.data) return;

    let data = {};
    try { data = event.data.json(); } catch { return; }

    const text = buildText(data.type, tryParse(data.payload));

    event.waitUntil(
        self.registration.showNotification('Canopy', {
            body: text,
            icon: '/favicon.ico',
            badge: '/favicon.ico',
            data: { url: '/dashboard' }
        })
    );
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();
    const url = event.notification.data?.url ?? '/dashboard';
    event.waitUntil(clients.openWindow(url));
});

function buildText(type, p) {
    switch (type) {
        case 0: return `${p.invitedBy} invited you to join ${p.groupTitle}`;
        case 1: return `${p.acceptedBy} accepted your invite to ${p.groupTitle}`;
        case 2: return `${p.declinedBy} declined your invite to ${p.groupTitle}`;
        case 3: return `${p.assignedBy} assigned you a task: ${p.taskTitle}`;
        case 4: return `${p.assignedBy} assigned you to project ${p.projectTitle}`;
        default: return 'You have a new notification';
    }
}

function tryParse(str) {
    try { return JSON.parse(str); } catch { return {}; }
}
