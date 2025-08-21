<template>
    <div class="foodnutrition-component">
        <h1>Daily food nutrition</h1>


        <div v-if="post" class="content">

            <!-- Circular Progress Indicator -->
            <circular-progress :total-calories="totalCalories"
                               :suggested-calories="suggestedCalories"
                               :progress-percentage="progressPercentage" />
            <!-- Display Total Calories and Suggested Limit -->
            <div>
                Total Calories: {{ totalCalories.toFixed(2) }} kcal / {{ suggestedCalories }} kcal
            </div>

            <progress-bar label="Calories" :value="totalCalories" :max="suggestedCalories" color="green"></progress-bar>
            <progress-bar label="Carbs" :value="totalCarbs" :max="suggestedCarbs" color="orange"></progress-bar>
            <progress-bar label="Fat" :value="totalFat" :max="suggestedFat" color="purple"></progress-bar>
            <progress-bar label="Protein" :value="totalProtein" :max="suggestedProtein" color="blue"></progress-bar>

            <div v-if="post" class="content">
                <DailyFoodEntryTable :post="post" :entries="entries" @food-log-submitted="$emit('food-log-submitted')" />
            </div>

            <!-- Display nutrition table so far -->
            <nutrition-table :posts="post":loading="loading" />
        </div>
    </div>
</template>

<script>
    import ProgressBar from './ProgressBar.vue';
    import CircularProgress from './CircularProgress.vue';
    import NutritionTable from './NutritionTable.vue';
    import DailyFoodEntryTable from './DailyFoodEntryTable.vue';

    export default {
        components: {
            ProgressBar,
            CircularProgress,
            NutritionTable,
            DailyFoodEntryTable
        },
        emits: ['food-log-submitted'],
        data() {
            return {
                loading: false,
                post: null,
                suggestedCalories: 2456,                 
                suggestedCarbs: 246, 
                suggestedFat: 68, 
                suggestedProtein: 215, 
                entries: [{ name: '', amount: 0, calories: 0, protein: 0, carbs: 0, fat: 0 }],
            };
        },
        props: {
            value: Number,
            max: Number,
            color: String,
        },
        computed: {
            totalCalories() {
                return this.calculateTotal('calories');
            },
            totalProtein() {
                return this.calculateTotal('protein');
            },
            totalCarbs() {
                return this.calculateTotal('carbs');
            },
            totalFat() {
                return this.calculateTotal('fat');
            },
            progressPercentage() {
                const percentage = (this.totalCalories / this.suggestedCalories) * 100;
                return Math.min(percentage, 100); // Cap the percentage at 100% to avoid overflow
            },     
            circumference() {
                const radius = 80; // Match the SVG circle's radius
                return 2 * Math.PI * radius;
            }
        },
        methods: {
            fetchData() {
                this.loading = true;
                // Ensure the fetch URL is correct
                fetch('foodnutrition/getlist')
                    .then(r => r.json())
                    .then(json => {
                        this.post = json;
                        this.loading = false;
                    });
            },
            calculateTotal(nutrient) {
                return this.entries.reduce((total, entry) => total + entry[nutrient], 0);
            },
        },
        created() {
            this.fetchData();
        },
    };
</script>

<style scoped>
th {
    font-weight: bold;
}
tr:nth-child(even) {
    background: #F2F2F2;
}

tr:nth-child(odd) {
    background: #FFF;
}

th, td {
    padding-left: .5rem;
    padding-right: .5rem;
}

.foodnutrition-component {
    text-align: center;
}

table {
    margin-left: auto;
    margin-right: auto;
}
</style>