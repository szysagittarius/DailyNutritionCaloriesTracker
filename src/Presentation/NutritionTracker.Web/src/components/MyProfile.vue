<template>
  <div class="profile-container">
    <div class="profile-header">
      <h1 class="profile-title">User Profile</h1>
      <p class="profile-subtitle">Manage your personal information and nutrition goals</p>
    </div>

    <div class="profile-content">
      <div class="profile-card">
        <form @submit.prevent="saveProfile" class="profile-form">
          <!-- Nutrition Goals Section -->
          <div class="form-section">
            <h3 class="section-title">Nutrition Goals</h3>
            <div class="form-group">
              <label for="suggestedCalories" class="form-label">Daily Calorie Goal</label>
              <div class="calorie-input-container">
                <input
                  id="suggestedCalories"
                  v-model.number="profile.suggestedCalories"
                  type="number"
                  class="form-input calorie-input"
                />
                <span class="calorie-unit">calories</span>
              </div>
            </div>
            
            <!-- New nutrition goal input fields (without validation attributes) -->
            <div class="form-grid">
              <div class="form-group">
                <label for="suggestedCarbs" class="form-label">Daily Carbs Goal</label>
                <div class="macro-input-container">
                  <input
                    id="suggestedCarbs"
                    v-model.number="profile.suggestedCarbs"
                    type="number"
                    class="form-input macro-input"
                  />
                  <span class="macro-unit">g</span>
                </div>
              </div>
              
              <div class="form-group">
                <label for="suggestedProtein" class="form-label">Daily Protein Goal</label>
                <div class="macro-input-container">
                  <input
                    id="suggestedProtein"
                    v-model.number="profile.suggestedProtein"
                    type="number"
                    class="form-input macro-input"
                  />
                  <span class="macro-unit">g</span>
                </div>
              </div>
            </div>
            
            <div class="form-group">
              <label for="suggestedFat" class="form-label">Daily Fat Goal</label>
              <div class="macro-input-container">
                <input
                  id="suggestedFat"
                  v-model.number="profile.suggestedFat"
                  type="number"
                  class="form-input macro-input"
                />
                <span class="macro-unit">g</span>
              </div>
            </div>
          </div>

          <!-- Personal Information Section -->
          <div class="form-section">
            <h3 class="section-title">Personal Information</h3>
            <div class="form-grid">
              <div class="form-group">
                <label for="name" class="form-label">Full Name</label>
                <input
                  id="name"
                  v-model="profile.name"
                  type="text"
                  class="form-input"
                  placeholder="Enter your full name"
                  required
                />
              </div>
              <div class="form-group">
                <label for="email" class="form-label">Email Address</label>
                <input
                  id="email"
                  v-model="profile.email"
                  type="email"
                  class="form-input"
                  placeholder="Enter your email"
                  required
                />
              </div>
            </div>
            <div class="form-group">
              <label for="password" class="form-label">Password (leave blank to keep current)</label>
              <input
                id="password"
                v-model="profile.password"
                type="password"
                class="form-input"
                placeholder="Enter new password"
              />
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="form-actions">
            <button type="submit" class="save-button" :disabled="isSaving">
              {{ isSaving ? 'Saving...' : 'Save Changes' }}
            </button>
            <button type="button" class="cancel-button" @click="resetForm">
              Reset
            </button>
          </div>
        </form>

        <!-- Success/Error Messages -->
        <div v-if="successMessage" class="message success-message">
          {{ successMessage }}
        </div>
        <div v-if="errorMessage" class="message error-message">
          {{ errorMessage }}
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import api from '../services/api'

export default {
  name: 'ProfilePage',
  data() {
    return {
      profile: {
        name: '',
        email: '',
        password: '',
        suggestedCalories: 2456,
        suggestedCarbs: 246,      // ADD THIS
        suggestedFat: 68,         // ADD THIS
        suggestedProtein: 215     // ADD THIS
      },
      isSaving: false,
      successMessage: '',
      errorMessage: ''
    }
  },
  async mounted() {
    await this.loadUserProfile()
  },
  methods: {
    async loadUserProfile() {
      try {
        const currentUser = api.getCurrentUser()
        if (currentUser) {
          const response = await fetch(`/user/profile/${currentUser.username}`)
          
          if (response.ok) {
            const userData = await response.json()
            this.profile.name = userData.name || currentUser.username || ''
            this.profile.email = userData.email || ''
            this.profile.suggestedCalories = userData.suggestedCalories || 2456
            this.profile.suggestedCarbs = userData.suggestedCarbs || 246          // ADD THIS
            this.profile.suggestedFat = userData.suggestedFat || 68               // ADD THIS
            this.profile.suggestedProtein = userData.suggestedProtein || 215      // ADD THIS
          }
          // You might want to fetch more user data from the backend here
        }
      } catch (error) {
        console.error('Error loading user profile:', error)
      }
    },
    
    async saveProfile() {
      this.isSaving = true
      this.successMessage = ''
      this.errorMessage = ''
      
      try {
        const response = await fetch('/user/updateprofile', {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(this.profile)
        })
        
        if (response.ok) {
          const result = await response.json()
          this.successMessage = 'Profile updated successfully!'
          
          // Update the nutrition tracker with new calorie goal
          this.$emit('profile-updated', this.profile.suggestedCalories)
        } else {
          const error = await response.json()
          this.errorMessage = error.message || 'Failed to update profile'
        }
      } catch (error) {
        console.error('Error updating profile:', error)
        this.errorMessage = 'An error occurred while updating profile'
      } finally {
        this.isSaving = false
        
        // Clear messages after 3 seconds
        setTimeout(() => {
          this.successMessage = ''
          this.errorMessage = ''
        }, 3000)
      }
    },
    
    resetForm() {
      this.loadUserProfile()
      this.successMessage = ''
      this.errorMessage = ''
    }
  }
}
</script>

<style scoped>
.profile-container {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.profile-header {
  text-align: center;
  margin-bottom: 40px;
  padding: 30px 0;
}

.profile-title {
  font-size: 2.2rem; /* Slightly larger */
  color: #333;
  margin-bottom: 10px;
  font-weight: 700;
}

.profile-subtitle {
  font-size: 1.1rem; /* Slightly larger */
  color: #666;
  margin: 0;
}

.profile-content {
  display: flex;
  justify-content: center;
  align-items: flex-start;
  min-height: calc(100vh - 200px);
  width: 100%;
}

.profile-card {
  background: white;
  border-radius: 16px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  padding: 50px;
  width: 100%;
  max-width: 700px;
  margin: 0 auto;
}

@media (max-width: 768px) {
  .profile-card {
    padding: 30px 20px;
  }
  
  .profile-container {
    padding: 10px;
  }
  
  .profile-title {
    font-size: 1.8rem;
  }
}

.profile-form {
  display: flex;
  flex-direction: column;
  gap: 35px; /* Increased spacing */
}

.form-section {
  display: flex;
  flex-direction: column;
  gap: 25px; /* Increased spacing */
}

.section-title {
  font-size: 1.4rem; /* Slightly larger */
  color: #333;
  margin-bottom: 15px; /* Increased */
  padding-bottom: 10px;
  border-bottom: 2px solid #e9ecef;
  font-weight: 600;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 25px; /* Increased from likely 15px */
}

@media (max-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 10px; /* Increased spacing */
}

.form-label {
  font-weight: 600;
  color: #333;
  font-size: 1rem;
}

.form-input {
  padding: 14px 18px; /* Increased padding */
  border: 2px solid #e1e5e9;
  border-radius: 8px; /* Slightly smaller radius */
  font-size: 1rem; /* Standard size */
  transition: all 0.3s ease;
  background-color: #f8f9fa;
}

.form-input:focus {
  outline: none;
  border-color: #667eea;
  background-color: white;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.calorie-input-container {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 15px;
}

.calorie-input {
  flex: 1;
  max-width: 180px;
}

.calorie-unit {
  font-weight: 500;
  color: #666;
  font-size: 1rem;
}

.macro-input-container {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.macro-input {
  flex: 1;
  max-width: 180px;
}

.macro-unit {
  font-weight: 500;
  color: #666;
  font-size: 1rem;
}

.form-help {
  color: #666;
  font-size: 0.9rem;
  font-style: italic;
}

.form-actions {
  display: flex;
  gap: 20px;
  justify-content: center;
  margin-top: 20px;
}

.save-button {
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
  color: white;
  border: none;
  padding: 14px 28px; /* Increased padding */
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  min-width: 140px; /* Increased width */
}

.save-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(40, 167, 69, 0.4);
}

.cancel-button {
  background: #6c757d;
  color: white;
  border: none;
  padding: 14px 28px; /* Increased padding */
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  min-width: 140px; /* Increased width */
}

.cancel-button:hover {
  background: #545b62;
  transform: translateY(-2px);
}

.message {
  margin-top: 20px;
  padding: 16px 20px;
  border-radius: 8px;
  text-align: center;
  font-weight: 500;
}

.success-message {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.error-message {
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}
</style>