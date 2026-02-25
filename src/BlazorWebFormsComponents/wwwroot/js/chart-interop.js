// Chart.js interop module for BlazorWebFormsComponents
// Provides createChart, updateChart, and destroyChart functions.

const chartInstances = {};

/**
 * Loads Chart.js if not already loaded.
 * @returns {Promise<void>}
 */
async function ensureChartJs() {
	if (window.Chart) return;

	return new Promise((resolve, reject) => {
		const script = document.createElement('script');
		script.src = '_content/Fritz.BlazorWebFormsComponents/js/chart.min.js';
		script.onload = () => resolve();
		script.onerror = () => reject(new Error('Failed to load Chart.js'));
		document.head.appendChild(script);
	});
}

/**
 * Creates a new Chart.js instance on the given canvas element.
 * @param {string} canvasId - The ID of the canvas element.
 * @param {object} config - Chart.js configuration object.
 */
export async function createChart(canvasId, config) {
	await ensureChartJs();

	// Destroy existing instance if present
	if (chartInstances[canvasId]) {
		chartInstances[canvasId].destroy();
	}

	const canvas = document.getElementById(canvasId);
	if (!canvas) {
		throw new Error(`Canvas element with id '${canvasId}' not found.`);
	}

	const ctx = canvas.getContext('2d');
	chartInstances[canvasId] = new Chart(ctx, config);
}

/**
 * Updates an existing Chart.js instance with new configuration.
 * @param {string} canvasId - The ID of the canvas element.
 * @param {object} config - New Chart.js configuration object.
 */
export async function updateChart(canvasId, config) {
	await ensureChartJs();

	const chart = chartInstances[canvasId];
	if (!chart) {
		// If no chart exists yet, create one
		await createChart(canvasId, config);
		return;
	}

	chart.data = config.data || {};
	chart.options = config.options || {};

	if (config.type && chart.config) {
		chart.config.type = config.type;
	}

	chart.update();
}

/**
 * Destroys a Chart.js instance and removes it from tracking.
 * @param {string} canvasId - The ID of the canvas element.
 */
export function destroyChart(canvasId) {
	const chart = chartInstances[canvasId];
	if (chart) {
		chart.destroy();
		delete chartInstances[canvasId];
	}
}
