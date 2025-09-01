<template>
  <div class="food-log-management">
    <h2>Food Log Management</h2>
    
    <!-- Summary Section -->
    <div class="summary-section">
      <h3>Today's Summary</h3>
      
      <!-- Import the nutrition component here -->
      <TodayNutritionSummary 
        :today-totals="todaySummary"
        :suggested-calories="2456"
        :suggested-carbs="246"
        :suggested-fat="68"
        :suggested-protein="215"
      />
      
      <!-- Keep existing summary cards below or remove them -->
      <div class="summary-cards">
        <div class="summary-card">
          <h4>Total Calories</h4>
          <p class="summary-value">{{ todaySummary.calories.toFixed(1) }}</p>
        </div>
        <div class="summary-card">
          <h4>Protein</h4>
          <p class="summary-value">{{ todaySummary.protein.toFixed(1) }}g</p>
        </div>
        <div class="summary-card">
          <h4>Carbs</h4>
          <p class="summary-value">{{ todaySummary.carbs.toFixed(1) }}g</p>
        </div>
        <div class="summary-card">
          <h4>Fat</h4>
          <p class="summary-value">{{ todaySummary.fat.toFixed(1) }}g</p>
        </div>
      </div>
    </div>
    
    <!-- Food Log Table -->
    <div class="table-section">
      <div class="history-container">
        <h3>Your Food Log History</h3>
        <div class="table-container">
          <table class="food-log-table">
            <thead>
              <tr>
                <th>Date</th>
                <th>Calories</th>
                <th>Protein (g)</th>
                <th>Carbs (g)</th>
                <th>Fat (g)</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in paginatedItems" :key="item.id">
                <td>{{ formatDate(item.dateLogged) }}</td>
                <td>{{ item.calories.toFixed(1) }}</td>
                <td>{{ item.protein.toFixed(1) }}</td>
                <td>{{ item.carbs.toFixed(1) }}</td>
                <td>{{ item.fat.toFixed(1) }}</td>
              </tr>
            </tbody>
          </table>
          
          <!-- Empty state -->
          <div v-if="foodLogItems.length === 0 && !isLoading" class="empty-state">
            <p>No food log entries found.</p>
          </div>
          
          <!-- Loading state -->
          <div v-if="isLoading" class="loading-state">
            <p>Loading food log data...</p>
          </div>
        </div>
        
        <!-- Pagination -->
        <div class="pagination" v-if="totalPages > 1">
          <button 
            @click="previousPage" 
            :disabled="currentPage === 1"
            class="pagination-btn"
          >
            Previous
          </button>
          <span class="page-info">
            Page {{ currentPage }} of {{ totalPages }}
          </span>
          <button 
            @click="nextPage" 
            :disabled="currentPage === totalPages"
            class="pagination-btn"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '../services/api'
import TodayNutritionSummary from './TodayNutritionSummary.vue'

// Reactive data
const foodLogItems = ref([])
const currentPage = ref(1)
const itemsPerPage = 10
const isLoading = ref(false)

// Get current user from API service (same as App.vue)
const currentUser = ref(api.getCurrentUser())
const userId = currentUser.value?.id || '00000000-0000-0000-0000-000000000001'

// Computed properties
const totalPages = computed(() => {
  return Math.ceil(foodLogItems.value.length / itemsPerPage)
})

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return foodLogItems.value.slice(start, end)
})

const todaySummary = computed(() => {
  const today = new Date().toDateString()
  const todayItems = foodLogItems.value.filter(item => {
    const itemDate = new Date(item.dateLogged).toDateString()
    return itemDate === today
  })
  
  // This sums up ALL today's food log entries
  return todayItems.reduce((summary, item) => {
    summary.calories += item.calories || 0
    summary.protein += item.protein || 0
    summary.carbs += item.carbs || 0
    summary.fat += item.fat || 0
    return summary
  }, { calories: 0, protein: 0, carbs: 0, fat: 0 })
})

// Methods
const fetchFoodLogData = async () => {
  isLoading.value = true
  try {
    console.log('Fetching food log data for user:', userId)
    console.log('Current user object:', currentUser.value)
    
    const response = await fetch(`foodlog/GetUserFoodLogs/${userId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })
    
    console.log('Response status:', response.status)
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    
    const data = await response.json()
    console.log('Response data:', data)
    
    // Transform the data - show only summary per food log entry
    const transformedData = []
    if (Array.isArray(data)) {
      data.forEach(log => {
        transformedData.push({
          id: log.id || Math.random(),
          dateLogged: log.dateTime || log.createTime,
          calories: log.totalCalories || 0,
          protein: log.totalProtein || 0,
          carbs: log.totalCarbs || 0,
          fat: log.totalFat || 0
        })
      })
    }
    
    // Sort by date (newest first)
    transformedData.sort((a, b) => new Date(b.dateLogged) - new Date(a.dateLogged))
    
    foodLogItems.value = transformedData
    console.log('Loaded food log entries:', foodLogItems.value.length)
  } catch (error) {
    console.error('Error fetching food log data:', error)
    alert('Failed to load food log data: ' + error.message)
  } finally {
    isLoading.value = false
  }
}

const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const previousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

// Lifecycle
onMounted(() => {
  fetchFoodLogData()
})
</script>

<style scoped>
.food-log-management {
  padding: 2rem;
  /* Remove these lines if they exist: */
  /* max-width: 1200px; */
  /* margin: 0 auto; */
}

/* Make sure the table container uses full width */
.table-container {
  overflow-x: auto;
  background: transparent;
  border-radius: 0;
  box-shadow: none;
  margin-bottom: 1.5rem;
  width: 100%; /* Ensure full width */
}

.food-log-table {
  width: 100%;
  border-collapse: collapse;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

.food-log-management h2 {
  color: #2c3e50;
  margin-bottom: 2rem;
  font-size: 2rem;
  font-weight: 600;
}

.table-section {
  margin-bottom: 3rem;
}

.table-section h3 {
  color: #34495e;
  margin-bottom: 1rem;
  font-size: 1.5rem;
}

.food-log-table th,
.food-log-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.food-log-table td:not(:first-child) {
  text-align: right; /* Right-align numeric values */
}

.food-log-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #495057;
}

/* Alternating row colors */
.food-log-table tbody tr:nth-child(odd) {
  background-color: #ffffff;
}

.food-log-table tbody tr:nth-child(even) {
  background-color: #f8f9fa;
}

.food-log-table tbody tr:hover {
  background-color: #e9ecef !important;
}

.empty-state,
.loading-state {
  padding: 3rem;
  text-align: center;
  color: #6c757d;
  font-style: italic;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 1rem;
}

.pagination-btn {
  padding: 0.5rem 1rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background-color 0.2s;
}

.pagination-btn:hover:not(:disabled) {
  background-color: #0056b3;
}

.pagination-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.page-info {
  font-weight: 500;
  color: #495057;
}

.summary-section {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.summary-section h3 {
  color: #34495e;
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
}

.summary-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
}

.summary-card {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 8px;
  text-align: center;
  border: 1px solid #e9ecef;
}

.summary-card h4 {
  color: #495057;
  margin-bottom: 0.5rem;
  font-size: 1rem;
  font-weight: 500;
}

.summary-value {
  color: #007bff;
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0;
}

.history-container {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.history-container h3 {
  color: #34495e;
  margin-bottom: 1.5rem;
  margin-top: 0;
  font-size: 1.5rem;
}

@media (max-width: 768px) {
  .food-log-management {
    padding: 1rem;
  }
  
  .summary-cards {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>