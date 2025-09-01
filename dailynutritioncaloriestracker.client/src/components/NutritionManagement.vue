<template>
  <div class="nutrition-management">
    <h2>Food Nutrition Management</h2>
    
    <!-- Food Nutrition Table -->
    <div class="table-section">
      <h3>Food Nutrition Database</h3>
      <div class="table-container">
        <table class="nutrition-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Measurement</th>
              <th>Carbs (g)</th>
              <th>Fat (g)</th>
              <th>Protein (g)</th>
              <th>Calories</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in paginatedItems" :key="item.id">
              <td>{{ item.name }}</td>
              <td>{{ item.measurement }}</td>
              <td>{{ item.carbs }}</td>
              <td>{{ item.fat }}</td>
              <td>{{ item.protein }}</td>
              <td>{{ item.calories }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Pagination -->
      <div class="pagination">
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

    <!-- Add New Food Nutrition Form -->
    <div class="form-section">
      <h3>Add New Food Nutrition</h3>
      <form @submit.prevent="submitForm" class="nutrition-form">
        <div class="form-row">
          <div class="form-group">
            <label for="name">Name:</label>
            <input 
              type="text" 
              id="name" 
              v-model="formData.name" 
              required 
              class="form-input"
            />
          </div>
          <div class="form-group">
            <label for="measurement">Measurement:</label>
            <input 
              type="text" 
              id="measurement" 
              v-model="formData.measurement" 
              readonly
              class="form-input form-input-readonly"
            />
          </div>
        </div>
        
        <div class="form-row">
          <div class="form-group">
            <label for="carbs">Carbs (g):</label>
            <input 
              type="number" 
              id="carbs" 
              v-model.number="formData.carbs" 
              step="0.1" 
              min="0" 
              required 
              class="form-input"
            />
          </div>
          <div class="form-group">
            <label for="fat">Fat (g):</label>
            <input 
              type="number" 
              id="fat" 
              v-model.number="formData.fat" 
              step="0.1" 
              min="0" 
              required 
              class="form-input"
            />
          </div>
        </div>
        
        <div class="form-row">
          <div class="form-group">
            <label for="protein">Protein (g):</label>
            <input 
              type="number" 
              id="protein" 
              v-model.number="formData.protein" 
              step="0.1" 
              min="0" 
              required 
              class="form-input"
            />
          </div>
          <div class="form-group">
            <label for="calories">Calories:</label>
            <input 
              type="number" 
              id="calories" 
              v-model.number="formData.calories" 
              min="0" 
              required 
              class="form-input"
            />
          </div>
        </div>
        
        <div class="form-actions">
          <button type="submit" :disabled="isSubmitting" class="submit-btn">
            {{ isSubmitting ? 'Adding...' : 'Add Food Nutrition' }}
          </button>
          <button type="button" @click="resetForm" class="reset-btn">
            Reset
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

// Reactive data
const nutritionItems = ref([])
const currentPage = ref(1)
const itemsPerPage = 10
const isSubmitting = ref(false)

// Form data
const formData = ref({
  name: '',
  measurement: 'per 100g',
  carbs: 0,
  fat: 0,
  protein: 0,
  calories: 0
})

// Computed properties
const totalPages = computed(() => {
  return Math.ceil(nutritionItems.value.length / itemsPerPage)
})

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return nutritionItems.value.slice(start, end)
})

// Methods
const fetchNutritionData = async () => {
  try {
    console.log('Fetching nutrition data...')
    const response = await fetch('foodnutrition', {
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
    
    nutritionItems.value = Array.isArray(data) ? data : []
    console.log('Loaded nutrition items:', nutritionItems.value.length)
  } catch (error) {
    console.error('Error fetching nutrition data:', error)
    alert('Failed to load nutrition data: ' + error.message)
  }
}

const submitForm = async () => {
  isSubmitting.value = true
  try {
    console.log('Submitting form data:', formData.value)
    const response = await fetch('foodnutrition', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(formData.value)
    })
    
    console.log('Response status:', response.status)
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    
    const result = await response.json()
    console.log('Submit response:', result)
    alert('Food nutrition added successfully!')
    resetForm()
    await fetchNutritionData() // Refresh the table
  } catch (error) {
    console.error('Error adding food nutrition:', error)
    alert('Failed to add food nutrition: ' + error.message)
  } finally {
    isSubmitting.value = false
  }
}

const resetForm = () => {
  formData.value = {
    name: '',
    measurement: 'per 100g',
    carbs: 0,
    fat: 0,
    protein: 0,
    calories: 0
  }
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
  fetchNutritionData()
})
</script>

<style scoped>
.nutrition-management {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.nutrition-management h2 {
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

.table-container {
  overflow-x: auto;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.nutrition-table {
  width: 100%;
  border-collapse: collapse;
}

.nutrition-table th,
.nutrition-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.nutrition-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #495057;
}

/* Alternating row colors */
.nutrition-table tbody tr:nth-child(odd) {
  background-color: #ffffff;
}

.nutrition-table tbody tr:nth-child(even) {
  background-color: #f8f9fa;
}

.nutrition-table tbody tr:hover {
  background-color: #e9ecef !important;
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

.form-section {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.form-section h3 {
  color: #34495e;
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
}

.nutrition-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #495057;
}

.form-input {
  padding: 0.75rem;
  border: 1px solid #ced4da;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.form-input:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.1);
}

.form-input-readonly {
  background-color: #f8f9fa;
  color: #6c757d;
  cursor: not-allowed;
}

.form-input-readonly:focus {
  border-color: #ced4da;
  box-shadow: none;
}

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
}

.submit-btn {
  padding: 0.75rem 1.5rem;
  background-color: #28a745;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: background-color 0.2s;
}

.submit-btn:hover:not(:disabled) {
  background-color: #218838;
}

.submit-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.reset-btn {
  padding: 0.75rem 1.5rem;
  background-color: #6c757d;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: background-color 0.2s;
}

.reset-btn:hover {
  background-color: #545b62;
}

@media (max-width: 768px) {
  .form-row {
    grid-template-columns: 1fr;
  }
  
  .form-actions {
    flex-direction: column;
  }
  
  .nutrition-management {
    padding: 1rem;
  }
}
</style>