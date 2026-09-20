<template>
  <div class="admin-shell">
    <!-- Sidebar -->
    <aside class="admin-sidebar">
      <div class="admin-sidebar-brand">
        <span class="admin-sidebar-icon">🛡</span>
        <span class="admin-sidebar-title">Admin</span>
      </div>

      <nav class="admin-nav">
        <!-- Dashboard (placeholder) -->
        <button
          class="admin-nav-item"
          :class="{ active: activePage === 'dashboard' }"
          @click="activePage = 'dashboard'"
        >
          <span class="admin-nav-icon">📊</span>
          Dashboard
        </button>

        <!-- Customers group -->
        <div class="admin-nav-group">CUSTOMERS</div>
        <button
          class="admin-nav-item"
          :class="{ active: activePage === 'users' }"
          @click="activePage = 'users'"
        >
          <span class="admin-nav-icon">👥</span>
          Users
        </button>
      </nav>
    </aside>

    <!-- Main content -->
    <div class="admin-main">
      <!-- Top bar -->
      <div class="admin-topbar">
        <div class="admin-breadcrumb">
          Administration
          <span v-if="activePage !== 'dashboard'"> › {{ pageLabel }}</span>
        </div>
      </div>

      <!-- Page content -->
      <div class="admin-content">
        <!-- Dashboard placeholder -->
        <div v-if="activePage === 'dashboard'" class="admin-dashboard">
          <h2 class="admin-section-title">Dashboard</h2>
          <div class="admin-stat-cards">
            <div class="stat-card">
              <div class="stat-icon" style="background:#eff6ff;color:#3b82f6">👥</div>
              <div class="stat-body">
                <div class="stat-label">Total Users</div>
                <div class="stat-value">—</div>
              </div>
            </div>
            <div class="stat-card">
              <div class="stat-icon" style="background:#f0fdf4;color:#22c55e">🥗</div>
              <div class="stat-body">
                <div class="stat-label">Food Items</div>
                <div class="stat-value">—</div>
              </div>
            </div>
            <div class="stat-card">
              <div class="stat-icon" style="background:#fefce8;color:#eab308">📋</div>
              <div class="stat-body">
                <div class="stat-label">Food Logs</div>
                <div class="stat-value">—</div>
              </div>
            </div>
          </div>
          <p class="admin-hint">Select a section from the left sidebar to manage records.</p>
        </div>

        <!-- Users management -->
        <UsersManagement v-else-if="activePage === 'users'" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import UsersManagement from './admin/UsersManagement.vue'

const activePage = ref('users')

const pageLabel = computed(() => {
  const labels = { dashboard: 'Dashboard', users: 'Users' }
  return labels[activePage.value] || activePage.value
})
</script>

<style scoped>
/* ── Shell ───────────────────────────────────── */
.admin-shell {
  display: flex;
  min-height: calc(100vh - 64px);   /* subtract the top nav bar height */
  background: #f1f5f9;
}

/* ── Sidebar ─────────────────────────────────── */
.admin-sidebar {
  width: 220px;
  min-width: 220px;
  background: #1e293b;
  color: #cbd5e1;
  display: flex;
  flex-direction: column;
  padding-bottom: 2rem;
}

.admin-sidebar-brand {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  padding: 1.1rem 1.25rem;
  border-bottom: 1px solid #334155;
}

.admin-sidebar-icon { font-size: 1.2rem; }

.admin-sidebar-title {
  font-size: 1rem;
  font-weight: 700;
  color: #f1f5f9;
  letter-spacing: .06em;
  text-transform: uppercase;
}

.admin-nav { padding: 0.75rem 0; }

.admin-nav-group {
  padding: 0.75rem 1.25rem 0.3rem;
  font-size: 0.7rem;
  font-weight: 700;
  color: #64748b;
  letter-spacing: .1em;
  text-transform: uppercase;
}

.admin-nav-item {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  width: 100%;
  padding: 0.6rem 1.25rem;
  background: none;
  border: none;
  color: #94a3b8;
  font-size: 0.9rem;
  cursor: pointer;
  text-align: left;
  transition: background .12s, color .12s;
  border-left: 3px solid transparent;
}

.admin-nav-item:hover {
  background: #334155;
  color: #e2e8f0;
}

.admin-nav-item.active {
  background: #334155;
  color: #f1f5f9;
  border-left-color: #3b82f6;
  font-weight: 600;
}

.admin-nav-icon { font-size: 1rem; }

/* ── Main ────────────────────────────────────── */
.admin-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.admin-topbar {
  background: #fff;
  border-bottom: 1px solid #e2e8f0;
  padding: 0.65rem 1.5rem;
  display: flex;
  align-items: center;
}

.admin-breadcrumb {
  font-size: 0.825rem;
  color: #64748b;
}

.admin-content {
  flex: 1;
  padding: 1.5rem;
  overflow-y: auto;
}

/* ── Dashboard placeholder ───────────────────── */
.admin-section-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0 0 1.25rem;
}

.admin-stat-cards {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 2rem;
}

.stat-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  background: #fff;
  border-radius: 8px;
  padding: 1.1rem 1.5rem;
  min-width: 180px;
  box-shadow: 0 1px 4px rgba(0,0,0,.07);
}

.stat-icon {
  width: 44px;
  height: 44px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.3rem;
}

.stat-label {
  font-size: 0.8rem;
  color: #64748b;
  font-weight: 500;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1e293b;
}

.admin-hint {
  color: #94a3b8;
  font-size: 0.9rem;
}
</style>
