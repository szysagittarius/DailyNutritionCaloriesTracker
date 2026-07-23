// API Base URL. Leave empty to use the Vite dev server proxy.
const API_BASE_URL = import.meta.env.VITE_API_URL ?? '';

const buildUrl = (path) => `${API_BASE_URL}${path}`;

const api = {
  // ==================== User Management ====================
  async createUser(userData) {
    const response = await fetch(buildUrl('/api/users'), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        name: userData.username,
        email: userData.email,
        password: userData.password
      }),
    });
    
    if (!response.ok) {
      throw new Error('Failed to create user');
    }
    
    return response.json();
  },

  async login(loginData) {
    const response = await fetch(buildUrl('/api/user/login'), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        username: loginData.username,
        password: loginData.password
      }),
    });
    
    if (!response.ok) {
      throw new Error('Failed to login');
    }
    
    const result = await response.json();
    
    console.log('=== BACKEND LOGIN RESPONSE ===');
    console.log('Backend response:', result);
    console.log('result.id:', result.id);
    console.log('result.username:', result.username);
    console.log('=== END BACKEND RESPONSE ===');
    
    // Store user info in localStorage INCLUDING the ID
    if (result.id) {
      const userToStore = {
        id: result.id,
        userId: result.id,
        username: result.username
      };
      
      localStorage.setItem('user', JSON.stringify(userToStore));
      console.log('✅ Stored user data in localStorage:', userToStore);
    } else {
      console.error('❌ Backend did not return user ID!');
    }
    
    return result;
  },

  logout() {
    localStorage.removeItem('user');
  },

  getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  // ==================== Food Nutrition ====================
  async getFoodNutrition() {
    const response = await fetch(buildUrl('/api/FoodNutrition'));
    
    if (!response.ok) {
      throw new Error('Failed to fetch food nutrition data');
    }
    
    return response.json();
  },

  async addFoodNutrition(nutritionData) {
    const response = await fetch(buildUrl('/api/FoodNutrition'), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(nutritionData)
    });
    
    if (!response.ok) {
      throw new Error('Failed to add food nutrition');
    }
    
    return response.json();
  },

  // ==================== Food Log (NEW Hexagonal API) ====================
  async getFoodLogsByUser(userId) {
    const response = await fetch(buildUrl(`/api/FoodLog/user/${userId}`));
    
    if (!response.ok) {
      throw new Error('Failed to fetch food logs');
    }
    
    return response.json();
  },

  async createFoodLog(foodLogData) {
    const response = await fetch(buildUrl('/api/FoodLog'), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(foodLogData)
    });
    
    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Failed to create food log: ${errorText}`);
    }
    
    return response.json();
  },

  // ==================== Generic Methods ====================
  async get(endpoint) {
    const response = await fetch(buildUrl(endpoint));
    
    if (!response.ok) {
      throw new Error(`Failed to fetch from ${endpoint}`);
    }
    
    return response.json();
  },

  async post(endpoint, data) {
    const response = await fetch(buildUrl(endpoint), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    });
    
    if (!response.ok) {
      throw new Error(`Failed to post to ${endpoint}`);
    }
    
    return response.json();
  }
};

export default api;
