<template>
  <div class="login-container">
    <div class="login-card">
      <h2 class="login-title">Login to Nutrition Tracker</h2>
      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label for="username" class="form-label">Username:</label>
          <input
            id="username"
            v-model="username"
            type="text"
            class="form-input"
            placeholder="Enter your username"
            required
          />
        </div>
        <div class="form-group">
          <label for="password" class="form-label">Password:</label>
          <input
            id="password"
            v-model="password"
            type="password"
            class="form-input"
            placeholder="Enter your password"
            required
          />
        </div>
        <button type="submit" class="login-button" :disabled="isLoading">
          {{ isLoading ? 'Logging in...' : 'Login' }}
        </button>
        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import api from '../services/api';

export default {
  name: 'Login',
  setup() {
    const username = ref('');
    const password = ref('');
    const isLoading = ref(false);
    const errorMessage = ref('');
    const router = useRouter();

    const handleLogin = async () => {
      isLoading.value = true;
      errorMessage.value = '';
      
      try {
        const result = await api.login({ 
          username: username.value, 
          password: password.value 
        });
        
        console.log('Login successful, user data stored:', result);
        
        // Force page refresh to reload App.vue with user data
        window.location.href = '/';
        
      } catch (error) {
        console.error('Error during login:', error);
        errorMessage.value = 'Invalid username or password.';
      } finally {
        isLoading.value = false;
      }
    };

    return {
      username,
      password,
      isLoading,
      errorMessage,
      handleLogin,
    };
  },
};
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.login-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  padding: 50px;
  width: 100%;
  max-width: 600px; /* This is OK - it's just for the login form */
  min-width: 350px;
}

.login-title {
  text-align: center;
  color: #333;
  margin-bottom: 30px;
  font-size: 1.8rem;
  font-weight: 600;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-label {
  font-weight: 500;
  color: #555;
  font-size: 0.95rem;
}

.form-input {
  padding: 14px 16px; /* Increased padding for desktop */
  border: 2px solid #e1e5e9;
  border-radius: 8px;
  font-size: 1rem;
  transition: all 0.3s ease;
  background-color: #f8f9fa;
}

.form-input:focus {
  outline: none;
  border-color: #667eea;
  background-color: white;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.login-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 16px 24px; /* Increased padding */
  border-radius: 8px;
  font-size: 1.1rem; /* Larger font for desktop */
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 10px;
}

.login-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.login-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.error-message {
  color: #dc3545;
  text-align: center;
  font-size: 0.9rem;
  margin-top: 10px;
  padding: 10px;
  background-color: #f8d7da;
  border: 1px solid #f5c6cb;
  border-radius: 6px;
}
</style>