import React from "react";
import ReactDOM from "react-dom/client";
import AppRouter from "./router/AppRouter.jsx";
import { AuthProvider } from "./auth/AuthContext.jsx";
import { AppProvider } from "./context/AppContext.jsx";
import { BrowserRouter } from "react-router-dom";
import "./App.css";
import "./index.css"
ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <BrowserRouter>
      <AuthProvider>
        <AppProvider>
          <AppRouter />
        </AppProvider>
      </AuthProvider>
    </BrowserRouter>
  </React.StrictMode>
);
