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
	if (s === 'Paid') return 'paid';
	if (s === 'Completed') return 'completed';
	if (s === 'Delivered') return 'delivered';
	return '';
};

// ===== State =================================================================
let state = {
	orders: [...initialOrders],
	selectedId: initialOrders[0]?.id ?? null,
	statusFilter: 'all',
	amountFilter: '100-1500', // label default
	sortBy: 'date-desc'
};

// ===== Render: Orders list ====================================================
function renderOrders(orders = null) {
	// Update state if new orders provided
	if (orders) {
		state.orders = [...orders];
		state.selectedId = orders[0]?.id ?? null;
	}
	
	const list = document.getElementById('ordersBody');
	if (!list) return;
	list.innerHTML = '';

	const filtered = state.orders
		.filter((o) => state.statusFilter === 'all' ? true : o.status === state.statusFilter)
		.filter((o) => {
			const range = state.amountFilter;
			if (!range || range === 'all') return true;
			if (range === '100-1500') return o.total >= 100 && o.total <= 1500;
			const [min, max] = range.split('-').map(Number);
			return o.total >= min && o.total <= max;
		})
		.sort((a, b) => {
			switch (state.sortBy) {
				case 'date-asc': return new Date(a.date) - new Date(b.date);
				case 'date-desc': return new Date(b.date) - new Date(a.date);
				case 'total-asc': return a.total - b.total;
				case 'total-desc': return b.total - a.total;
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

		// customer
		const c2 = el('div', 'col');
		const cust = el('div', 'customer');
		const av = el('div', 'avatar');
		av.style.setProperty('--bg', order.customer.color);
		av.textContent = order.customer.initials;
		const nameBox = el('div');
		nameBox.appendChild(el('div', 'name', order.customer.name));
		cust.append(av, nameBox);
		c2.appendChild(cust);

		// status
		const c3 = el('div', 'col');
		const badge = el('span', `badge ${statusClass(order.status).toLowerCase()}`, order.status);
		c3.appendChild(badge);

		// total
		const c4 = el('div', 'col col-right', fmtMoney(order.total));

		// date
		const c5 = el('div', 'col',
			new Intl.DateTimeFormat('en-US', { month: 'short', day: '2-digit' }).format(new Date(order.date))
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

	const date = new Date(order.date);
	const dateStr =
		new Intl.DateTimeFormat('en-US', { month: 'short', day: 'numeric' }).format(date) +
		' · ' +
		new Intl.DateTimeFormat('en-US', { hour: '2-digit', minute: '2-digit' }).format(date);

    var detailDateEl = document.getElementById('detailDate');
    if (detailDateEl) detailDateEl.textContent = dateStr;

	const avatar = document.getElementById('detailAvatar');
	if (avatar) {
		avatar.style.setProperty('--bg', order.customer.color);
		avatar.textContent = order.customer.initials;
	}

	var detailNameEl = document.getElementById('detailName');
	if (detailNameEl) detailNameEl.textContent = order.customer.name;

	const list = document.getElementById('detailItems');
	if (list) {
		list.innerHTML = '';
		for (const item of order.items) {
			const li = el('li', 'item');
			const t = el('div', 'thumb', '🧰');
			const meta = el('div');
			meta.appendChild(el('div', 'item-title', item.title));
			meta.appendChild(el('div', 'item-note', 'SKU • Qty 1'));
			const p = el('div', 'item-price', fmtMoney(item.price));
			li.append(t, meta, p);
			list.appendChild(li);
		}
	}

	const total = order.items.reduce((s, it) => s + it.price, 0);

	var detailTotalEl = document.getElementById('detailTotal');
	if (detailTotalEl) detailTotalEl.textContent = fmtMoney(total);
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