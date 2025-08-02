import axios from 'axios';
import authService from './authService';

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

// Matches the UpdateUserDTO
export interface UpdateUserPayload {
    role: string;
    isActive: boolean;
}

// Matches the RegisterDTO
export interface RegisterPayload {
    username: string;
    email: string;
    password?: string; // Optional for some operations
    firstName: string;
    lastName: string;
}

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/api/Account/Users`;

const getAuthHeaders = () => {
    const user = authService.getCurrentUser();
    return { Authorization: `Bearer ${user?.token}` };
};

const getUsers = async (): Promise<User[]> => {
    const response = await axios.get(API_URL, { headers: getAuthHeaders() });
    return response.data;
};

const activateUser = async (id: number): Promise<void> => {
    await axios.put(`${API_URL}/${id}/activate`, {}, { headers: getAuthHeaders() });
};

const deactivateUser = async (id: number): Promise<void> => {
    await axios.put(`${API_URL}/${id}/deactivate`, {}, { headers: getAuthHeaders() });
};

const resetPassword = async (id: number, newPassword: string): Promise<void> => {
    await axios.post(`${API_URL}/${id}/reset-password`, { newPassword }, { headers: getAuthHeaders() });
};

const registerAdmin = async (adminData: RegisterPayload): Promise<void> => {
    await axios.post(`${API_URL}/register-admin`, adminData, { headers: getAuthHeaders() });
};
const registerPharmacist = async (pharmacistData: RegisterPayload): Promise<void> =>{
    await axios.post(`${API_URL}/register-pharmacist`, pharmacistData,{headers:getAuthHeaders()});
}

export default {
    getUsers,
    activateUser,
    deactivateUser,
    resetPassword,
    registerAdmin,
    registerPharmacist
};