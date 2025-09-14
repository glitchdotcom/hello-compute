<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Multi-Step DIY Poppiesss Builder</title>
    <link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@400;600;700&family=Open+Sans:wght@400;600&display=swap" rel="stylesheet">
    <style>
        :root {
            /* Main Theme Colors */
            --primary-accent: #FF6F61; /* Vibrant Coral Red - for highlights, selected states */
            --secondary-accent: #6B5B95; /* Deep Periwinkle - for action buttons */
            --tertiary-accent: #88B04B; /* Vibrant Green - for action buttons */
            --quaternary-accent: #FFD700; /* Sunny Yellow - for a cheerful touch */

            /* Backgrounds & Text */
            --page-bg: #FFF8E1; /* Light Creamy Yellow */
            --text-dark: #36454F; /* Charcoal Gray */
            --container-bg: #FFFFFF;
            --border-light: #EEEEEE; /* Very light gray for subtle lines */

            /* Step-specific Option Colors */
            --cake-base-option-bg: #A7DBD8; /* Soft Teal */
            --frosting-option-bg: #E0BBE4; /* Light Lavender */
            --coating-option-bg: #F7EAAD; /* Pale Gold */
            --toppings-option-bg: #FFC3A0; /* Light Peach */

            /* Navigation & Progress */
            --nav-button-bg: var(--primary-accent);
            --nav-button-hover: #E05E55; /* Darker coral */
            --disabled-nav-button: #CCCCCC;
            --progress-bar-fill: var(--primary-accent);

            /* Recipe Output */
            --recipe-bg: #FDFCEF; /* Slightly lighter cream for recipe box */
        }

        body {
            font-family: 'Open Sans', sans-serif;
            background-color: var(--page-bg);
            color: var(--text-dark);
            margin: 0;
            padding: 20px;
            text-align: center;
            line-height: 1.6;
            transition: background-color 0.5s ease;
        }
        h1, h2, h3, h4 {
            font-family: 'Montserrat', sans-serif;
            color: var(--primary-accent);
            margin-bottom: 1rem;
        }
        h1 {
            font-size: 2.8em;
            margin-top: 20px;
            margin-bottom: 15px;
        }
        h2 {
            font-size: 2.2em;
            text-align: center;
            margin-bottom: 35px;
            outline: none; /* Remove default outline for focused headings, handled by JS focus */
        }
        p {
            margin-bottom: 1rem;
        }

        #builder-container {
            background-color: var(--container-bg);
            border-radius: 15px;
            box-shadow: 0 12px 35px rgba(0, 0, 0, 0.12);
            padding: 40px;
            margin: 30px auto;
            max-width: 950px;
            text-align: left;
            position: relative;
        }
        .step-page {
            display: none;
            padding-top: 15px;
        }
        .step-page.active {
            display: block;
        }

        .options {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            justify-content: center;
            margin-bottom: 35px;
        }
        .options button {
            color: var(--text-dark);
            border: none;
            border-radius: 10px;
            padding: 18px 25px;
            cursor: pointer;
            font-size: 1.15em;
            font-weight: 600;
            transition: all 0.25s ease-in-out;
            width: 200px;
            min-height: 75px;
            display: flex;
            align-items: center;
            justify-content: center;
            text-align: center;
            box-sizing: border-box;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            outline: 2px solid transparent; /* default outline for focus */
            outline-offset: 2px;
        }
        .options button:focus { /* Visible focus indicator */
            outline: 2px solid var(--primary-accent);
        }

        /* Unique background colors for each step's options */
        #cake-base-options button { background-color: var(--cake-base-option-bg); }
        #frosting-options button { background-color: var(--frosting-option-bg); }
        #coating-options button { background-color: var(--coating-option-bg); }
        #topping-options button { background-color: var(--toppings-option-bg); }

        .options button:hover {
            background-color: var(--primary-accent);
            color: white;
            transform: translateY(-7px);
            box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
        }
        .options button.selected {
            background-color: var(--primary-accent);
            color: white;
            transform: scale(1.05);
            box-shadow: 0 8px 18px rgba(0, 0, 0, 0.25);
            border: 2px solid white;
            outline: 2px solid white; /* Ensures focus outline is still visible on selected */
        }

        .navigation-buttons {
            display: flex;
            justify-content: space-between;
            margin-top: 45px;
            padding-top: 25px;
            border-top: 1px solid var(--border-light);
        }
        .navigation-buttons button {
            background-color: var(--nav-button-bg);
            color: white;
            padding: 15px 32px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 1.15em;
            font-weight: 600;
            transition: background-color 0.2s ease, transform 0.1s ease, box-shadow 0.2s ease;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            outline: 2px solid transparent;
            outline-offset: 2px;
        }
        .navigation-buttons button:focus {
            outline: 2px solid var(--secondary-accent); /* Distinct focus for nav buttons */
        }
        .navigation-buttons button#resetBtn {
             background-color: var(--secondary-accent);
        }
        .navigation-buttons button:hover {
            background-color: var(--nav-button-hover);
            transform: translateY(-3px);
            box-shadow: 0 6px 15px rgba(0, 0, 0, 0.2);
        }
        .navigation-buttons button#resetBtn:hover {
            background-color: #5a4b80;
        }
        .navigation-buttons button:disabled {
            background-color: var(--disabled-nav-button);
            color: #777;
            cursor: not-allowed;
            transform: none;
            box-shadow: none;
            outline: none; /* Disabled elements are not focusable */
        }

        #recipe-output {
            background-color: var(--recipe-bg);
            border: 3px solid var(--primary-accent);
            border-radius: 12px;
            padding: 35px;
            margin-top: 35px;
            text-align: left;
            box-shadow: inset 0 3px 8px rgba(0, 0, 0, 0.08);
        }
        #recipe-output h3 {
            color: var(--primary-accent);
            margin-bottom: 25px;
            text-align: center;
            font-size: 2em;
        }
        .step-instruction {
            margin-bottom: 15px;
            font-size: 1.1em;
        }
        .step-instruction strong {
            color: var(--text-dark);
        }
        .nutrition-info {
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px dashed var(--primary-accent);
            font-size: 1em;
        }
        .nutrition-info h4 {
            color: var(--primary-accent);
            margin-bottom: 15px;
            font-size: 1.3em;
        }
        .final-action-buttons {
            display: flex;
            justify-content: center;
            gap: 25px;
            margin-top: 40px;
        }
        .final-action-buttons button {
            color: white;
            padding: 15px 30px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 1.1em;
            font-weight: 600;
            transition: background-color 0.2s ease, transform 0.1s ease, box-shadow 0.2s ease;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            outline: 2px solid transparent;
            outline-offset: 2px;
        }
        .final-action-buttons button:focus {
            outline: 2px solid var(--primary-accent);
        }
        .final-action-buttons button.order-btn {
            background-color: var(--primary-accent);
        }
        .final-action-buttons button.save-btn {
            background-color: var(--secondary-accent);
        }
        .final-action-buttons button.print-btn {
            background-color: var(--tertiary-accent);
        }
        .final-action-buttons button.order-btn:hover { background-color: var(--nav-button-hover); }
        .final-action-buttons button.save-btn:hover { background-color: #5a4b80; }
        .final-action-buttons button.print-btn:hover { background-color: #72963a; }

        .final-action-buttons button:hover {
            transform: translateY(-3px);
            box-shadow: 0 6px 15px rgba(0, 0, 0, 0.2);
        }

        /* Progress Bar */
        #progress-bar-container {
            width: 100%;
            background-color: var(--border-light);
            border-radius: 6px;
            margin-bottom: 25px;
            height: 14px;
            overflow: hidden;
            box-shadow: inset 0 2px 5px rgba(0, 0, 0, 0.08);
        }
        #progress-bar {
            height: 100%;
            width: 0%;
            background-color: var(--progress-bar-fill);
            border-radius: 6px;
            transition: width 0.5s cubic-bezier(0.23, 1, 0.32, 1);
            box-shadow: 0 0 10px rgba(255, 111, 97, 0.6);
        }
        #progress-text {
            text-align: right;
            font-size: 1em;
            color: var(--text-dark);
            margin-top: 12px;
            margin-bottom: 25px;
            font-weight: 600;
            /* Added for accessibility: */
            position: relative; /* Needed for screen reader visibility */
        }

        /* Order Confirmation specific styles */
        .message-container {
            background-color: #fff;
            border-radius: 15px;
            box-shadow: 0 12px 35px rgba(0, 0, 0, 0.12);
            padding: 45px;
            max-width: 650px;
            margin: 40px auto 0 auto;
        }
        .message-container h1 {
            color: var(--primary-accent);
            font-family: 'Montserrat', sans-serif;
            font-size: 2.8em;
            margin-bottom: 25px;
        }
        .message-container p {
            font-size: 1.4em;
            line-height: 1.7;
            margin-bottom: 35px;
            color: var(--text-dark);
        }
        .home-button {
            background-color: var(--secondary-accent);
            color: white;
            padding: 16px 35px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 1.2em;
            font-weight: 700;
            text-decoration: none;
            transition: background-color 0.2s ease, transform 0.1s ease, box-shadow 0.2s ease;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.15);
            outline: 2px solid transparent;
            outline-offset: 2px;
        }
        .home-button:focus {
            outline: 2px solid var(--primary-accent);
        }
        .home-button:hover {
            background-color: #5a4b80;
            transform: translateY(-3px);
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.2);
        }

        /* Print-specific styles */
        @media print {
            body { background-color: #fff; color: #000; margin: 0; padding: 0; }
            #builder-container, .navigation-buttons, .final-action-buttons, #progress-bar-container, #progress-text, h1, p:first-of-type { display: none; }
            #recipe-output, #step-6 {
                border: none;
                box-shadow: none;
                margin: 0;
                padding: 20px;
                width: 100%;
                box-sizing: border-box;
                display: block !important;
                font-size: 1.1em;
            }
            #recipe-output h3 { color: #000; font-size: 1.6em; }
            .step-instruction strong { color: #000; }
            .nutrition-info { border-top: 1px dashed #777; }
            .nutrition-info h4 { color: #000; }

            #step-6 { display: none !important; }
        }
    </style>
</head>
<body>
    <header> <!-- Added header for semantic grouping -->
        <h1>Craft Your Perfect Poppiesss!</h1>
        <p>Dive in and select your favorite ingredients to create a unique and delicious Poppiesss recipe tailored just for you.</p>
    </header>

    <main id="builder-container" aria-live="polite"> <!-- Main content area, aria-live for dynamic updates -->
        <h2 class="visually-hidden">Poppiesss Builder Progress</h2> <!-- Hidden heading for screen readers -->
        <div id="progress-bar-container" role="progressbar" aria-valuenow="0" aria-valuemin="1" aria-valuemax="6" aria-labelledby="progress-text-label">
            <div id="progress-bar"></div>
        </div>
        <div id="progress-text"><span id="progress-text-label">Progress:</span> Step 1 of 6</div> <!-- Linked progress text -->

        <!-- Step 1: Cake Base -->
        <section class="step-page active" id="step-1" data-selection-key="cakeBase" aria-live="polite" aria-atomic="true">
            <h2 id="step1-heading" tabindex="-1">1. Choose Your Poppiesss Base</h2> <!-- tabindex makes heading focusable -->
            <div class="options" role="radiogroup" aria-labelledby="step1-heading" id="cake-base-options">
                <button data-value="Vanilla Cake" role="radio" aria-checked="false">Vanilla Cake</button>
                <button data-value="Chocolate Cake" role="radio" aria-checked="false">Chocolate Cake</button>
                <button data-value="Red Velvet Cake" role="radio" aria-checked="false">Red Velvet Cake</button>
                <button data-value="Lemon Cake" role="radio" aria-checked="false">Lemon Cake</button>
            </div>
        </section>

        <!-- Step 2: Frosting -->
        <section class="step-page" id="step-2" data-selection-key="frosting" aria-hidden="true" aria-live="polite" aria-atomic="true">
            <h2 id="step2-heading" tabindex="-1">2. Select Your Frosting/Binder</h2>
            <div class="options" role="radiogroup" aria-labelledby="step2-heading" id="frosting-options">
                <button data-value="Cream Cheese Frosting" role="radio" aria-checked="false">Cream Cheese Frosting</button>
                <button data-value="Chocolate Ganache" role="radio" aria-checked="false">Chocolate Ganache</button>
                <button data-value="Vanilla Buttercream" role="radio" aria-checked="false">Vanilla Buttercream</button>
                <button data-value="Strawberry Frosting" role="radio" aria-checked="false">Strawberry Frosting</button>
            </div>
        </section>

        <!-- Step 3: Dipping Coating -->
        <section class="step-page" id="step-3" data-selection-key="coating" aria-hidden="true" aria-live="polite" aria-atomic="true">
            <h2 id="step3-heading" tabindex="-1">3. Pick Your Dipping Coating</h2>
            <div class="options" role="radiogroup" aria-labelledby="step3-heading" id="coating-options">
                <button data-value="White Chocolate" role="radio" aria-checked="false">White Chocolate</button>
                <button data-value="Milk Chocolate" role="radio" aria-checked="false">Milk Chocolate</button>
                <button data-value="Dark Chocolate" role="radio" aria-checked="false">Dark Chocolate</button>
                <button data-value="Pink Candy Melts" role="radio" aria-checked="false">Pink Candy Melts</button>
                <button data-value="Blue Candy Melts" role="radio" aria-checked="false">Blue Candy Melts</button>
            </div>
        </section>

        <!-- Step 4: Toppings -->
        <section class="step-page" id="step-4" data-selection-key="toppings" aria-hidden="true" aria-live="polite" aria-atomic="true">
            <h2 id="step4-heading" tabindex="-1">4. Add Your Toppings</h2>
            <div class="options" role="radiogroup" aria-labelledby="step4-heading" id="topping-options">
                <button data-value="Rainbow Sprinkles" role="radio" aria-checked="false">Rainbow Sprinkles</button>
                <button data-value="Chopped Nuts" role="radio" aria-checked="false">Chopped Nuts</button>
                <button data-value="Chocolate Drizzle" role="radio" aria-checked="false">Chocolate Drizzle</button>
                <button data-value="Crushed Oreos" role="radio" aria-checked="false">Crushed Oreos</button>
                <button data-value="No Toppings" role="radio" aria-checked="false">No Toppings</button>
            </div>
        </section>

        <!-- Step 5: Recipe Output -->
        <section class="step-page" id="step-5" aria-hidden="true" aria-live="polite" aria-atomic="true">
            <h2 id="step5-heading" tabindex="-1">Your Custom Poppiesss Recipe!</h2>
            <div id="recipe-output">
                <h3>Your Custom Poppiesss Recipe:</h3>
                <p class="step-instruction"><strong>Poppiesss Base:</strong> <span id="output-cake-base"></span></p>
                <p class="step-instruction"><strong>Frosting/Binder:</strong> <span id="output-frosting"></span></p>
                <p class="step-instruction"><strong>Dipping Coating:</strong> <span id="output-coating"></span></p>
                <p class="step-instruction"><strong>Toppings:</strong> <span id="output-toppings"></span></p>
                <p class="step-instruction"><strong>General Instructions:</strong> Crumble the Poppiesss base and mix with the frosting until a dough-like consistency is achieved. Form into balls and chill thoroughly. Melt your chosen coating, dip the chilled Poppiesss balls, and then decorate with your selected toppings while the coating is wet. Let the coating set before enjoying!</p>

                <div class="nutrition-info">
                    <h4>Estimated Nutrition per Poppiesss:</h4>
                    <p><strong>Total Calories:</strong> <span id="output-calories">0</span> kcal</p>
                    <p><strong>Dietary Notes:</strong> <span id="output-dietary">Vegetarian</span></p>
                    <small>*Nutritional values are approximate estimates and should not be used for actual dietary planning.</small>
                </div>
            </div>
            <div class="final-action-buttons">
                <button class="order-btn" onclick="placeOrder()">Order My Custom Poppiesss!</button>
                <button class="save-btn" onclick="saveRecipe()">Save My Recipe</button>
                <button class="print-btn" onclick="printRecipe()">Print My Recipe</button>
            </div>
        </section>

        <!-- Step 6: Order Confirmation -->
        <section class="step-page" id="step-6" aria-hidden="true" aria-live="polite" aria-atomic="true">
            <h2 id="step6-heading" tabindex="-1">Poppiesss Order Confirmation</h2>
            <div class="message-container">
                <h1>Thank You For Your Poppiesss Order!</h1>
                <p>Ma'am / Sir, please show this to our nearest branch and do payment :)</p>
                <button class="home-button" onclick="resetBuilder()">Build Another Poppiesss!</button>
            </div>
        </section>

        <nav class="navigation-buttons" aria-label="Step navigation"> <!-- Navigation area with aria-label -->
            <button id="prevBtn" onclick="prevStep()" disabled aria-label="Previous Step">Previous</button>
            <button id="resetBtn" onclick="resetBuilder()" aria-label="Reset Builder">Reset</button>
            <button id="nextBtn" onclick="nextStep()" aria-label="Next Step">Next</button>
        </nav>
    </main>

    <script>
        const selections = {
            cakeBase: '',
            frosting: '',
            coating: '',
            toppings: ''
        };

        const ingredientData = {
            "Vanilla Cake": { calories: 200, dietary: ["Vegetarian"] },
            "Chocolate Cake": { calories: 220, dietary: ["Vegetarian"] },
            "Red Velvet Cake": { calories: 230, dietary: ["Vegetarian"] },
            "Lemon Cake": { calories: 190, dietary: ["Vegetarian"] },

            "Cream Cheese Frosting": { calories: 80, dietary: ["Vegetarian"] },
            "Chocolate Ganache": { calories: 100, dietary: ["Vegetarian"] },
            "Vanilla Buttercream": { calories: 90, dietary: ["Vegetarian"] },
            "Strawberry Frosting": { calories: 85, dietary: ["Vegetarian"] },

            "White Chocolate": { calories: 70, dietary: ["Vegetarian"] },
            "Milk Chocolate": { calories: 65, dietary: ["Vegetarian"] },
            "Dark Chocolate": { calories: 60, dietary: ["Vegetarian"] },
            "Pink Candy Melts": { calories: 75, dietary: ["Vegetarian"] },
            "Blue Candy Melts": { calories: 75, dietary: ["Vegetarian"] },

            "Rainbow Sprinkles": { calories: 15, dietary: ["Vegetarian"] },
            "Chopped Nuts": { calories: 25, dietary: ["Vegetarian"] },
            "Chocolate Drizzle": { calories: 20, dietary: ["Vegetarian"] },
            "Crushed Oreos": { calories: 30, dietary: ["Vegetarian"] },
            "No Toppings": { calories: 0, dietary: ["Vegetarian"] }
        };

        let currentStep = 0;
        const totalSteps = 6;
        const stepPages = document.querySelectorAll('.step-page');
        const prevBtn = document.getElementById('prevBtn');
        const nextBtn = document.getElementById('nextBtn');
        const progressBarContainer = document.getElementById('progress-bar-container'); // Get container
        const progressBar = document.getElementById('progress-bar');
        const progressText = document.getElementById('progress-text');

        // Helper to update ARIA attributes for progress bar
        function updateProgressBarARIA() {
            const currentVal = currentStep + 1;
            progressBarContainer.setAttribute('aria-valuenow', currentVal);
            progressText.textContent = `Progress: Step ${currentVal} of ${totalSteps}`;
        }

        function showStep(stepIndex) {
            stepPages.forEach((page, index) => {
                page.classList.remove('active');
                page.setAttribute('aria-hidden', 'true'); // Hide inactive steps from screen readers
            });

            const activePage = stepPages[stepIndex];
            activePage.classList.add('active');
            activePage.setAttribute('aria-hidden', 'false'); // Show active step to screen readers
            currentStep = stepIndex;
            updateNavigationButtons();
            updateProgressBarARIA(); // Call ARIA update here

            // Focus on the heading of the newly active step for screen reader users
            const activeHeading = activePage.querySelector('h2');
            if (activeHeading) {
                activeHeading.focus();
            }

            if (currentStep === totalSteps - 2) { // If it's the final recipe step (step 5)
                generateRecipe();
            }
        }

        function updateNavigationButtons() {
            prevBtn.disabled = currentStep === 0;
            if (currentStep === totalSteps - 1) {
                nextBtn.style.display = 'none';
            } else {
                nextBtn.style.display = 'inline-block';
            }
        }

        function setupSelectionListeners() {
            stepPages.forEach(stepPage => {
                const selectionKey = stepPage.dataset.selectionKey;
                if (selectionKey) {
                    const optionsDiv = stepPage.querySelector('.options');
                    optionsDiv.querySelectorAll('button').forEach(button => {
                        button.addEventListener('click', () => {
                            // Deselect previous radio button and set aria-checked to false
                            const previouslySelected = optionsDiv.querySelector('button.selected');
                            if (previouslySelected) {
                                previouslySelected.classList.remove('selected');
                                previouslySelected.setAttribute('aria-checked', 'false');
                            }
                            // Select current radio button and set aria-checked to true
                            button.classList.add('selected');
                            button.setAttribute('aria-checked', 'true');
                            selections[selectionKey] = button.dataset.value;
                        });
                    });
                }
            });
        }

        function nextStep() {
            // Validation only for selection steps (steps 1-4)
            if (currentStep >= 0 && currentStep <= 3) {
                const currentSelectionKey = stepPages[currentStep].dataset.selectionKey;
                if (currentSelectionKey && !selections[currentSelectionKey]) {
                    alert('Please make a selection before proceeding!');
                    return;
                }
            }

            if (currentStep < totalSteps - 1) {
                showStep(currentStep + 1);
            }
        }

        function prevStep() {
            if (currentStep > 0) {
                showStep(currentStep - 1);
            }
        }

        function resetBuilder() {
            for (const key in selections) {
                selections[key] = '';
            }
            document.querySelectorAll('.options button.selected').forEach(btn => {
                btn.classList.remove('selected');
                btn.setAttribute('aria-checked', 'false'); // Reset ARIA for options
            });
            showStep(0);
            document.getElementById('output-cake-base').textContent = '';
            document.getElementById('output-frosting').textContent = '';
            document.getElementById('output-coating').textContent = '';
            document.getElementById('output-toppings').textContent = '';
            document.getElementById('output-calories').textContent = '0';
            document.getElementById('output-dietary').textContent = 'Vegetarian';
        }

        function generateRecipe() {
            document.getElementById('output-cake-base').textContent = selections.cakeBase || 'Not selected';
            document.getElementById('output-frosting').textContent = selections.frosting || 'Not selected';
            document.getElementById('output-coating').textContent = selections.coating || 'Not selected';
            document.getElementById('output-toppings').textContent = selections.toppings || 'Not selected';

            let totalCalories = 0;
            let dietaryNotes = new Set();

            for (const key in selections) {
                const ingredientName = selections[key];
                if (selections[key] && ingredientData[ingredientName]) {
                    totalCalories += ingredientData[ingredientName].calories;
                    ingredientData[ingredientName].dietary.forEach(note => dietaryNotes.add(note));
                }
            }
            document.getElementById('output-calories').textContent = totalCalories;
            document.getElementById('output-dietary').textContent = dietaryNotes.size > 0 ? Array.from(dietaryNotes).join(', ') : 'N/A';
        }

        function saveRecipe() {
            if (!selections.cakeBase) {
                alert('No recipe to save! Please complete the selections first.');
                return;
            }

            const recipeName = prompt("Enter a name for your Poppiesss recipe (e.g., 'My Favorite Birthday Poppiesss'):");
            if (recipeName) {
                const savedRecipes = JSON.parse(localStorage.getItem('cakePopRecipes')) || {};
                savedRecipes[recipeName] = { ...selections, calories: document.getElementById('output-calories').textContent, dietary: document.getElementById('output-dietary').textContent };
                localStorage.setItem('cakePopRecipes', JSON.stringify(savedRecipes));
                alert(`"${recipeName}" recipe saved successfully! You can find it in your browser's local storage.`);
            }
        }

        function printRecipe() {
            if (!selections.cakeBase) {
                alert('No recipe to print! Please complete the selections first.');
                return;
            }
            window.print();
        }

        function placeOrder() {
            if (!selections.cakeBase) {
                alert('Please complete your Poppiesss selection before placing an order!');
                return;
            }
            showStep(totalSteps - 1);
        }

        document.addEventListener('DOMContentLoaded', () => {
            setupSelectionListeners();
            showStep(0);
        });
    </script>
</body>
</html>
