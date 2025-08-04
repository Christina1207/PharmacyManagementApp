import React, { useState } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';
import { Button } from './ui/button';
import { useToast } from '../hooks/use-toast';
import { Truck } from 'lucide-react';
import { Receipt } from 'lucide-react';
import {
  LayoutDashboard,
  Users,
  Pill,
  Package,
  FileText,
  Settings,
  LogOut,
  User,
  Building2,
  UserCheck,
  ChevronLeft, 
  ClipboardCheck
} from 'lucide-react';

const MainLayout = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const { toast } = useToast();
  
  // State to manage the sidebar's collapsed state
  const [isCollapsed, setIsCollapsed] = useState(false);

  const handleLogout = async () => {
    try {
      await logout();
      toast({
        title: "Logged out successfully",
        description: "You have been logged out of the system."
      });
      navigate('/login');
    } catch (error) {
      toast({
        title: "Logout failed",
        description: "There was an error logging out. Please try again.",
        variant: "destructive"
      });
    }
  };

  const menuItems = [
    { path: '/dashboard', label: 'Dashboard', icon: LayoutDashboard, roles: ['Admin'] },
    { path: '/patients', label: 'Patients', icon: Users, roles: ['Admin'] },
    { path: '/dispense', label: 'Dispense', icon: FileText, roles: ['Pharmacist'] },
    { path: '/sales', label: 'Sales History', icon: Receipt, roles: ['Admin', 'Pharmacist'] },
    { path: '/medications', label: 'Medications', icon: Pill, roles: ['Admin', 'Pharmacist'] },
    { path: '/inventory/receive-order', label: 'Receive Order', icon: Truck, roles: ['Admin', 'Pharmacist'] },
    { path: '/inventory', label: 'Inventory', icon: Package, roles: ['Admin', 'Pharmacist'] },
    { path: '/inventory/checks', label: 'Stock Counts', icon: ClipboardCheck, roles: ['Admin', 'Pharmacist'] },
    { path: '/admin/departments', label: 'Departments', icon: Building2, roles: ['Admin'] },
    { path: '/admin/doctors', label: 'Doctors', icon: UserCheck, roles: ['Admin'] },
    { path: '/admin/users', label: 'Users', icon: Settings, roles: ['Admin'] }
  ];

  const filteredMenuItems = menuItems.filter(item => item.roles.includes(user?.role));
  const isActive = (path) => location.pathname.startsWith(path);

  return (
    // Set a fixed screen height and prevent the whole page from scrolling
    <div className="h-screen w-full bg-secondary/50 flex overflow-hidden">
      {/* --- Sidebar --- */}
      <aside className={`bg-card border-r border-border flex flex-col transition-all duration-300 ease-in-out ${isCollapsed ? 'w-20' : 'w-64'}`}>
        
        {/* Logo */}
        <div className="p-4 border-b border-border flex items-center justify-between">
          <div className={`flex items-center space-x-2 overflow-hidden ${isCollapsed ? 'w-0' : 'w-full'}`}>
            <div className="w-8 h-8 bg-primary rounded-lg flex items-center justify-center flex-shrink-0">
              <Pill className="w-5 h-5 text-primary-foreground" />
            </div>
            <div className="transition-opacity duration-100 ease-in-out" style={{ opacity: isCollapsed ? 0 : 1 }}>
              <h1 className="text-lg font-bold text-foreground whitespace-nowrap">PharmaCare</h1>
            </div>
          </div>
        </div>

        {/* Navigation */}
        {/* This nav section will scroll internally if menu items overflow */}
        <nav className="flex-1 p-2 space-y-2 overflow-y-auto">
          {filteredMenuItems.map((item) => {
            const Icon = item.icon;
            return (
              <button
                key={item.path}
                onClick={() => navigate(item.path)}
                className={`w-full flex items-center space-x-3 px-3 py-2.5 rounded-lg text-left transition-colors ${
                  isActive(item.path)
                    ? 'bg-primary text-primary-foreground'
                    : 'text-foreground hover:bg-muted'
                } ${isCollapsed ? 'justify-center' : ''}`}
                title={isCollapsed ? item.label : ''} // Show tooltip when collapsed
              >
                <Icon className="w-5 h-5 flex-shrink-0" />
                <span className={`text-sm font-medium transition-opacity duration-200 ${isCollapsed ? 'opacity-0 w-0' : 'opacity-100'}`}>
                  {item.label}
                </span>
              </button>
            );
          })}
        </nav>

        {/* User info and Collapse Toggle (This will always be visible at the bottom) */}
        <div className="p-2 border-t border-border">
           <div className={`flex items-center p-2 rounded-lg ${isCollapsed ? 'justify-center' : 'justify-between'}`}>
             <div className={`flex items-center space-x-3 overflow-hidden transition-all duration-300 ${isCollapsed ? 'w-0 opacity-0' : 'w-auto opacity-100'}`}>
                <div className="w-8 h-8 bg-muted rounded-full flex items-center justify-center flex-shrink-0">
                  <User className="w-4 h-4 text-muted-foreground" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-foreground truncate whitespace-nowrap">
                    {user?.firstName} {user?.lastName}
                  </p>
                  <p className="text-xs text-muted-foreground whitespace-nowrap">{user?.role}</p>
                </div>
                <Button variant="ghost" size="icon" onClick={handleLogout} className="flex-shrink-0">
                    <LogOut className="w-4 h-4" />
                </Button>
            </div>
            {/* Collapse Button */}
            <Button variant="outline" size="icon" onClick={() => setIsCollapsed(!isCollapsed)} className="flex-shrink-0">
              <ChevronLeft className={`w-4 h-4 transition-transform duration-300 ${isCollapsed ? 'rotate-180' : 'rotate-0'}`} />
            </Button>
          </div>
        </div>
      </aside>

      {/* --- Main Content --- */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Header (This will always be visible at the top) */}
        <header className="bg-card border-b border-border p-2">
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-xl font-semibold text-foreground">
                {filteredMenuItems.find(item => isActive(item.path))?.label || 'Dashboard'}
              </h2>
              <p className="text-sm text-muted-foreground">
                Welcome back, {user?.firstName}
              </p>
            </div>
            <div className="flex items-center space-x-2 text-sm text-muted-foreground">
              <span>{new Date().toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })}</span>
            </div>
          </div>
        </header>

        {/* Content Area (This will scroll internally if content overflows) */}
        <main className="flex-1 p-6 overflow-y-auto">
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default MainLayout;
