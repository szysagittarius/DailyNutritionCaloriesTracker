# Role-Based Access Control (RBAC) — Implementation Plan

## Goal
Allow only users with the **Admin** role to see and use the Admin tab. Regular users see no trace of the admin section.

---

## 1. Database Layer

### 1a. New tables

| Table | Columns | Notes |
|---|---|---|
| `Roles` | `Id` (uniqueidentifier PK), `Name` (nvarchar, unique), `Description` (nvarchar, nullable) | Seed rows: `Admin`, `User` |
| `UserRoles` | `UserId` (FK → Users.Id), `RoleId` (FK → Roles.Id) | Composite PK, many-to-many join |

### 1b. Seed data
```sql
-- Roles
INSERT INTO dbo.Roles (Id, Name, Description) VALUES
  (NEWID(), 'Admin', 'Full access to admin panel'),
  (NEWID(), 'User',  'Standard user access');

-- Assign Demo User the Admin role
INSERT INTO dbo.UserRoles (UserId, RoleId)
SELECT U.Id, R.Id
FROM dbo.Users U, dbo.Roles R
WHERE U.Email = 'demo@nutritiontracker.local' AND R.Name = 'Admin';
```

### 1c. EF Core migration
- Add `Role` and `UserRole` entity classes
- Add `DbSet<Role>`, `DbSet<UserRole>` to `NutritionTrackerDbContext`
- Configure many-to-many in `OnModelCreating`
- Run `dotnet ef migrations add AddRoles`

---

## 2. Domain Layer (`NutritionTracker.Domain`)

### New entities
```csharp
// Role.cs
public class Role {
    public Guid Id { get; }
    public string Name { get; }
    public string? Description { get; }
}
```

### Update `User` entity
Add a read-only collection:
```csharp
public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
```

---

## 3. Application Layer (`NutritionTracker.Application`)

### Update ports
```csharp
// IUserRepository — add eager-loading overload
Task<UserDto?> GetByIdWithRolesAsync(Guid id, ...);
Task<UserDto?> GetByUsernameWithRolesAsync(string username, ...);
```

### Update `UserDto`
```csharp
public record UserDto(..., IReadOnlyList<string> Roles);
```

### New use cases
| Use Case | Purpose |
|---|---|
| `AssignRoleUseCase` | Assign a named role to a user |
| `RemoveRoleUseCase` | Remove a role from a user |
| `GetAllRolesUseCase` | List all available roles |

---

## 4. API Layer (`NutritionTracker.RestApi`)

### Update `LoginResponse`
```csharp
public class LoginResponse {
    public Guid Id { get; set; }
    public string Username { get; set; }
    public List<string> Roles { get; set; }  // ← NEW
    public string Message { get; set; }
}
```

The login endpoint already calls `GetUserByUsernameUseCase`; update it to use the new roles-aware overload and include the roles in the response.

### New `RolesController` endpoints
| Method | Route | Purpose |
|---|---|---|
| GET | `/api/Role` | List all roles |
| POST | `/api/User/{id}/roles` | Assign role to user |
| DELETE | `/api/User/{id}/roles/{roleName}` | Remove role from user |

---

## 5. Frontend (`NutritionTracker.Web`)

### 5a. Update `api.js` login storage
```js
const userToStore = {
    id: result.id,
    userId: result.id,
    username: result.username,
    roles: result.roles ?? []   // ← store roles array
}
localStorage.setItem('user', JSON.stringify(userToStore))
```

### 5b. Add `api.hasRole()` helper
```js
hasRole(role) {
    const user = this.getCurrentUser()
    return Array.isArray(user?.roles) && user.roles.includes(role)
}
```

### 5c. Guard the Admin tab in `App.vue`
```html
<!-- Only render the Admin tab when the user has the Admin role -->
<button
  v-if="isAdmin"
  class="admin-tab-btn"
  :class="{ active: activeTab === 'admin' }"
  @click="setActiveTab('admin')"
>
  ⚙ Admin
</button>
```

```js
const isAdmin = computed(() => api.hasRole('Admin'))
```

### 5d. Guard the Admin content block
```html
<div v-else-if="activeTab === 'admin' && isAdmin" class="admin-content-wrapper">
  <AdminPanel />
</div>
```

### 5e. Role management UI in `AdminPanel`
Add a new sidebar item **"Roles"** under CUSTOMERS. Clicking it opens a simple page that:
- Shows each user's current roles
- Provides a dropdown + "Assign" button to add a role
- "Remove" button per role badge

---

## 6. Implementation Order

```
Step 1  Domain  — Add Role entity, update User entity
Step 2  DB      — Add EF Core entities + migration + seed
Step 3  App     — Update UserDto, IUserRepository, login use case, new role use cases
Step 4  API     — Update LoginResponse, update login endpoint, add RolesController
Step 5  Frontend— Update api.js, guard Admin tab, add role management UI
```

---

## 7. Open Questions (decide before implementing)

| # | Question | Options |
|---|---|---|
| 1 | Should `Admin` also see regular user pages (Home, Food Log)? | Yes / Admin-only account |
| 2 | What happens if an Admin visits `/` directly when `activeTab` defaults to `home`? | Auto-redirect to admin or keep home |
| 3 | Persist roles in a `roles` column on `Users` (simpler) or keep a join table (extensible)? | Join table recommended for future |
| 4 | Should the API enforce role checks server-side (JWT / policy)? | Out of scope for now (no auth middleware yet) |

---

*Review this plan and confirm before implementation begins.*
