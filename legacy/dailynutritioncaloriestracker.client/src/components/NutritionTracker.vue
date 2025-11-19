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
                <DailyFoodEntryTable :post="post" :entries="entries" :user-id="userId" @food-log-submitted="$emit('food-log-submitted')" />
            </div>

            <!-- Display nutrition table so far -->
            <nutrition-table :posts="post" :loading="loading" />
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
        props: {
            suggestedCalories: {
                type: Number,
                default: 2456
            },
            // Add userId prop to receive from parent component
            userId: {
                type: String,
                required: true
            }
        },
        data() {
            return {
                loading: false,
                post: null,              
                suggestedCarbs: 246, 
                suggestedFat: 68, 
                suggestedProtein: 215, 
                entries: [{ 
                    name: '', 
                    amount: 0, 
                    calories: 0, 
                    protein: 0, 
                    carbs: 0, 
                    fat: 0,
                    foodNutritionId: null
                }],
                availableFoods: []
            };
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
                return Math.min(percentage, 100);
            },     
            circumference() {
                const radius = 80;
                return 2 * Math.PI * radius;
            }
        },
        methods: {
            fetchData() {
                this.loading = true;
                fetch('foodnutrition/getlist')
                    .then(r => r.json())
                    .then(json => {
                        this.post = json;
                        this.loading = false;
                    });
            },
            async fetchAvailableFoods() {
                try {
                    const response = await fetch('foodnutrition/getlist');
                    
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    
                    const foods = await response.json();
                    this.availableFoods = foods;
                    console.log('Fetched available foods:', foods);
                } catch (error) {
                    console.error('Error fetching available foods:', error);
                    this.availableFoods = [];
                }
            },
            calculateTotal(nutrient) {
                return this.entries.reduce((total, entry) => total + entry[nutrient], 0);
            },
            async submitFoodLog() {
                try {
                    console.log('=== SUBMIT FOOD LOG START ===');
                    console.log('Current userId prop:', this.userId);
                    console.log('Type of userId:', typeof this.userId);
                    console.log('Current entries:', this.entries);

                    // Validate entries have required FoodNutritionId
                    const validEntries = this.entries.filter(entry => 
                        entry.name && entry.amount > 0 && entry.foodNutritionId
                    );

                    console.log('Valid entries:', validEntries);

                    if (validEntries.length === 0) {
                        console.log('❌ No valid entries found');
                        alert('Please add at least one valid food item with nutrition information');
                        return;
                    }

                    // Make sure userId is valid
                    if (!this.userId || this.userId === '' || this.userId === 'undefined') {
                        console.log('❌ Invalid userId');
                        alert('User ID is required. Please log in again.');
                        return;
                    }

                    const foodLogData = {
                        userId: this.userId,
                        logDate: new Date().toISOString(),
                        foodItems: validEntries.map(entry => ({
                            name: entry.name,
                            unit: entry.amount,
                            foodNutritionId: entry.foodNutritionId
                        }))
                    };

                    console.log('📤 Sending foodLogData:', foodLogData);

                    const response = await fetch('/foodlog/createfoodlog', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify(foodLogData)
                    });

                    console.log('📥 Response status:', response.status);
                    console.log('📥 Response ok:', response.ok);

                    if (response.ok) {
                        const responseData = await response.json();
                        console.log('✅ Success response:', responseData);
                        this.$emit('food-log-submitted');
                        // Reset entries after successful submission
                        this.entries = [{ 
                            name: '', 
                            amount: 0, 
                            calories: 0, 
                            protein: 0, 
                            carbs: 0, 
                            fat: 0,
                            foodNutritionId: null
                        }];
                        this.fetchData();
                        alert('Food log submitted successfully!');
                    } else {
                        const error = await response.text();
                        console.error('❌ Error response:', error);
                        console.error('❌ Response status:', response.status);
                        alert('Error submitting food log: ' + error);
                    }
                    
                    console.log('=== SUBMIT FOOD LOG END ===');
                } catch (error) {
                    console.error('❌ Exception during submitFoodLog:', error);
                    alert('Error submitting food log: ' + error.message);
                }
            }
        },
        mounted() {
            // Add debug logging when component mounts
            console.log('=== NUTRITION TRACKER MOUNTED ===')
            console.log('Received userId prop:', this.userId)
            console.log('userId type:', typeof this.userId)
            console.log('=== END NUTRITION TRACKER DEBUG ===')
        },
        created() {
            this.fetchData();
            this.fetchAvailableFoods();
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