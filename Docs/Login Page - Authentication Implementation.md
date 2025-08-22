# Daily Nutrition Tracker - Authentication Implementation Guide

## Overview
This guide outlines how to add user authentication (login/register) to the existing Daily Nutrition Tracker Vue.js application.

## 🔧 Backend Changes

### 1. Create LoginDto Model
Create a new model to handle login requests:

```csharp
namespace NutritionTracker.Api.Models
{
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
```

### 2. UserController Login Endpoint
The `[HttpPost("login")]` endpoint already exists in UserController.cs and:
- Validates user credentials against the database
- Returns username on successful login
- Provides appropriate error responses

## 🎯 Frontend Implementation

### 1. Why Vue Router is Required

**Current Problem:** The app loads directly into the nutrition tracker with no authentication flow.

**Vue Router Solution:**
- **Route Protection**: Automatically redirect unauthenticated users to login
- **Navigation Control**: Clean separation between login and main app views
- **Session Management**: Maintain user state across page refreshes
- **Clean URLs**: `/login` for authentication, `/` for main application

**Installation:**
```bash
npm install vue-router@4
```

### 2. Router Configuration

```javascript
import { createRouter, createWebHistory } from 'vue-router'
import Login from '../components/Login.vue'
import api from '../services/api'

const routes = [
  { path: '/login', name: 'Login', component: Login },
  { path: '/', name: 'Home', component: () => import('../App.vue'), meta: { requiresAuth: true } }
]

const router = createRouter({ history: createWebHistory(), routes })

// Authentication Guard - Automatic Redirects
router.beforeEach((to, from, next) => {
  const currentUser = api.getCurrentUser()
  if (to.meta.requiresAuth && !currentUser) {
    next('/login')  // Redirect unauthenticated users to login
  } else if (to.path === '/login' && currentUser) {
    next('/')       // Redirect authenticated users to main app
  } else {
    next()          // Allow navigation
  }
})

export default router
```

### 3. API Service Requirements

**Purpose of api.js:**
- **Centralized Communication**: Single point for all backend API calls
- **Session Management**: Handle user authentication state
- **Error Handling**: Consistent error responses across the application
- **Maintainability**: Easy to update API endpoints or authentication logic

**Key Functions:**
- `createUser()` - Calls `/User/createuser` endpoint for registration
- `login()` - Calls `/User/login` endpoint and stores user session
- `logout()` - Clears user session from localStorage
- `getCurrentUser()` - Retrieves current user authentication status

**Implementation:** (Already created in api.js)

### 4. Update Application Entry Point

```javascript
import './assets/main.css'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'  // NEW: Add router

createApp(App).use(router).mount('#app')  // NEW: Use router
```

### 5. Login Component

> 🔐 **Login Component Information**  
> Simple form that toggles between registration and login modes. Uses `api.createUser()` for registration and `api.login()` for authentication.

![Login Component](https://img.shields.io/badge/Component-Login%20Form-667eea?style=for-the-badge&logo=vue.js)

```vue
<template>
  <div class="login-container">
    <div class="login-card">
      <h2>{{ isRegistering ? 'Register' : 'Login' }}</h2>
      <form @submit.prevent="handleSubmit">
        <input v-model="username" placeholder="Username" required />
        <input v-if="isRegistering" v-model="email" type="email" placeholder="Email" required />
        <input v-model="password" type="password" placeholder="Password" required />
        <button type="submit">{{ isRegistering ? 'Register' : 'Login' }}</button>
        <button type="button" @click="toggleForm">
          {{ isRegistering ? 'Switch to Login' : 'Switch to Register' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'

export default {
  setup() {
    const router = useRouter()
    const username = ref(''), email = ref(''), password = ref('')
    const isRegistering = ref(false)

    const handleSubmit = async () => {
      try {
        if (isRegistering.value) {
          await api.createUser({ username: username.value, email: email.value, password: password.value })
          alert('Registration successful!')
          isRegistering.value = false
        } else {
          await api.login({ username: username.value, password: password.value })
          router.push('/') // Redirect to main app
        }
      } catch (error) {
        alert('Error: ' + error.message)
      }
    }

    const toggleForm = () => { isRegistering.value = !isRegistering.value }

    return { username, email, password, isRegistering, handleSubmit, toggleForm }
  }
}
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}
.login-card {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  max-width: 400px;
}
</style>
```

### 6. Update Main Application

> 🏠 **Main App Authentication Wrapper**  
> Wraps existing nutrition tracker with authentication. Shows login when needed, otherwise displays main app with welcome message and logout.

![Main App](https://img.shields.io/badge/Component-Main%20App%20Wrapper-fcb69f?style=for-the-badge&logo=vue.js)

```vue
<script setup>
// EXISTING imports
import NutritionTracker from './components/NutritionTracker.vue'
import TheWelcome from './components/TheWelcome.vue'
import FoodLog from './components/FoodLog.vue'
import { ref } from 'vue'

// NEW authentication imports
import { useRouter } from 'vue-router'
import api from './services/api'

// EXISTING functionality (unchanged)
const foodLogKey = ref(0)
const activeTab = ref('home')
const handleFoodLogSubmitted = () => { foodLogKey.value++ }
const setActiveTab = (tab) => { activeTab.value = tab }

// NEW authentication functionality
const router = useRouter()
const currentUser = ref(api.getCurrentUser())
const handleLogout = () => {
  api.logout()
  router.push('/login')
}
</script>

<template>
  <div id="app">
    <!-- NEW: Route-based rendering -->
    <router-view v-if="$route.path === '/login'" />
    
    <!-- EXISTING: Main app (now wrapped with authentication) -->
    <div v-else>
      <header>
        <nav class="menu-bar">
          <ul class="tab-list">
            <!-- EXISTING navigation tabs -->
            <li class="tab-item" :class="{ active: activeTab === 'home' }" @click="setActiveTab('home')">Home</li>
            <li class="tab-item" :class="{ active: activeTab === 'nutrition' }" @click="setActiveTab('nutrition')">Nutrition</li>
            
            <!-- NEW: User authentication section -->
            <li class="user-info">
              <span>Welcome, {{ currentUser?.username }}!</span>
              <button @click="handleLogout" class="logout-btn">Logout</button>
            </li>
          </ul>
        </nav>
      </header>

      <!-- EXISTING: Main content (unchanged) -->
      <main>
        <TheWelcome />
        <div class="main-content">
          <div class="left-panel">
            <NutritionTracker @food-log-submitted="handleFoodLogSubmitted" />
          </div>
          <div class="right-panel">
            <FoodLog :key="foodLogKey" />
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<style>
/* EXISTING styles remain unchanged */

/* NEW authentication styles */
.user-info { 
  margin-left: auto; 
  display: flex; 
  align-items: center;
  gap: 1rem; 
}
.logout-btn { 
  padding: 0.5rem 1rem; 
  background: #dc3545; 
  color: white; 
  border: none; 
  border-radius: 4px; 
  cursor: pointer;
}
.logout-btn:hover { 
  background: #c82333; 
}
</style>
```

## 🔄 Authentication Flow

1. **Application Launch** → Router checks user authentication status
2. **Unauthenticated User** → Automatically redirected to `/login`
3. **User Login/Register** → API service handles backend communication
4. **Successful Authentication** → Session stored, redirect to main app (`/`)
5. **Authenticated User** → Main app displays with welcome message and logout option
6. **Logout Action** → Session cleared, redirect to login page

## ✅ Benefits

- **Preserves Existing Functionality**: All nutrition tracking features remain unchanged
- **Automatic Authentication Flow**: Seamless user experience with route protection
- **Session Persistence**: User remains logged in across browser sessions
- **Clean Architecture**: Clear separation between authentication and main application logic
- **Maintainable Code**: Centralized API management and consistent error handling

## 🔧 Configuration Notes

- **Backend API URL**: Currently configured for `https://localhost:7155`
- **Session Storage**: Uses browser localStorage for user session persistence
- **Route Protection**: Automatic redirects based on authentication status
- **Error Handling**: User-friendly error messages for authentication failures

## 📦 Package Management (package.json vs package-lock.json)

![Package Info](https://img.shields.io/badge/NPM-Package%20Management-cb3837?style=for-the-badge&logo=npm)

### package.json
- **Human-written manifest** for the project
- **Declares metadata**: name, version, scripts, dependencies with version ranges (e.g., `^1.2.3`, `~1.2.3`)
- **You edit this file** when adding/removing packages or changing scripts
- **Must be committed** to version control

### package-lock.json
- **Auto-generated by npm** (created/updated when you run `npm install`)
- **Locks the full dependency tree** to exact package versions, resolved URLs, integrity hashes
- **Ensures reproducible installs** across machines and CI by forcing exact versions
- **Used by `npm ci`** for fast, deterministic installs
- **Should be committed** (do not ignore)

### Key Commands:
```bash
# Install dependencies and update package-lock.json
npm install

# Install exactly what's in package-lock.json (CI/production)
npm ci

# Install Vue Router for this project
npm install vue-router@4
```

> **📝 Note**: `npm install` respects version ranges in package.json but writes resolved exact versions into package-lock.json. This ensures everyone gets the same dependency versions.
