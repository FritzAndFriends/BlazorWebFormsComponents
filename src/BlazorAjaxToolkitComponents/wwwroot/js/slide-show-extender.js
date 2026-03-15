// SlideShowExtender JS behavior module
// Turns an Image element into an automatic slideshow that cycles through images.

const behaviors = new Map();

/**
 * Creates a slideshow behavior and attaches it to the target image element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[SlideShowExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const {
        slideShowServicePath,
        slideShowServiceMethod,
        contextKey,
        useContextKey,
        imageDescriptionLabelId,
        nextButtonId,
        previousButtonId,
        playButtonId,
        playButtonText,
        stopButtonText,
        playInterval,
        loop,
        autoPlay,
        imageTitleLabelId,
        slides: initialSlides
    } = properties;

    // State
    let slides = [];
    let currentIndex = 0;
    let isPlaying = false;
    let playTimerId = null;

    // Element references
    const descriptionLabel = imageDescriptionLabelId ? document.getElementById(imageDescriptionLabelId) : null;
    const titleLabel = imageTitleLabelId ? document.getElementById(imageTitleLabelId) : null;
    const nextButton = nextButtonId ? document.getElementById(nextButtonId) : null;
    const previousButton = previousButtonId ? document.getElementById(previousButtonId) : null;
    const playButton = playButtonId ? document.getElementById(playButtonId) : null;

    /**
     * Loads slides from data-* attributes on the target element
     */
    function loadSlidesFromDataAttributes() {
        const dataSlides = target.getAttribute('data-slides');
        if (dataSlides) {
            try {
                return JSON.parse(dataSlides);
            } catch (e) {
                console.warn('[SlideShowExtender] Failed to parse data-slides attribute:', e);
            }
        }
        return [];
    }

    /**
     * Initializes the slideshow with provided or discovered slides
     */
    function initializeSlides() {
        // Priority: 1) Slides parameter, 2) data-slides attribute
        if (initialSlides && Array.isArray(initialSlides) && initialSlides.length > 0) {
            slides = initialSlides.map(s => ({
                imageUrl: s.imageUrl || s.ImageUrl || '',
                title: s.title || s.Title || '',
                description: s.description || s.Description || ''
            }));
        } else {
            slides = loadSlidesFromDataAttributes();
        }

        if (slides.length === 0) {
            // Create a single slide from the current image if no slides provided
            const currentSrc = target.src || target.getAttribute('src') || '';
            const currentAlt = target.alt || target.getAttribute('alt') || '';
            if (currentSrc) {
                slides = [{
                    imageUrl: currentSrc,
                    title: currentAlt,
                    description: ''
                }];
            }
        }
    }

    /**
     * Displays the slide at the given index
     */
    function showSlide(index) {
        if (slides.length === 0) return;

        // Handle bounds
        if (index < 0) {
            index = loop ? slides.length - 1 : 0;
        } else if (index >= slides.length) {
            index = loop ? 0 : slides.length - 1;
        }

        currentIndex = index;
        const slide = slides[currentIndex];

        // Update image
        target.src = slide.imageUrl;
        target.alt = slide.title || slide.description || '';

        // Update title label
        if (titleLabel) {
            titleLabel.textContent = slide.title || '';
        }

        // Update description label
        if (descriptionLabel) {
            descriptionLabel.textContent = slide.description || '';
        }

        updateButtonStates();
    }

    /**
     * Advances to the next slide
     */
    function nextSlide() {
        const nextIndex = currentIndex + 1;
        if (!loop && nextIndex >= slides.length) {
            // At end, not looping - stop playing
            if (isPlaying) {
                stop();
            }
            return;
        }
        showSlide(nextIndex);
    }

    /**
     * Goes to the previous slide
     */
    function previousSlide() {
        showSlide(currentIndex - 1);
    }

    /**
     * Starts automatic slideshow playback
     */
    function play() {
        if (isPlaying || slides.length <= 1) return;

        isPlaying = true;
        updatePlayButtonText();

        playTimerId = setInterval(() => {
            nextSlide();
        }, playInterval);
    }

    /**
     * Stops automatic slideshow playback
     */
    function stop() {
        if (!isPlaying) return;

        isPlaying = false;
        updatePlayButtonText();

        if (playTimerId) {
            clearInterval(playTimerId);
            playTimerId = null;
        }
    }

    /**
     * Toggles play/pause state
     */
    function togglePlayPause() {
        if (isPlaying) {
            stop();
        } else {
            play();
        }
    }

    /**
     * Updates the play button text based on current state
     */
    function updatePlayButtonText() {
        if (playButton) {
            playButton.textContent = isPlaying ? stopButtonText : playButtonText;
        }
    }

    /**
     * Updates navigation button enabled/disabled states
     */
    function updateButtonStates() {
        if (!loop) {
            if (previousButton) {
                previousButton.disabled = currentIndex === 0;
            }
            if (nextButton) {
                nextButton.disabled = currentIndex === slides.length - 1;
            }
        }
    }

    /**
     * Sets up event listeners for navigation controls
     */
    function setupEventListeners() {
        if (nextButton) {
            nextButton.addEventListener('click', handleNextClick);
        }
        if (previousButton) {
            previousButton.addEventListener('click', handlePreviousClick);
        }
        if (playButton) {
            playButton.addEventListener('click', handlePlayClick);
        }
    }

    /**
     * Removes event listeners
     */
    function removeEventListeners() {
        if (nextButton) {
            nextButton.removeEventListener('click', handleNextClick);
        }
        if (previousButton) {
            previousButton.removeEventListener('click', handlePreviousClick);
        }
        if (playButton) {
            playButton.removeEventListener('click', handlePlayClick);
        }
    }

    // Event handlers
    function handleNextClick(e) {
        e.preventDefault();
        nextSlide();
    }

    function handlePreviousClick(e) {
        e.preventDefault();
        previousSlide();
    }

    function handlePlayClick(e) {
        e.preventDefault();
        togglePlayPause();
    }

    // Initialize
    initializeSlides();
    setupEventListeners();
    updatePlayButtonText();
    
    // Show first slide
    if (slides.length > 0) {
        showSlide(0);
    }

    // Auto-play if configured
    if (autoPlay && slides.length > 1) {
        play();
    }

    // Expose methods on the target element for external control
    target._slideShow = {
        play,
        stop,
        next: nextSlide,
        previous: previousSlide,
        goTo: showSlide,
        isPlaying: () => isPlaying,
        getCurrentIndex: () => currentIndex,
        getSlideCount: () => slides.length
    };

    const state = {
        targetId,
        target,
        slides,
        currentIndex: () => currentIndex,
        isPlaying: () => isPlaying,
        playTimerId: () => playTimerId,
        nextButton,
        previousButton,
        playButton,
        descriptionLabel,
        titleLabel,
        play,
        stop,
        nextSlide,
        previousSlide,
        showSlide,
        removeEventListeners,
        properties: { ...properties }
    };

    behaviors.set(behaviorId, state);
    return {};
}

/**
 * Updates behavior properties.
 * @param {string} behaviorId
 * @param {object} properties
 */
export function updateBehavior(behaviorId, properties) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    // Update stored properties
    state.properties = { ...properties };

    // If slides changed, reinitialize
    if (properties.slides && Array.isArray(properties.slides)) {
        state.slides = properties.slides.map(s => ({
            imageUrl: s.imageUrl || s.ImageUrl || '',
            title: s.title || s.Title || '',
            description: s.description || s.Description || ''
        }));
        
        // Reset to first slide
        state.showSlide(0);
    }
}

/**
 * Disposes the behavior and cleans up resources.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    // Stop playback
    state.stop();

    // Remove event listeners
    state.removeEventListeners();

    // Remove exposed methods from target element
    if (state.target && state.target._slideShow) {
        delete state.target._slideShow;
    }

    behaviors.delete(behaviorId);
}

/**
 * Programmatically starts slideshow playback.
 * @param {string} targetId - The ID of the target element
 */
export function play(targetId) {
    const target = document.getElementById(targetId);
    if (target && target._slideShow) {
        target._slideShow.play();
    }
}

/**
 * Programmatically stops slideshow playback.
 * @param {string} targetId - The ID of the target element
 */
export function stop(targetId) {
    const target = document.getElementById(targetId);
    if (target && target._slideShow) {
        target._slideShow.stop();
    }
}

/**
 * Navigates to the next slide.
 * @param {string} targetId - The ID of the target element
 */
export function next(targetId) {
    const target = document.getElementById(targetId);
    if (target && target._slideShow) {
        target._slideShow.next();
    }
}

/**
 * Navigates to the previous slide.
 * @param {string} targetId - The ID of the target element
 */
export function previous(targetId) {
    const target = document.getElementById(targetId);
    if (target && target._slideShow) {
        target._slideShow.previous();
    }
}

/**
 * Navigates to a specific slide by index.
 * @param {string} targetId - The ID of the target element
 * @param {number} index - The zero-based slide index
 */
export function goToSlide(targetId, index) {
    const target = document.getElementById(targetId);
    if (target && target._slideShow) {
        target._slideShow.goTo(index);
    }
}
