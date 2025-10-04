(function () {
    'use strict';

    const headerSelectors = [
        '.header-container.header-chrome'
    ];

    const footerSelectors = [
        '.minerva-footer'
    ];

    function hideElements(selectors) {
        selectors.forEach(selector => {
            try {
                const elements = document.querySelectorAll(selector);
                elements.forEach(element => {
                    if (element && element.style) {
                        element.style.display = 'none';
                    }
                });

            } catch (e) {
                console.debug('Failed to hide selector:', selector, e);
            }
        });
    }

    hideElements(headerSelectors);
    hideElements(footerSelectors);

    return 'Wikipedia artciel ui formatting completed';
})();