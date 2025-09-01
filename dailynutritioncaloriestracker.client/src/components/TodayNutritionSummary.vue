<template>
  <div class="nutrition-summary">
    <!-- Circular Progress Indicator -->
    <circular-progress :total-calories="todayTotals.calories"
                       :suggested-calories="suggestedCalories"
                       :progress-percentage="progressPercentage" />
    
    <!-- Display Total Calories and Suggested Limit -->
    <div class="calories-display">
      Total Calories: {{ todayTotals.calories.toFixed(2) }} kcal / {{ suggestedCalories }} kcal
    </div>

    <!-- Progress Bars -->
    <div class="progress-bars">
      <progress-bar label="Calories" :value="todayTotals.calories" :max="suggestedCalories" color="green"></progress-bar>
      <progress-bar label="Carbs" :value="todayTotals.carbs" :max="suggestedCarbs" color="orange"></progress-bar>
      <progress-bar label="Fat" :value="todayTotals.fat" :max="suggestedFat" color="purple"></progress-bar>
      <progress-bar label="Protein" :value="todayTotals.protein" :max="suggestedProtein" color="blue"></progress-bar>
    </div>
  </div>
</template>

<script>
import ProgressBar from './ProgressBar.vue'
import CircularProgress from './CircularProgress.vue'

export default {
  name: 'TodayNutritionSummary',
  components: {
    ProgressBar,
    CircularProgress
  },
  props: {
    todayTotals: {
      type: Object,
      required: true,
      default: () => ({
        calories: 0,
        protein: 0,
        carbs: 0,
        fat: 0
      })
    },
    suggestedCalories: {
      type: Number,
      default: 2456
    },
    suggestedCarbs: {
      type: Number,
      default: 246
    },
    suggestedFat: {
      type: Number,
      default: 68
    },
    suggestedProtein: {
      type: Number,
      default: 215
    }
  },
  computed: {
    progressPercentage() {
      const percentage = (this.todayTotals.calories / this.suggestedCalories) * 100
      return Math.min(percentage, 100)
    }
  }
}
</script>

<style scoped>
.nutrition-summary {
  text-align: center;
  padding: 1rem;
}

.calories-display {
  margin: 1rem 0;
  font-size: 1.1rem;
  font-weight: 500;
  color: #495057;
}

.progress-bars {
  margin-top: 1.5rem;
}
</style>