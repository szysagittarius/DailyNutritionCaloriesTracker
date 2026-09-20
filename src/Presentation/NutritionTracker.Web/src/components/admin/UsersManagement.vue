<template>
  <div class="um-page">
    <!-- Page Header -->
    <div class="um-page-header">
      <h1 class="um-page-title">Users</h1>
      <button class="um-btn um-btn-primary" @click="openCreateModal">+ Add New User</button>
    </div>

    <!-- Filter / Search bar -->
    <div class="um-toolbar">
      <input
        v-model="searchQuery"
        type="text"
        class="um-search"
        placeholder="Search by name or email..."
      />
      <span class="um-count">{{ filteredUsers.length }} user(s)</span>
    </div>

    <!-- Table -->
    <div class="um-card">
      <div v-if="isLoading" class="um-loading">Loading users...</div>
      <div v-else-if="error" class="um-error">{{ error }}</div>
      <table v-else class="um-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Cal Goal</th>
            <th>Carbs (g)</th>
            <th>Fat (g)</th>
            <th>Protein (g)</th>
            <th class="um-actions-col">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="filteredUsers.length === 0">
            <td colspan="7" class="um-empty">No users found.</td>
          </tr>
          <tr v-for="user in paginatedUsers" :key="user.id" class="um-row">
            <td>{{ user.name }}</td>
            <td class="um-email">{{ user.email }}</td>
            <td>{{ user.suggestedCalories }}</td>
            <td>{{ user.suggestedCarbs }}</td>
            <td>{{ user.suggestedFat }}</td>
            <td>{{ user.suggestedProtein }}</td>
            <td class="um-actions-col">
              <button class="um-btn um-btn-sm um-btn-edit" @click="openEditModal(user)">Edit</button>
              <button class="um-btn um-btn-sm um-btn-danger" @click="confirmDelete(user)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination -->
      <div class="um-pagination" v-if="totalPages > 1">
        <button :disabled="currentPage === 1" @click="currentPage--" class="um-btn um-btn-sm">‹ Prev</button>
        <span>Page {{ currentPage }} / {{ totalPages }}</span>
        <button :disabled="currentPage === totalPages" @click="currentPage++" class="um-btn um-btn-sm">Next ›</button>
      </div>
    </div>

    <!-- Edit / Create Modal -->
    <div v-if="showModal" class="um-modal-backdrop" @click.self="closeModal">
      <div class="um-modal">
        <div class="um-modal-header">
          <h3>{{ isEditing ? 'Edit User' : 'Add New User' }}</h3>
          <button class="um-modal-close" @click="closeModal">✕</button>
        </div>
        <div class="um-modal-body">
          <div class="um-form-row">
            <label>Name</label>
            <input v-model="form.name" type="text" class="um-input" />
          </div>
          <div class="um-form-row">
            <label>Email</label>
            <input v-model="form.email" type="email" class="um-input" />
          </div>
          <div class="um-form-row">
            <label>Password {{ isEditing ? '(leave blank to keep current)' : '*' }}</label>
            <input v-model="form.password" type="password" class="um-input"
              :placeholder="isEditing ? 'Leave blank to keep current (min 6 chars if changing)' : 'Min 6 characters'" />
          </div>
          <div class="um-form-grid">
            <div class="um-form-row">
              <label>Calorie Goal</label>
              <input v-model.number="form.suggestedCalories" type="number" class="um-input" />
            </div>
            <div class="um-form-row">
              <label>Carbs Goal (g)</label>
              <input v-model.number="form.suggestedCarbs" type="number" class="um-input" />
            </div>
            <div class="um-form-row">
              <label>Fat Goal (g)</label>
              <input v-model.number="form.suggestedFat" type="number" class="um-input" />
            </div>
            <div class="um-form-row">
              <label>Protein Goal (g)</label>
              <input v-model.number="form.suggestedProtein" type="number" class="um-input" />
            </div>
          </div>
          <div v-if="formError" class="um-form-error">{{ formError }}</div>
        </div>
        <div class="um-modal-footer">
          <button class="um-btn um-btn-secondary" @click="closeModal">Cancel</button>
          <button class="um-btn um-btn-primary" :disabled="isSaving" @click="saveUser">
            {{ isSaving ? 'Saving...' : (isEditing ? 'Save Changes' : 'Create User') }}
          </button>
        </div>
      </div>
    </div>

    <!-- Delete Confirm Modal -->
    <div v-if="showDeleteConfirm" class="um-modal-backdrop" @click.self="showDeleteConfirm = false">
      <div class="um-modal um-modal-sm">
        <div class="um-modal-header">
          <h3>Delete User</h3>
          <button class="um-modal-close" @click="showDeleteConfirm = false">✕</button>
        </div>
        <div class="um-modal-body">
          <p>Are you sure you want to delete <strong>{{ userToDelete?.name }}</strong>?<br/>This action cannot be undone.</p>
        </div>
        <div class="um-modal-footer">
          <button class="um-btn um-btn-secondary" @click="showDeleteConfirm = false">Cancel</button>
          <button class="um-btn um-btn-danger" :disabled="isSaving" @click="deleteUser">
            {{ isSaving ? 'Deleting...' : 'Delete' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const users = ref([])
const isLoading = ref(false)
const error = ref('')
const searchQuery = ref('')
const currentPage = ref(1)
const itemsPerPage = 10

const showModal = ref(false)
const isEditing = ref(false)
const isSaving = ref(false)
const formError = ref('')
const editingId = ref(null)

const showDeleteConfirm = ref(false)
const userToDelete = ref(null)

const emptyForm = () => ({
  name: '',
  email: '',
  password: '',
  suggestedCalories: 2000,
  suggestedCarbs: 250,
  suggestedFat: 65,
  suggestedProtein: 150
})

const form = ref(emptyForm())

// ── Computed ──────────────────────────────────────────────
const filteredUsers = computed(() => {
  const q = searchQuery.value.toLowerCase()
  if (!q) return users.value
  return users.value.filter(u =>
    u.name.toLowerCase().includes(q) || u.email.toLowerCase().includes(q)
  )
})

const totalPages = computed(() => Math.max(1, Math.ceil(filteredUsers.value.length / itemsPerPage)))

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  return filteredUsers.value.slice(start, start + itemsPerPage)
})

// ── API ───────────────────────────────────────────────────
const fetchUsers = async () => {
  isLoading.value = true
  error.value = ''
  try {
    const res = await fetch('/api/User')
    if (!res.ok) throw new Error(`HTTP ${res.status}`)
    const json = await res.json()
    users.value = json.data || json
  } catch (e) {
    error.value = 'Failed to load users: ' + e.message
  } finally {
    isLoading.value = false
  }
}

const saveUser = async () => {
  formError.value = ''
  if (!form.value.name || !form.value.email) {
    formError.value = 'Name and Email are required.'
    return
  }
  if (!isEditing.value) {
    // CREATE: password is required and must be ≥ 6 chars
    if (!form.value.password) {
      formError.value = 'Password is required when creating a user.'
      return
    }
    if (form.value.password.length < 6) {
      formError.value = 'Password must be at least 6 characters.'
      return
    }
  } else if (form.value.password && form.value.password.length < 6) {
    // EDIT: if a new password is provided it must also be ≥ 6 chars
    formError.value = 'New password must be at least 6 characters (or leave blank to keep current).'
    return
  }
  isSaving.value = true
  try {
    const payload = { ...form.value }
    // Don't send empty password when editing
    if (isEditing.value && !payload.password) delete payload.password

    const url = isEditing.value ? `/api/User/${editingId.value}` : '/api/User'
    const method = isEditing.value ? 'PUT' : 'POST'

    const res = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })
    if (!res.ok) {
      const txt = await res.text()
      throw new Error(txt || `HTTP ${res.status}`)
    }
    await fetchUsers()
    closeModal()
  } catch (e) {
    formError.value = 'Save failed: ' + e.message
  } finally {
    isSaving.value = false
  }
}

const deleteUser = async () => {
  if (!userToDelete.value) return
  isSaving.value = true
  try {
    const res = await fetch(`/api/User/${userToDelete.value.id}`, { method: 'DELETE' })
    if (!res.ok) throw new Error(`HTTP ${res.status}`)
    await fetchUsers()
    showDeleteConfirm.value = false
    userToDelete.value = null
  } catch (e) {
    alert('Delete failed: ' + e.message)
  } finally {
    isSaving.value = false
  }
}

// ── Modal helpers ─────────────────────────────────────────
const openCreateModal = () => {
  isEditing.value = false
  editingId.value = null
  form.value = emptyForm()
  formError.value = ''
  showModal.value = true
}

const openEditModal = (user) => {
  isEditing.value = true
  editingId.value = user.id
  form.value = {
    name: user.name,
    email: user.email,
    password: '',
    suggestedCalories: user.suggestedCalories,
    suggestedCarbs: user.suggestedCarbs,
    suggestedFat: user.suggestedFat,
    suggestedProtein: user.suggestedProtein
  }
  formError.value = ''
  showModal.value = true
}

const confirmDelete = (user) => {
  userToDelete.value = user
  showDeleteConfirm.value = true
}

const closeModal = () => {
  showModal.value = false
  formError.value = ''
}

onMounted(fetchUsers)
</script>

<style scoped>
/* ── Page layout ─────────────────────────────── */
.um-page { padding: 0; }

.um-page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.25rem;
}

.um-page-title {
  font-size: 1.6rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
}

.um-toolbar {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.um-search {
  flex: 1;
  max-width: 360px;
  padding: 0.5rem 0.85rem;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 0.9rem;
  background: #fff;
}
.um-search:focus { outline: none; border-color: #3b82f6; box-shadow: 0 0 0 3px rgba(59,130,246,.15); }

.um-count { color: #64748b; font-size: 0.875rem; }

/* ── Card / Table ────────────────────────────── */
.um-card {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 1px 4px rgba(0,0,0,.08);
  overflow: hidden;
}

.um-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.875rem;
}

.um-table thead tr {
  background: #f1f5f9;
}

.um-table th {
  padding: 0.75rem 1rem;
  text-align: left;
  font-weight: 600;
  color: #475569;
  white-space: nowrap;
  border-bottom: 1px solid #e2e8f0;
}

.um-table td {
  padding: 0.7rem 1rem;
  border-bottom: 1px solid #f1f5f9;
  color: #334155;
}

.um-row:hover { background: #f8fafc; }

.um-email { color: #64748b; }

.um-actions-col { text-align: center; width: 140px; }

.um-empty, .um-loading, .um-error {
  padding: 2rem;
  text-align: center;
  color: #94a3b8;
  font-size: 0.9rem;
}
.um-error { color: #ef4444; }

/* ── Pagination ──────────────────────────────── */
.um-pagination {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  font-size: 0.875rem;
  color: #475569;
  border-top: 1px solid #f1f5f9;
}

/* ── Buttons ─────────────────────────────────── */
.um-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.3rem;
  padding: 0.45rem 1rem;
  border: none;
  border-radius: 5px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: opacity .15s, background .15s;
}
.um-btn:disabled { opacity: .55; cursor: not-allowed; }

.um-btn-primary  { background: #3b82f6; color: #fff; }
.um-btn-primary:hover:not(:disabled)  { background: #2563eb; }

.um-btn-secondary { background: #e2e8f0; color: #475569; }
.um-btn-secondary:hover:not(:disabled) { background: #cbd5e1; }

.um-btn-danger   { background: #ef4444; color: #fff; }
.um-btn-danger:hover:not(:disabled)   { background: #dc2626; }

.um-btn-edit     { background: #f0f9ff; color: #0284c7; border: 1px solid #bae6fd; }
.um-btn-edit:hover:not(:disabled)     { background: #e0f2fe; }

.um-btn-sm { padding: 0.3rem 0.65rem; font-size: 0.8rem; margin: 0 2px; }

/* ── Modal ───────────────────────────────────── */
.um-modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}

.um-modal {
  background: #fff;
  border-radius: 10px;
  width: 100%;
  max-width: 560px;
  box-shadow: 0 20px 60px rgba(0,0,0,.25);
  overflow: hidden;
}

.um-modal-sm { max-width: 420px; }

.um-modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 1.25rem;
  border-bottom: 1px solid #e2e8f0;
  background: #f8fafc;
}

.um-modal-header h3 {
  margin: 0;
  font-size: 1.05rem;
  font-weight: 600;
  color: #1e293b;
}

.um-modal-close {
  background: none;
  border: none;
  font-size: 1.1rem;
  cursor: pointer;
  color: #94a3b8;
  padding: 0.2rem 0.4rem;
  border-radius: 4px;
}
.um-modal-close:hover { background: #f1f5f9; color: #475569; }

.um-modal-body { padding: 1.25rem; }
.um-modal-body p { margin: 0; color: #475569; line-height: 1.6; }

.um-modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.6rem;
  padding: 1rem 1.25rem;
  border-top: 1px solid #e2e8f0;
  background: #f8fafc;
}

/* ── Form ────────────────────────────────────── */
.um-form-row {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
  margin-bottom: 1rem;
}

.um-form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0 1rem;
}

.um-form-row label {
  font-size: 0.8rem;
  font-weight: 600;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: .04em;
}

.um-input {
  padding: 0.5rem 0.75rem;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 0.9rem;
  background: #fff;
  transition: border-color .15s;
}
.um-input:focus { outline: none; border-color: #3b82f6; box-shadow: 0 0 0 3px rgba(59,130,246,.12); }

.um-form-error {
  color: #ef4444;
  font-size: 0.85rem;
  margin-top: 0.25rem;
  padding: 0.5rem 0.75rem;
  background: #fef2f2;
  border-radius: 5px;
  border: 1px solid #fecaca;
}
</style>
