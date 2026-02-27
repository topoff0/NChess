import { createBrowserRouter } from "react-router-dom";
import { routes } from "./routes";
import { HomePage } from "../../pages/HomePage";
import { AuthPage } from "../../pages/AuthPage";
import { ProtectedRoute } from "./ProtectedRoute";
import { CreateAccountPage } from "../../pages/CreateAccountPage";

export const router = createBrowserRouter([
  {
    path: routes.home,
    element:
      <HomePage />
  },
  {
    path: routes.auth,
    element:
      <AuthPage />
  },
  {
    element:
      <ProtectedRoute />,
    children: [
      {
        path: routes.createAccount,
        element:
          <CreateAccountPage />
      }
    ],
  }
])
