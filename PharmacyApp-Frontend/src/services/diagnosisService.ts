import axios from 'axios';
import authService from './authService';

export interface Diagnosis {
  id: number;
  description: string;
}

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/api/Pharmacist/PharmacistDashboard`;

const getAuthHeaders = () => {
    const user = authService.getCurrentUser();
    return { Authorization: `Bearer ${user?.token}` };
};

const getDiagnoses = async (): Promise<Diagnosis[]> => {
    const response = await axios.get(`${API_URL}/Diagnoses`, { headers: getAuthHeaders() });
    return response.data;
};

export default { getDiagnoses };