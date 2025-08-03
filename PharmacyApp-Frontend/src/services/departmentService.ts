import apiClient from './apiClient';

// --- Interfaces matching Backend DTOs ---

// Matches GetDepartmentDTO
export interface Department {
  id: number;
  name: string;
}

// Matches CreateDepartmentDTO & UpdateDepartmentDTO
export interface DepartmentPayload {
  name: string;
}

// --- API Route Prefix ---

const API_URL = '/api/admin/departments';

// --- Service Methods ---

const getDepartments = async (): Promise<Department[]> => {
    const response = await apiClient.get(API_URL);
    return response.data;
};

const createDepartment = async (payload: DepartmentPayload): Promise<Department> => {
    const response = await apiClient.post(API_URL, payload);
    return response.data;
};

const updateDepartment = async (id: number, payload: DepartmentPayload): Promise<void> => {
    await apiClient.put(`${API_URL}/${id}`, payload);
};

const deleteDepartment = async (id: number): Promise<void> => {
    await apiClient.delete(`${API_URL}/${id}`);
};

export default {
    getDepartments,
    createDepartment,
    updateDepartment,
    deleteDepartment
};