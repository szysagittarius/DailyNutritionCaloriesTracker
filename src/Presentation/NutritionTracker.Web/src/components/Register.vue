<template>
  <div class="login-container">
    <div class="login-card">
      <h2 class="login-title">Create your account</h2>
      <form @submit.prevent="handleRegister" class="login-form">
        <div class="form-group">
          <label for="username" class="form-label">Username:</label>
          <input
            id="username"
            v-model="username"
            type="text"
            class="form-input"
            placeholder="Choose a username"
            required
          />
        </div>
        <div class="form-group">
          <label for="email" class="form-label">Email:</label>
          <input
            id="email"
            v-model="email"
            type="email"
            class="form-input"
            placeholder="Enter your email"
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
            placeholder="At least 6 characters"
            minlength="6"
            required
          />
        </div>
        <div class="form-group">
          <label for="confirmPassword" class="form-label">Confirm password:</label>
          <input
            id="confirmPassword"
            v-model="confirmPassword"
            type="password"
            class="form-input"
            placeholder="Re-enter your password"
            minlength="6"
            required
          />
        </div>
        <button type="submit" class="login-button" :disabled="isLoading">
          {{ isLoading ? 'Creating account...' : 'Register' }}
        </button>
        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>
        <div v-if="successMessage" class="success-message">
          {{ successMessage }}
        </div>
      </form>
      <p class="switch-link">
        Already have an account?
        <router-link to="/login">Log in</router-link>
      </p>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import api from '../services/api';

export default {
  name: 'Register',
  setup() {
    const username = ref('');
    const email = ref('');
    const password = ref('');
    const confirmPassword = ref('');
    const isLoading = ref(false);
    const errorMessage = ref('');
    const successMessage = ref('');
    const router = useRouter();

    const handleRegister = async () => {
      errorMessage.value = '';
      successMessage.value = '';

      if (password.value !== confirmPassword.value) {
        errorMessage.value = 'Passwords do not match.';
        return;
      }

      isLoading.value = true;

      try {
        await api.createUser({
          username: username.value,
          email: email.value,
          password: password.value,
        });

        successMessage.value = 'Account created! Redirecting to login...';
        setTimeout(() => router.push('/login'), 1200);
      } catch (error) {
        console.error('Error during registration:', error);
        errorMessage.value = 'Could not create account. Please try a different username/email.';
      } finally {
        isLoading.value = false;
      }
    };

    return {
      username,
      email,
      password,
      confirmPassword,
      isLoading,
      errorMessage,
      successMessage,
      handleRegister,
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
  max-width: 600px;
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
  padding: 14px 16px;
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
  padding: 16px 24px;
  border-radius: 8px;
  font-size: 1.1rem;
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

.success-message {
  color: #155724;
  text-align: center;
  font-size: 0.9rem;
  margin-top: 10px;
  padding: 10px;
  background-color: #d4edda;
  border: 1px solid #c3e6cb;
  border-radius: 6px;
}

.switch-link {
  text-align: center;
  margin-top: 20px;
  color: #555;
  font-size: 0.95rem;
}

.switch-link a {
  color: #667eea;
  font-weight: 600;
  text-decoration: none;
}

.switch-link a:hover {
  text-decoration: underline;
}
</style>
