(() => {
    const key = 'lifedeck.ui.theme';
    const media = window.matchMedia('(prefers-color-scheme: dark)');
    let preference;
    try { preference = localStorage.getItem(key); } catch { }
    const apply = theme => {
        document.documentElement.dataset.theme = theme;
        document.querySelector('meta[name="theme-color"]')?.setAttribute('content', theme === 'dark' ? '#151918' : '#f6f7f5');
    };
    apply(preference === 'light' || preference === 'dark' ? preference : media.matches ? 'dark' : 'light');
    media.addEventListener('change', event => { if (!preference) apply(event.matches ? 'dark' : 'light'); });
    window.lifeDeckTheme = { toggle() {
        preference = document.documentElement.dataset.theme === 'dark' ? 'light' : 'dark';
        apply(preference);
        try { localStorage.setItem(key, preference); } catch { }
    }};
})();
