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

Default API URL: `https://localhost:7155`

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

- The frontend communicates with the REST API at `https://localhost:7155`
- User authentication data is stored in localStorage
- The app uses Vue Router for navigation between pages
