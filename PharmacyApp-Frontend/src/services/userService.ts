import axios from 'axios';
import authService from './authService';
import apiClient from './apiClient';

// Matches the UserDTO from the backend
export interface User {
  id: number;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

// Matches the RegisterDTO (to create admin ,pharmacist)
export interface RegisterPayload {
    username: string;
    email: string;
    password?: string; // Optional for some operations
    firstName: string;
    lastName: string;
}

const API_URL = '/api/admin/users'; // common prefix 


const getUsers = async (): Promise<User[]> => {
    const response = await apiClient.get(API_URL);
    return response.data;
};

const activateUser = async (id: number): Promise<void> => {
    await apiClient.put(`${API_URL}/${id}/activate`);
};

const deactivateUser = async (id: number): Promise<void> => {
    await apiClient.put(`${API_URL}/${id}/deactivate`);
};

const resetPassword = async (id: number, newPassword: string): Promise<void> => {
    await apiClient.post(`${API_URL}/${id}/reset-password`, { newPassword });
};

const registerAdmin = async (adminData: RegisterPayload): Promise<void> => {
    await apiClient.post(`${API_URL}/register-admin`, adminData);
};

const registerPharmacist = async (pharmacistData: RegisterPayload): Promise<void> => {
    await apiClient.post(`${API_URL}/register-pharmacist`, pharmacistData);
};


export default {
    getUsers,
    activateUser,
    deactivateUser,
    resetPassword,
    registerAdmin,
    registerPharmacist
};