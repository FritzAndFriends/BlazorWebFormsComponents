// PasswordStrength JS behavior module
// Displays a visual indicator of password strength as the user types.

const behaviors = new Map();

// Default strength descriptions
const DEFAULT_DESCRIPTIONS = ["Very Poor", "Weak", "Average", "Strong", "Excellent"];

/**
 * Creates a password strength behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[PasswordStrength] Target element '${targetId}' not found.`);
        return {};
    }

    // Parse properties
    const props = parseProperties(properties);
    
    // Create the indicator element
    const indicator = createIndicatorElement(props);
    
    // Position and insert the indicator
    positionIndicator(target, indicator, props.displayPosition);

    /**
     * Input handler: evaluate password strength on each keystroke.
     */
    function inputHandler() {
        const password = target.value;
        const strength = calculateStrength(password, props);
        updateIndicator(indicator, strength, props);
    }

    // Attach event listener
    target.addEventListener("input", inputHandler);

    // Initialize with current value
    inputHandler();

    const state = {
        targetId,
        properties: props,
        indicator,
        inputHandler
    };

    behaviors.set(behaviorId, state);
    return {};
}

/**
 * Parses and normalizes the properties from the Blazor component.
 */
function parseProperties(properties) {
    const descriptions = properties.textStrengthDescriptions
        ? properties.textStrengthDescriptions.split(";").map(s => s.trim())
        : DEFAULT_DESCRIPTIONS;

    const styles = properties.strengthStyles
        ? properties.strengthStyles.split(";").map(s => s.trim())
        : [];

    // Parse calculation weightings if provided (format: "length;numeric;symbol;upper;lower;mixed")
    let weightings = { length: 50, numeric: 15, symbol: 15, upper: 10, lower: 5, mixed: 5 };
    if (properties.calculationWeightings) {
        const parts = properties.calculationWeightings.split(";").map(s => parseFloat(s.trim()) || 0);
        if (parts.length >= 6) {
            weightings = {
                length: parts[0],
                numeric: parts[1],
                symbol: parts[2],
                upper: parts[3],
                lower: parts[4],
                mixed: parts[5]
            };
        }
    }

    return {
        displayPosition: properties.displayPosition || "RightSide",
        strengthIndicatorType: properties.strengthIndicatorType || "Text",
        preferredPasswordLength: properties.preferredPasswordLength || 10,
        minimumNumericCharacters: properties.minimumNumericCharacters || 0,
        minimumSymbolCharacters: properties.minimumSymbolCharacters || 0,
        minimumUpperCaseCharacters: properties.minimumUpperCaseCharacters || 0,
        minimumLowerCaseCharacters: properties.minimumLowerCaseCharacters || 0,
        requiresUpperAndLowerCaseCharacters: properties.requiresUpperAndLowerCaseCharacters || false,
        descriptions,
        styles,
        barBorderCssClass: properties.barBorderCssClass || "",
        weightings,
        textCssClass: properties.textCssClass || "",
        helpHandleCssClass: properties.helpHandleCssClass || "",
        helpHandlePosition: properties.helpHandlePosition || "RightSide"
    };
}

/**
 * Creates the indicator DOM element based on the indicator type.
 */
function createIndicatorElement(props) {
    const container = document.createElement("span");
    container.className = "password-strength-indicator";
    container.style.display = "inline-block";
    container.style.verticalAlign = "middle";

    if (props.strengthIndicatorType === "BarIndicator") {
        // Create bar indicator
        const barContainer = document.createElement("div");
        barContainer.className = props.barBorderCssClass || "password-strength-bar-border";
        barContainer.style.display = "inline-block";
        barContainer.style.width = "100px";
        barContainer.style.height = "10px";
        barContainer.style.border = "1px solid #ccc";
        barContainer.style.backgroundColor = "#f0f0f0";
        barContainer.style.position = "relative";
        barContainer.style.overflow = "hidden";

        const bar = document.createElement("div");
        bar.className = "password-strength-bar";
        bar.style.height = "100%";
        bar.style.width = "0%";
        bar.style.transition = "width 0.3s ease, background-color 0.3s ease";

        barContainer.appendChild(bar);
        container.appendChild(barContainer);
        container.barElement = bar;
    } else {
        // Create text indicator
        const textSpan = document.createElement("span");
        textSpan.className = props.textCssClass || "password-strength-text";
        container.appendChild(textSpan);
        container.textElement = textSpan;
    }

    return container;
}

/**
 * Positions and inserts the indicator relative to the target element.
 */
function positionIndicator(target, indicator, position) {
    indicator.style.marginLeft = "5px";
    indicator.style.marginRight = "5px";

    switch (position) {
        case "LeftSide":
            target.parentNode.insertBefore(indicator, target);
            break;
        case "AboveRight":
            indicator.style.display = "block";
            indicator.style.textAlign = "right";
            target.parentNode.insertBefore(indicator, target);
            break;
        case "AboveLeft":
            indicator.style.display = "block";
            indicator.style.textAlign = "left";
            target.parentNode.insertBefore(indicator, target);
            break;
        case "BelowRight":
            indicator.style.display = "block";
            indicator.style.textAlign = "right";
            if (target.nextSibling) {
                target.parentNode.insertBefore(indicator, target.nextSibling);
            } else {
                target.parentNode.appendChild(indicator);
            }
            break;
        case "BelowLeft":
            indicator.style.display = "block";
            indicator.style.textAlign = "left";
            if (target.nextSibling) {
                target.parentNode.insertBefore(indicator, target.nextSibling);
            } else {
                target.parentNode.appendChild(indicator);
            }
            break;
        case "RightSide":
        default:
            if (target.nextSibling) {
                target.parentNode.insertBefore(indicator, target.nextSibling);
            } else {
                target.parentNode.appendChild(indicator);
            }
            break;
    }
}

/**
 * Calculates password strength based on configurable rules.
 * Returns an object with score (0-100) and level (0-4).
 */
function calculateStrength(password, props) {
    if (!password || password.length === 0) {
        return { score: 0, level: 0, meetsRequirements: false };
    }

    const { weightings } = props;
    let score = 0;

    // Count character types
    const numericCount = (password.match(/[0-9]/g) || []).length;
    const symbolCount = (password.match(/[^a-zA-Z0-9]/g) || []).length;
    const upperCount = (password.match(/[A-Z]/g) || []).length;
    const lowerCount = (password.match(/[a-z]/g) || []).length;

    // Length score (proportional to preferred length)
    const lengthRatio = Math.min(password.length / props.preferredPasswordLength, 1);
    score += lengthRatio * weightings.length;

    // Numeric characters score
    if (props.minimumNumericCharacters > 0) {
        const numericRatio = Math.min(numericCount / props.minimumNumericCharacters, 1);
        score += numericRatio * weightings.numeric;
    } else if (numericCount > 0) {
        score += weightings.numeric;
    }

    // Symbol characters score
    if (props.minimumSymbolCharacters > 0) {
        const symbolRatio = Math.min(symbolCount / props.minimumSymbolCharacters, 1);
        score += symbolRatio * weightings.symbol;
    } else if (symbolCount > 0) {
        score += weightings.symbol;
    }

    // Uppercase characters score
    if (props.minimumUpperCaseCharacters > 0) {
        const upperRatio = Math.min(upperCount / props.minimumUpperCaseCharacters, 1);
        score += upperRatio * weightings.upper;
    } else if (upperCount > 0) {
        score += weightings.upper;
    }

    // Lowercase characters score
    if (props.minimumLowerCaseCharacters > 0) {
        const lowerRatio = Math.min(lowerCount / props.minimumLowerCaseCharacters, 1);
        score += lowerRatio * weightings.lower;
    } else if (lowerCount > 0) {
        score += weightings.lower;
    }

    // Mixed case bonus
    if (props.requiresUpperAndLowerCaseCharacters) {
        if (upperCount > 0 && lowerCount > 0) {
            score += weightings.mixed;
        }
    } else if (upperCount > 0 && lowerCount > 0) {
        score += weightings.mixed;
    }

    // Check if all requirements are met
    const meetsRequirements = 
        password.length >= props.preferredPasswordLength &&
        numericCount >= props.minimumNumericCharacters &&
        symbolCount >= props.minimumSymbolCharacters &&
        upperCount >= props.minimumUpperCaseCharacters &&
        lowerCount >= props.minimumLowerCaseCharacters &&
        (!props.requiresUpperAndLowerCaseCharacters || (upperCount > 0 && lowerCount > 0));

    // Cap score at 100
    score = Math.min(score, 100);

    // Calculate level (0-4) based on score
    let level;
    if (score < 20) {
        level = 0; // Very Poor
    } else if (score < 40) {
        level = 1; // Weak
    } else if (score < 60) {
        level = 2; // Average
    } else if (score < 80) {
        level = 3; // Strong
    } else {
        level = 4; // Excellent
    }

    return { score, level, meetsRequirements };
}

/**
 * Updates the indicator display based on calculated strength.
 */
function updateIndicator(indicator, strength, props) {
    const { descriptions, styles } = props;
    const level = Math.min(strength.level, descriptions.length - 1);
    const description = descriptions[level] || "";
    const style = styles[level] || "";

    // Color mapping for visual feedback
    const colors = ["#ff4444", "#ff8800", "#ffcc00", "#88cc00", "#00cc00"];
    const color = colors[level] || "#ccc";

    if (props.strengthIndicatorType === "BarIndicator" && indicator.barElement) {
        const bar = indicator.barElement;
        bar.style.width = `${strength.score}%`;
        bar.style.backgroundColor = color;
        
        // Apply custom style class if provided
        if (style) {
            bar.className = "password-strength-bar " + style;
        }
    } else if (indicator.textElement) {
        const textSpan = indicator.textElement;
        textSpan.textContent = description;
        textSpan.style.color = color;
        
        // Apply custom style class if provided
        if (style) {
            textSpan.className = (props.textCssClass || "password-strength-text") + " " + style;
        }
    }
}

/**
 * Updates behavior properties.
 * @param {string} behaviorId
 * @param {object} properties
 */
export function updateBehavior(behaviorId, properties) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const target = document.getElementById(state.targetId);
    if (!target) return;

    // Parse new properties
    const props = parseProperties(properties);
    state.properties = props;

    // Update indicator with current password value
    const password = target.value;
    const strength = calculateStrength(password, props);
    updateIndicator(state.indicator, strength, props);
}

/**
 * Disposes the behavior and removes event listeners.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const target = document.getElementById(state.targetId);
    if (target) {
        target.removeEventListener("input", state.inputHandler);
    }

    // Remove indicator element
    if (state.indicator && state.indicator.parentNode) {
        state.indicator.parentNode.removeChild(state.indicator);
    }

    behaviors.delete(behaviorId);
}
