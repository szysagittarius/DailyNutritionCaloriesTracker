<template>
    <div>
        <table>
            <thead>
                <tr>
                    <th>Food Item</th>
                    <th>Serve Amount (g)</th>
                    <th>Total Carbs</th>
                    <th>Total Fat</th>
                    <th>Total Protein</th>
                    <th>Total Calories</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(entry, index) in entries" :key="index">
                    <td>
                        <input type="text"
                               v-model="entry.name"
                               @input="autocomplete(index)"
                               :list="'food-list-' + index">
                        <datalist :id="'food-list-' + index">
                            <option v-for="food in filteredFoods(entry.name)" :value="food.name" :key="food.name"></option>
                        </datalist>
                    </td>
                    <td><input type="number" v-model.number="entry.amount" @input="calculateNutrients(index)"></td>
                    <td>{{ formatNumber(entry.carbs) }}</td>
                    <td>{{ formatNumber(entry.fat) }}</td>
                    <td>{{ formatNumber(entry.protein) }}</td>
                    <td>{{ entry.calories.toFixed(2) }}</td>
                    <td><button @click="removeEntry(index)">Remove</button></td>
                </tr>
            </tbody>
        </table>
        <button @click="addEntry">Add Food Item</button>
        <button @click="submitFoodLog">Submit Food Log</button>
    </div>
</template>

<script>
    import axios from 'axios';
    import { FoodItemDto } from '@/models/FoodItemDto';

    export default {
        emits: ['food-log-submitted'],
        props: {
            post: Array,
            entries: Array,
            // ADD THIS MISSING PROP
            userId: {
                type: String,
                required: true
            }
        },
        methods: {
            addEntry() {
                this.entries.push({ name: '', amount: 0, calories: 0, carbs: 0, fat: 0, protein: 0 });
            },
            removeEntry(index) {
                this.entries.splice(index, 1);
            },
            calculateNutrients(index) {
                const entry = this.entries[index];
                const foodItem = this.post.find(food => food.name === entry.name);
                if (foodItem) {
                    entry.calories = (foodItem.calories / 100) * entry.amount;
                    entry.carbs = (foodItem.carbs / 100) * entry.amount;
                    entry.fat = (foodItem.fat / 100) * entry.amount;
                    entry.protein = (foodItem.protein / 100) * entry.amount;
                    // ADD THIS: Set the foodNutritionId when a food is selected
                    entry.foodNutritionId = foodItem.id;
                }
            },
            autocomplete(index) {
                this.calculateNutrients(index);
            },
            filteredFoods(searchTerm) {
                if (!searchTerm) return [];
                const lowerCaseTerm = searchTerm.toLowerCase();
                return this.post.filter(food => food.name.toLowerCase().includes(lowerCaseTerm));
            },
            formatNumber(value) {
                return Number(value).toFixed(2);
            },
            submitFoodLog() {
                // ADD DEBUGGING
                console.log('=== DAILY FOOD ENTRY TABLE SUBMIT ===');
                console.log('userId prop:', this.userId);
                console.log('entries:', this.entries);
                
                // Validate userId
                if (!this.userId) {
                    console.error('❌ userId is missing in DailyFoodEntryTable');
                    alert('User ID is required. Please log in again.');
                    return;
                }

                // Filter for valid entries with foodNutritionId
                const validEntries = this.entries.filter(entry => 
                    entry.name && entry.amount > 0 && entry.foodNutritionId
                );

                console.log('Valid entries with foodNutritionId:', validEntries);

                if (validEntries.length === 0) {
                    alert('Please select valid food items from the dropdown');
                    return;
                }

                const foodLogDto = {
                    Id: null,
                    DateTime: new Date(),
                    TotalCalories: this.entries.reduce((total, item) => total + item.calories, 0),
                    TotalCarbs: this.entries.reduce((total, item) => total + item.carbs, 0),
                    TotalProtein: this.entries.reduce((total, item) => total + item.protein, 0),
                    TotalFat: this.entries.reduce((total, item) => total + item.fat, 0),
                    // Update FoodItems to include foodNutritionId
                    FoodItems: validEntries.map(item => ({
                        name: item.name,
                        unit: item.amount,
                        foodNutritionId: item.foodNutritionId,
                        calories: item.calories,
                        carbs: item.carbs,
                        protein: item.protein,
                        fat: item.fat
                    })),
                    CreateTime: new Date(),
                    UpdateTime: new Date(),
                    UserId: this.userId, // This should now work
                };

                console.log('📤 Sending foodLogDto:', foodLogDto);

                axios.post('/foodlog/createfoodlog', foodLogDto)
                    .then(response => {
                        console.log('✅ Food log created successfully:', response.data);
                        this.$emit('food-log-submitted');
                        alert('Food log submitted successfully!');
                    })
                    .catch(error => {
                        console.error('❌ Error creating food log:', error);
                        console.error('❌ Error details:', error.response?.data);
                        alert('Failed to submit food log: ' + (error.response?.data?.message || error.message));
                    });
            }
        }
    }
</script>
