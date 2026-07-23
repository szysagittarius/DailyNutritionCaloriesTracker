# Nutrition Tracker - Vue.js Frontend

This is the Vue.js frontend for the Nutrition Tracker application built with Vite.

## Project Setup

### Install dependencies
```sh
npm install
```

### Development Server
Start the development server with hot-reload:
```sh
npm run dev
```
The app will be available at `https://localhost:5173`

### Build for Production
```sh
npm run build
```

### Lint
```sh
npm run lint
```

## Project Structure

```
src/
├── assets/          # Static assets (images, fonts, etc.)
├── components/      # Vue components
│   ├── Login.vue
│   ├── MyProfile.vue
│   ├── NutritionTracker.vue
│   ├── FoodLog.vue
│   └── ...
├── views/           # Page-level components
│   └── MainApp.vue
├── router/          # Vue Router configuration
├── services/        # API services
│   └── api.js
├── models/          # Data models/types
├── styles/          # Global styles
├── App.vue          # Root component
└── main.js          # Application entry point
```

## API Configuration

The API base URL is configured in `.env` files:
- `.env` - Default configuration
- `.env.development` - Development configuration
- `.env.production` - Production configuration

Two variables control where API calls go:

| Variable | Purpose |
|---|---|
| `VITE_API_URL` | Prepended to every API path in `api.js`. Leave **empty** to use the Vite dev-server proxy (recommended for local dev). |
| `VITE_API_TARGET` | The backend URL the Vite proxy forwards `/api/*` requests to. Change this to switch backends. |

**Current local targets:**
- REST API (default): `VITE_API_TARGET=https://localhost:7155`
- Azure Functions (alternative): `VITE_API_TARGET=http://127.0.0.1:7071`

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build locally
- `npm run lint` - Run ESLint

## Technologies

- **Vue 3** - Progressive JavaScript framework
- **Vite** - Next generation frontend tooling
- **Vue Router** - Official router for Vue.js
- **Axios** - HTTP client for API calls
- **ESLint** - Code linting

## Notes

- The frontend proxies all `/api/*` calls through the Vite dev server to the configured backend
- **REST API** (default): `https://localhost:7155` — requires SQL Server
- **Azure Functions** (alternative): `http://127.0.0.1:7071` — requires Azurite table emulator
- User authentication data is stored in localStorage
- The app uses Vue Router for navigation between pages
