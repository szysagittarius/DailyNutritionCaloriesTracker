const API_BASE_URL = 'https://localhost:7155'; // Updated with your actual port

const api = {
  async createUser(userData) {
    const response = await fetch(`${API_BASE_URL}/User/createuser`, {
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
    const response = await fetch(`${API_BASE_URL}/User/login`, {
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
    
    // Store user info in localStorage for session management
    localStorage.setItem('user', JSON.stringify({
      username: result.username
    }));
    
    return result;
  },

  logout() {
    localStorage.removeItem('user');
  },

  getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }
};

export default api;