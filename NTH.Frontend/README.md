# NTH.Frontend

React Router browser frontend for NTH.

## Features

- ~~🚀 Server-side rendering~~
- ⚡️ Hot Module Replacement (HMR)
- 📦 Asset bundling and optimization
- 🔄 Data loading and mutations
- 🔒 TypeScript by default
- 🎉 TailwindCSS for styling
- 📖 [React Router docs](https://reactrouter.com/)

## Getting Started

### Installation

Always install the dependencies:

```bash
npm install # or
npm update
```

### Development

Start the development server with HMR:

```bash
npm run dev
```

Your application will be available at `http://localhost:5173`.

⚠️ **When debugging DO NOT USE 127.0.0.1 as host.** ⚠️  
⚠️ **Or else you will have problems with all backend static files.** ⚠️


## Building for Production

Create a production build:

```bash
npm run build
```

## Deployment

This app build generates static files. Just provide copydist.cjs a
default destination as the backend wwwroot/.

```bash
npm run copybuild # copy the assets to NTH backend
```

### ~~Docker Deployment~~

To build and run using Docker:

```bash
docker build -t my-app .

# Run the container
docker run -p 3000:3000 my-app
```

## Styling

This template comes with [Tailwind CSS](https://tailwindcss.com/) already configured for a simple default starting experience. You can use whatever CSS framework you prefer.

and

added some [Font Awesome icons](https://fontawesome.com/icons) as SVG

---

Built with ❤️ using React Router.
