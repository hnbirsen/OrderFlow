const initialOrders = Array.isArray(window.demoOrders) ? window.demoOrders : [];

// ===== Helpers ===============================================================
const fmtMoney = (n) =>
	new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(n);

const el = (tag, cls, text) => {
	const node = document.createElement(tag);
	if (cls) node.className = cls;
	if (text != null) node.textContent = text;
	return node;
};

const statusClass = (s) => {
	if (!s) return '';
	if (s === 'New') return 'new';
	if (s === 'Processing') return 'processing';
	if (s === 'Sent') return 'sent';
	if (s === 'Completed') return 'completed';
	if (s === 'Cancelled') return 'cancelled';
	if (s === 'Refunded') return 'refunded';
	return '';
};

// ===== State =================================================================
let state = {
	orders: [...initialOrders],
	selectedId: initialOrders[initialOrders.length - 1]?.id ?? null,
	statusFilter: 'all',
	sortBy: 'date-desc'
};

// ===== Render: Orders list ====================================================
function renderOrders(orders = null) {
	// Update state if new orders provided
	if (orders) {
		state.orders = [...orders];
		state.selectedId = orders[orders.length - 1]?.id ?? null;
	}
	const list = document.getElementById('ordersBody');
	if (!list) return;
	list.innerHTML = '';

	const filtered = state.orders
		.map(o => normalizeOrder(o))
		.filter((o) => state.statusFilter === 'all' ? true : o.status === state.statusFilter)
		.sort((a, b) => {
			switch (state.sortBy) {
				case 'date-asc': return new Date(a.createdAt) - new Date(b.createdAt);
				case 'date-desc': return new Date(b.createdAt) - new Date(a.createdAt);
				default: return 0;
			}
		});

	for (const order of filtered) {
		const row = el('div', 'order-row' + (order.id === state.selectedId ? ' selected' : ''));
		row.setAttribute('role', 'listitem');

		// check
		const c0 = el('div', 'col col-check');
		const cb = el('input');
		cb.type = 'checkbox';
		cb.addEventListener('click', (e) => e.stopPropagation());
		c0.appendChild(cb);

		// id
		const c1 = el('div', 'col');
		c1.appendChild(el('div', 'subtle', order.id));

		// description
		const c2 = el('div', 'col', order.description);

		// status
		const c3 = el('div', 'col');
		const badge = el('span', `badge ${statusClass(order.status).toLowerCase()}`, order.status);
		c3.appendChild(badge);

		// items count
		const c4 = el('div', 'col', String(Object.keys(order.items || {}).length));

		// date
		const c5 = el('div', 'col',
			new Intl.DateTimeFormat('en-US', { month: 'short', day: '2-digit' }).format(new Date(order.createdAt))
		);

		// kebab
		const c6 = el('div', 'col col-kebab kebab', '⋯');

		row.append(c0, c1, c2, c3, c4, c5, c6);

		row.addEventListener('click', () => {
			state.selectedId = order.id;
			renderOrders();
			renderDetail(order);
		});

		list.appendChild(row);
	}

	// Sync detail if selected disappears (due to filters)
	const selected = filtered.find((o) => o.id === state.selectedId) ?? filtered[0];
	if (selected) {
		state.selectedId = selected.id;
		renderDetail(selected);
	} else {
		clearDetail();
	}
}

// ===== Render: Detail panel ===================================================
function clearDetail() {
	const d = (id) => document.getElementById(id);
	if (d('detailOrder')) d('detailOrder').textContent = 'Order —';
	if (d('detailStatus')) { d('detailStatus').className = 'badge'; d('detailStatus').textContent = ''; }
	if (d('detailDate')) d('detailDate').textContent = '';
	if (d('detailAvatar')) d('detailAvatar').textContent = '?';
	if (d('detailName')) d('detailName').textContent = '—';
	if (d('detailItems')) d('detailItems').innerHTML = '';
	if (d('detailTotal')) d('detailTotal').textContent = '$—';
}

function renderDetail(order) {
	const orderEl = document.getElementById('detailOrder');
	if (!orderEl) return;
	orderEl.textContent = `Order ${order.id}`;
	const st = document.getElementById('detailStatus');
	st.className = `badge ${statusClass(order.status).toLowerCase()}`;
	st.textContent = order.status;

	const date = new Date(order.createdAt);
	const dateStr =
		new Intl.DateTimeFormat('en-US', { month: 'short', day: 'numeric' }).format(date) +
		' · ' +
		new Intl.DateTimeFormat('en-US', { hour: '2-digit', minute: '2-digit' }).format(date);

    var detailDateEl = document.getElementById('detailDate');
    if (detailDateEl) detailDateEl.textContent = dateStr;

	const avatar = document.getElementById('detailAvatar');
	if (avatar) {
		avatar.style.setProperty('--bg', '#e5e7eb');
		avatar.textContent = (String(order.id).slice(-2) || '??');
	}

	var detailNameEl = document.getElementById('detailName');
	if (detailNameEl) detailNameEl.textContent = order.description;

	const list = document.getElementById('detailItems');
	if (list) {
		list.innerHTML = '';
		const items = order.items || {};
		for (const key of Object.keys(items)) {
			const qty = items[key];
			const li = el('li', 'item');
			const t = el('div', 'thumb', '📦');
			const meta = el('div');
			meta.appendChild(el('div', 'item-title', key));
			meta.appendChild(el('div', 'item-note', `Qty ${qty}`));
			const p = el('div', 'item-price', '');
			li.append(t, meta, p);
			list.appendChild(li);
		}
	}

	var detailTotalEl = document.getElementById('detailTotal');
	if (detailTotalEl) detailTotalEl.textContent = '';

	['detail-process', 'detail-ship', 'detail-track', 'detail-refund'].forEach(id => {
		var btn = document.getElementById(id);
		if (btn) btn.hidden = true;
	});

	switch (order.status) {
		case 'New':
			var btn = document.getElementById('detail-process');
			if (btn) btn.hidden = false;
			break;
        case 'Processing':
			var btn = document.getElementById('detail-ship');
			if (btn) btn.hidden = false;
			break;
		case 'Sent':
			var btn = document.getElementById('detail-track');
			if (btn) btn.hidden = false;
			break;
		case 'Completed':
			var btn = document.getElementById('detail-refund');
			if (btn) btn.hidden = false;
			break;
		default:
			break;
	}
}

const detailCta = document.querySelector('.detail-cta');
if (detailCta) {
	detailCta.querySelectorAll('button').forEach(function (btn) {
		btn.onclick = function () {
			const newStatus = btn.getAttribute('data-new-status');
			if (newStatus) updateStatus(newStatus);
		};
	});
}

// Normalize incoming data (OrderDto) in a case-insensitive way
function normalizeOrder(o) {
	if (!o || typeof o !== 'object') return { id: null, description: '', status: '', items: {}, createdAt: null };
	const lowerKeyed = {};
	for (const k in o) {
		if (Object.prototype.hasOwnProperty.call(o, k)) {
			lowerKeyed[k.toLowerCase()] = o[k];
		}
	}
	return {
		id: lowerKeyed['id'] ?? null,
		description: lowerKeyed['description'] ?? '',
		status: lowerKeyed['status'] ?? '',
		items: lowerKeyed['items'] ?? {},
		createdAt: lowerKeyed['createdat'] ?? null
	};
}

// ===== Menus / Filters ========================================================
function attachMenu(buttonId, menuId, onSelect, initialLabel) {
	const btn = document.getElementById(buttonId);
	const menu = document.getElementById(menuId);
	if (!btn || !menu) return;
	const label = btn.querySelector('span');

	btn.addEventListener('click', () => {
		const isOpen = menu.classList.contains('open');
		document.querySelectorAll('.menu.open').forEach((m) => m.classList.remove('open'));
		if (!isOpen) menu.classList.add('open');
	});

	menu.addEventListener('click', (e) => {
		const item = e.target.closest('.menu-item');
		if (!item) return;
		const value = item.dataset.value;
		onSelect(value, item.textContent);
		menu.classList.remove('open');
		if (label) label.textContent = item.textContent;
	});

	document.addEventListener('click', (e) => {
		if (!menu.contains(e.target) && !btn.contains(e.target)) {
			menu.classList.remove('open');
		}
	});

	if (initialLabel && label) label.textContent = initialLabel;
}

attachMenu('statusBtn', 'statusMenu', (value) => {
	state.statusFilter = value;
	renderOrders();
});
attachMenu('amountBtn', 'amountMenu', (value) => {
	state.amountFilter = value;
	renderOrders();
}, '$100—$1500');
attachMenu('sortBtn', 'sortMenu', (value) => {
	state.sortBy = value;
	renderOrders();
});

// Master checkbox visual only
const master = document.getElementById('masterCheck');
if (master) {
	master.addEventListener('change', (e) => {
		const all = document.querySelectorAll('.orders-body input[type="checkbox"]');
		all.forEach((c) => (c.checked = e.target.checked));
	});
}

// Close detail on small screens (if you adapt layout responsively)
const closeBtn = document.getElementById('closeDetail');
if (closeBtn) closeBtn.addEventListener('click', () => clearDetail());

// ===== Init ===================================================================
// renderOrders() will be called from individual pages with their data

// ===== Sidebar Toggle =========================================================
function initSidebarToggle() {
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const surface = document.querySelector('.surface');
    
    if (!sidebar || !sidebarToggle || !surface) return;
    
    // Load saved state from localStorage
    const isCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
    if (isCollapsed) {
        sidebar.classList.add('collapsed');
        surface.classList.add('sidebar-collapsed');
    }
    
    sidebarToggle.addEventListener('click', () => {
        const isCurrentlyCollapsed = sidebar.classList.contains('collapsed');
        
        if (isCurrentlyCollapsed) {
            // Expand sidebar
            sidebar.classList.remove('collapsed');
            surface.classList.remove('sidebar-collapsed');
            localStorage.setItem('sidebarCollapsed', 'false');
        } else {
            // Collapse sidebar
            sidebar.classList.add('collapsed');
            surface.classList.add('sidebar-collapsed');
            localStorage.setItem('sidebarCollapsed', 'true');
        }
    });
}

// Initialize sidebar toggle when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initSidebarToggle);
} else {
    initSidebarToggle();
}

function updateStatus(newStatus) {
	if (!state.selectedId) return;
	const order = state.orders.find(o => o.id === state.selectedId);
	if (!order) return;

	// Send status update to server
	fetch(`/orders/update-status`, {
		method: 'PUT',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({
			OrderId: state.selectedId,
			NewStatus: newStatus
		})
	})
	.then(response => {
		if (!response.ok) throw new Error('Network response was not ok');
		return response.json();
	})
	.then(data => {
		// Optionally handle server response
	})
	.catch(error => {
		console.error('Error updating status:', error);
	});
}