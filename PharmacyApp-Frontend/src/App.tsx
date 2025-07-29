import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext.jsx";
import PrivateRoute from "./components/PrivateRoute.jsx";
import MainLayout from "./components/MainLayout.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import DashboardPage from "./pages/DashboardPage.jsx";
import DispensePage from "./pages/DispensePage.jsx";
import PatientsPage from "./pages/PatientsPage.jsx";
import MedicationsPage from "./pages/MedicationsPage.jsx";
import InventoryPage from "./pages/InventoryPage.jsx";
import DepartmentsPage from "./pages/admin/DepartmentsPage.jsx";
import DoctorsPage from "./pages/admin/DoctorsPage.jsx";
import UsersPage from "./pages/admin/UsersPage.jsx";
import ReceiveOrderPage from "./pages/ReceiveOrderPage.jsx";
import NotFound from "./pages/NotFound";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <AuthProvider>
      <TooltipProvider>
        <Toaster />
        <Sonner />
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            
            <Route path="/" element={
              <PrivateRoute>
                <MainLayout />
              </PrivateRoute>
            }>
              <Route path="inventory/receive-order" element={<ReceiveOrderPage />} />
              <Route path="dashboard" element={<DashboardPage />} />
              <Route path="dispense" element={<DispensePage />} />
              <Route path="patients" element={<PatientsPage />} />
              <Route path="medications" element={<MedicationsPage />} />
              <Route path="inventory" element={<InventoryPage />} />
              <Route path="admin/departments" element={<DepartmentsPage />} />
              <Route path="admin/doctors" element={<DoctorsPage />} />
              <Route path="admin/users" element={<UsersPage />} />
            </Route>
            
            <Route path="*" element={<NotFound />} />
          </Routes>
        </BrowserRouter>
      </TooltipProvider>
    </AuthProvider>
  </QueryClientProvider>
);

export default App;
