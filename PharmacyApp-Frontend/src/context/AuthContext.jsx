import React, { createContext, useContext, useState, useEffect } from 'react';
import authService from '../services/authService'; // Use the updated service

const AuthContext = createContext();

export const useAuth = () => {
  return useContext(AuthContext);
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null); // Will store the full AuthResponse object
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const storedUser = authService.getCurrentUser();
    if (storedUser) {
      setUser(storedUser);
    }
    setLoading(false);
  }, []);

  const login = async (username, password) => {
    try {
      const response = await authService.login(username, password);
      setUser(response);
      return response; // Return the full response
    } catch (error) {
      // Axios wraps the error, let's re-throw the important part
      if (error.response && error.response.data) {
        throw new Error(error.response.data.Message || 'Login failed');
      }
      throw error;
    }
  };

  const logout = () => {
    authService.logout();
    setUser(null);
  };

  const value = {
    // Note the change here to provide user info and role directly
    user: user?.user,
    token: user?.token,
    isAuthenticated: !!user?.token,
    logout,
    login,
    loading,
  };

  return (
    <AuthContext.Provider value={value}>
      {!loading && children}
    </AuthContext.Provider>
  );
};