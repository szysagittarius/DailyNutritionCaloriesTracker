<script setup>
    import NutritionTracker from './components/NutritionTracker.vue'
    import TheWelcome from './components/TheWelcome.vue'
    import FoodLog from './components/FoodLog.vue'
    import FoodLogPage from './components/MyFoodLog.vue'
    import ProfilePage from './components/MyProfile.vue'
    import NutritionManagement from './components/NutritionManagement.vue'
    import { ref, onMounted } from 'vue'
    import { useRouter } from 'vue-router'
    import api from './services/api'

    const foodLogKey = ref(0)
    const activeTab = ref('home')
    const router = useRouter()
    const currentUser = ref(null) // Changed from api.getCurrentUser() to null
    const userSuggestedCalories = ref(2456) // Default value

    // Function to load user data
    const loadUserData = () => {
        currentUser.value = api.getCurrentUser()
        
        // Add debug logging
        console.log('=== APP.VUE DEBUG USER INFO ===')
        console.log('api.getCurrentUser() result:', api.getCurrentUser())
        console.log('currentUser.value:', currentUser.value)
        console.log('currentUser.value?.id:', currentUser.value?.id)
        console.log('currentUser.value?.userId:', currentUser.value?.userId)
        console.log('currentUser.value?.username:', currentUser.value?.username)
        console.log('=== END APP.VUE DEBUG ===')
    }

    // Load user data when component mounts
    onMounted(() => {
        loadUserData()
        
        // If no user data and not on login page, redirect to login
        if (!currentUser.value && router.currentRoute.value.path !== '/login') {
            router.push('/login')
        }
    })

    const handleFoodLogSubmitted = () => {
        foodLogKey.value++
    }

    const setActiveTab = (tab) => {
        activeTab.value = tab
    }

    const handleLogout = () => {
        api.logout()
        router.push('/login')
    }

    const handleProfileUpdated = (newSuggestedCalories) => {
        userSuggestedCalories.value = newSuggestedCalories
    }
</script>

<template>
  <div id="app">
    <router-view v-if="$route.path === '/login'" />
    <div v-else>
      <header>
        <!-- Menu Bar -->
        <nav class="menu-bar">
          <ul class="tab-list">
            <li class="tab-item" :class="{ active: activeTab === 'home' }" @click="setActiveTab('home')">
              Home
            </li>
            <li class="tab-item" :class="{ active: activeTab === 'profile' }" @click="setActiveTab('profile')">
              User Profile
            </li>
            <li class="tab-item" :class="{ active: activeTab === 'nutrition' }" @click="setActiveTab('nutrition')">
              Food Nutrition Management
            </li>
            <li class="tab-item" :class="{ active: activeTab === 'foodlog' }" @click="setActiveTab('foodlog')">
              Food Log
            </li>
            <li class="user-info">
              <span>Welcome, {{ currentUser?.username || 'User' }}!</span>
              <button @click="handleLogout" class="logout-btn">Logout</button>
            </li>
          </ul>
        </nav>
      </header>

      <main>
        <TheWelcome />

        <!-- Profile Page -->
        <div v-if="activeTab === 'profile'" class="profile-content">
          <ProfilePage @profile-updated="handleProfileUpdated" />
        </div>
        
        <!-- Nutrition Management Page -->
        <div v-else-if="activeTab === 'nutrition'" class="nutrition-content">
          <NutritionManagement />
        </div>
        
        <!-- Food Log Page -->
        <div v-else-if="activeTab === 'foodlog'" class="food-log-content">
          <FoodLogPage />
        </div>
        
        <!-- Main Content for home tab -->
        <div v-else class="main-content">
          <div class="left-panel">
            <!-- ADD THE MISSING :user-id PROP HERE -->
            <NutritionTracker 
              v-if="currentUser && currentUser.id"
              msg="You did it!" 
              :suggested-calories="userSuggestedCalories"
              :user-id="currentUser.id"
              @food-log-submitted="handleFoodLogSubmitted" 
            />
            <div v-else>
              Loading user data...
            </div>
          </div>
          <div class="right-panel">
            <FoodLog :key="foodLogKey"></FoodLog>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
}

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  margin: 0;
  padding: 0;
}

:root {
  --content-max-width: 1200px; /* For home tab */
  --data-page-max-width: 1600px; /* For data pages - wider */
  --header-height: 64px;
  --gap: 1.5rem;
}

/* Header: always spans the viewport and sits above content */
header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: var(--header-height);
  z-index: 1000;
  background: #f8f9fa;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  align-items: center;
  box-sizing: border-box;
}

/* Center the tab area inside header */
.menu-bar {
  width: 100%;
  display: flex;
  justify-content: center;
  padding: 0 1rem;
  box-sizing: border-box;
}

/* Tabs container should match the widest content */
.tab-list {
  display: flex;
  gap: 1rem;
  margin: 0;
  padding: 0;
  list-style: none;
  width: 100%;
  max-width: var(--data-page-max-width); /* Change from --content-max-width to --data-page-max-width */
  align-items: center;
  box-sizing: border-box;
  margin-left: auto;
  margin-right: auto;
}

/* Individual tab */
.tab-item {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 0.45rem 0.9rem;
  background: #fff;
  border: 1px solid #ddd;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color .15s, border-color .15s;
  font-weight: 500;
  white-space: nowrap;
}
.tab-item:hover { background: #e9ecef; border-color: #adb5bd; }
.tab-item.active { background: #007bff; color: #fff; border-color: #007bff; }

/* User info section */
.user-info {
  margin-left: auto;
  display: flex;
  align-items: center;
  gap: 1rem;
  font-weight: 500;
}

.logout-btn {
  padding: 0.4rem 0.8rem;
  background-color: #dc3545;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background-color 0.2s;
}

.logout-btn:hover {
  background-color: #c82333;
}

/* Main content sits below the fixed header */
main {
  padding-top: var(--header-height);
  box-sizing: border-box;
  width: 100%;
}

/* Centered two-column layout */
/* Home tab - standard width */
.main-content {
  display: flex;
  gap: var(--gap);
  padding: 2rem;
  width: 100%;
  max-width: var(--content-max-width); /* 1200px */
  margin: 0 auto;
  min-height: calc(100vh - var(--header-height));
  box-sizing: border-box;
}

/* Data pages - wider */
.profile-content,
.nutrition-content,
.food-log-content {
  display: flex;
  gap: var(--gap);
  padding: 2rem;
  width: 100%;
  max-width: var(--data-page-max-width); /* 1600px - wider! */
  margin: 0 auto;
  min-height: calc(100vh - var(--header-height));
  box-sizing: border-box;
  flex-direction: column;
}

.left-panel,
.right-panel {
  flex: 1;
  box-sizing: border-box;
}

/* Visuals for panels */
.left-panel { background: #f8f9fa; padding: 1.5rem; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.06); }
.right-panel { background: #fff; padding: 1.5rem; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.06); border: 1px solid #e0e0e0; }

/* Update responsive behavior */
@media (max-width: 768px) { /* Changed from 900px to 768px */
  .tab-list { 
    gap: 0.5rem; 
    overflow-x: auto; 
    padding: 0 0.5rem; 
  }
  
  .main-content,
  .profile-content,
  .nutrition-content,
  .food-log-content { 
    flex-direction: column; 
    padding: 1rem; /* Only reduce padding on mobile */
  }
  
  .user-info { 
    margin-left: 0; 
    margin-top: 0.5rem; 
  }
}

/* Add this to the END of your App.vue <style> section to override main.css */
@media (min-width: 1024px) {
  body {
    display: block !important;  /* Override the flex */
    place-items: unset !important;
  }

  #app {
    display: block !important;           /* Override the grid */
    grid-template-columns: unset !important;
    padding: 0 !important;               /* Let your components control padding */
    max-width: none !important;          /* Remove the 1280px limit */
    margin: 0 !important;
  }
}
</style>
