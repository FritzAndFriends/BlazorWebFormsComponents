// Fuse.js based search for BlazorWebFormsComponents
// Uses Fuse.js for fuzzy search with highlighting support

let fuseInstance = null;
let componentData = [];

/**
 * Initialize Fuse.js with component data
 * @param {Array} components - Array of component objects with name, category, route, description, keywords
 */
window.initializeSearch = function(components) {
    componentData = components;
    
    const options = {
        includeScore: true,
        includeMatches: true,
        threshold: 0.4,
        minMatchCharLength: 2,
        keys: [
            { name: 'name', weight: 0.4 },
            { name: 'description', weight: 0.3 },
            { name: 'category', weight: 0.15 },
            { name: 'keywords', weight: 0.15 }
        ]
    };
    
    fuseInstance = new Fuse(componentData, options);
    return true;
};

/**
 * Search components and return results
 * @param {string} query - Search query string
 * @param {number} maxResults - Maximum number of results to return (default 8)
 * @returns {Array} Array of search results with highlighting info
 */
window.searchComponents = function(query, maxResults = 8) {
    if (!fuseInstance || !query || query.trim().length < 1) {
        return [];
    }
    
    const results = fuseInstance.search(query.trim(), { limit: maxResults });
    
    return results.map(result => ({
        name: result.item.name,
        category: result.item.category,
        route: result.item.route,
        description: result.item.description,
        score: result.score,
        matches: result.matches || []
    }));
};

/**
 * Highlight matching text in a string
 * @param {string} text - Original text
 * @param {Array} indices - Array of [start, end] index pairs
 * @returns {string} HTML string with <mark> tags around matches
 */
window.highlightMatches = function(text, indices) {
    if (!indices || indices.length === 0) {
        return escapeHtml(text);
    }
    
    // Sort indices by start position
    const sortedIndices = [...indices].sort((a, b) => a[0] - b[0]);
    
    let result = '';
    let lastIndex = 0;
    
    for (const [start, end] of sortedIndices) {
        result += escapeHtml(text.substring(lastIndex, start));
        result += '<mark>' + escapeHtml(text.substring(start, end + 1)) + '</mark>';
        lastIndex = end + 1;
    }
    
    result += escapeHtml(text.substring(lastIndex));
    return result;
};

/**
 * Escape HTML special characters
 * @param {string} text - Text to escape
 * @returns {string} Escaped text
 */
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

/**
 * Get component data for initialization
 * @returns {Array} Component data array
 */
window.getComponentData = function() {
    return componentData;
};
