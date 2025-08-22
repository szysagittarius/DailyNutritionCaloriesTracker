<script setup>
import NutritionTracker from '../components/NutritionTracker.vue'
import TheWelcome from '../components/TheWelcome.vue'
import FoodLog from '../components/FoodLog.vue'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'

const foodLogKey = ref(0)
const activeTab = ref('home')
const router = useRouter()
const currentUser = ref(api.getCurrentUser())

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
</script>

<template>
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
          <span>Welcome, {{ currentUser?.username }}!</span>
          <button @click="handleLogout" class="logout-btn">Logout</button>
        </li>
      </ul>
    </nav>
  </header>

  <main>
    <TheWelcome />
    <div class="main-content">
      <div class="left-panel">
        <NutritionTracker msg="You did it!" @food-log-submitted="handleFoodLogSubmitted" />
      </div>
      <div class="right-panel">
        <FoodLog :key="foodLogKey"></FoodLog>
      </div>
    </div>
  </main>
</template>

<style>
:root {
  --content-max-width: 1200px;
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

/* Tabs container constrained and centered */
.tab-list {
  display: flex;
  gap: 1rem;
  margin: 0;
  padding: 0;
  list-style: none;
  width: 100%;
  max-width: var(--content-max-width);
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
.main-content {
  display: flex;
  gap: var(--gap);
  padding: 2rem;
  width: 100%;
  max-width: var(--content-max-width);
  margin: 0 auto;
  min-height: calc(100vh - var(--header-height));
  box-sizing: border-box;
}

.left-panel,
.right-panel {
  flex: 1;
  box-sizing: border-box;
}

/* Visuals for panels */
.left-panel { background: #f8f9fa; padding: 1.5rem; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.06); }
.right-panel { background: #fff; padding: 1.5rem; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.06); border: 1px solid #e0e0e0; }

/* Responsive: stack columns on small screens */
@media (max-width: 900px) {
  .tab-list { gap: 0.5rem; overflow-x: auto; padding: 0 0.5rem; }
  .main-content { flex-direction: column; padding: 1rem; }
  .user-info { margin-left: 0; margin-top: 0.5rem; }
}
</style>