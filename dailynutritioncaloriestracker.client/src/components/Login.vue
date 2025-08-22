<template>
  <div class="login-container">
    <div class="login-card">
      <h2>{{ isRegistering ? 'Register' : 'Login' }}</h2>
      
      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="username">Username:</label>
          <input 
            type="text" 
            id="username"
            v-model="username" 
            required 
            class="form-input"
          />
        </div>
        
        <div class="form-group" v-if="isRegistering">
          <label for="email">Email:</label>
          <input 
            type="email" 
            id="email"
            v-model="email" 
            required 
            class="form-input"
          />
        </div>
        
        <div class="form-group">
          <label for="password">Password:</label>
          <input 
            type="password" 
            id="password"
            v-model="password" 
            required 
            class="form-input"
          />
        </div>
        
        <div class="form-actions">
          <button type="submit" class="btn-primary" :disabled="loading">
            {{ loading ? 'Processing...' : (isRegistering ? 'Register' : 'Login') }}
          </button>
          
          <button 
            type="button" 
            @click="toggleForm" 
            class="btn-secondary"
          >
            {{ isRegistering ? 'Switch to Login' : 'Switch to Register' }}
          </button>
        </div>
      </form>
      
      <div v-if="errorMessage" class="error-message">
        {{ errorMessage }}
      </div>
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
    const email = ref('');
    const password = ref('');
    const isRegistering = ref(false);
    const loading = ref(false);
    const errorMessage = ref('');
    const router = useRouter();

    const handleSubmit = async () => {
      loading.value = true;
      errorMessage.value = '';
      
      try {
        if (isRegistering.value) {
          await api.createUser({ 
            username: username.value, 
            email: email.value, 
            password: password.value 
          });
          alert('Registration successful! Please login.');
          toggleForm();
        } else {
          await api.login({ 
            username: username.value, 
            password: password.value 
          });
          router.push('/'); // Redirect to main app
        }
      } catch (error) {
        console.error('Error during submission:', error);
        errorMessage.value = isRegistering.value 
          ? 'Registration failed. Please try again.' 
          : 'Invalid username or password.';
      } finally {
        loading.value = false;
      }
    };

    const toggleForm = () => {
      isRegistering.value = !isRegistering.value;
      username.value = '';
      email.value = '';
      password.value = '';
      errorMessage.value = '';
    };

    return {
      username,
      email,
      password,
      isRegistering,
      loading,
      errorMessage,
      handleSubmit,
      toggleForm,
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
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  width: 100%;
  max-width: 400px;
}

.login-card h2 {
  text-align: center;
  margin-bottom: 1.5rem;
  color: #333;
  font-weight: 600;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: #555;
  font-weight: 500;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e1e1e1;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.3s ease;
  box-sizing: border-box;
}

.form-input:focus {
  outline: none;
  border-color: #007bff;
}

.form-actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-top: 1.5rem;
}

.btn-primary {
  width: 100%;
  padding: 0.75rem;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.btn-primary:hover:not(:disabled) {
  background: #0056b3;
}

.btn-primary:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

.btn-secondary {
  width: 100%;
  padding: 0.75rem;
  background: transparent;
  color: #007bff;
  border: 2px solid #007bff;
  border-radius: 6px;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-secondary:hover {
  background: #007bff;
  color: white;
}

.error-message {
  margin-top: 1rem;
  padding: 0.75rem;
  background: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
  border-radius: 6px;
  text-align: center;
}
</style>