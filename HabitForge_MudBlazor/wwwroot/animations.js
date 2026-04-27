(function () {
    function animateNewElements() {
        const observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                mutation.addedNodes.forEach(function (node) {
                    if (node.nodeType !== 1) return;
                    const targets = [
                        '.mud-card', '.mud-paper', '.mud-button',
                        '.mud-table-row', '.mud-list-item', '.mud-nav-item',
                        '.mud-chip', '.mud-alert', '.mud-dialog',
                        '.mud-menu-item', '.mud-expansion-panel', '.mud-timeline-item',
                        '.mud-treeview-item', '.mud-breadcrumb-item'
                    ];
                    targets.forEach(function (selector) {
                        const elements = node.matches && node.matches(selector)
                            ? [node]
                            : Array.from(node.querySelectorAll ? node.querySelectorAll(selector) : []);
                        elements.forEach(function (el) {
                            el.style.opacity = '0';
                            requestAnimationFrame(function () {
                                el.style.opacity = '';
                            });
                        });
                    });
                });
            });
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }

    function addRippleToButtons() {
        document.addEventListener('click', function (e) {
            const btn = e.target.closest('.mud-button-root');
            if (!btn) return;
            const rect = btn.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            const ripple = document.createElement('span');
            ripple.style.cssText = `
                position:absolute;
                border-radius:50%;
                background:rgba(255,255,255,0.35);
                width:${size}px;height:${size}px;
                left:${x}px;top:${y}px;
                animation:rippleEffect 0.6s linear forwards;
                pointer-events:none;
            `;
            if (!btn.style.position || btn.style.position === 'static') {
                btn.style.position = 'relative';
            }
            btn.style.overflow = 'hidden';
            btn.appendChild(ripple);
            ripple.addEventListener('animationend', function () { ripple.remove(); });
        });

        const rippleStyle = document.createElement('style');
        rippleStyle.textContent = `
            @keyframes rippleEffect {
                0% { transform: scale(0); opacity: 1; }
                100% { transform: scale(4); opacity: 0; }
            }
        `;
        document.head.appendChild(rippleStyle);
    }

    function staggerTableRows() {
        document.addEventListener('DOMNodeInserted', function (e) {
            if (!e.target || !e.target.classList) return;
            if (e.target.classList.contains('mud-table-container')) {
                const rows = e.target.querySelectorAll('.mud-table-row');
                rows.forEach(function (row, i) {
                    row.style.animationDelay = (i * 50) + 'ms';
                });
            }
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            animateNewElements();
            addRippleToButtons();
            staggerTableRows();
        });
    } else {
        animateNewElements();
        addRippleToButtons();
        staggerTableRows();
    }
})();