import axios from 'axios';
import authService from './authService';

export interface Doctor {
    id: number;
    firstName: string;
    lastName: string;
    speciality: string;
}

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/api/Admin/AdminDashboard`;

const getAuthHeaders = () => {
    const user = authService.getCurrentUser();
    return { Authorization: `Bearer ${user?.token}` };
};

const searchDoctors = async (searchTerm: string): Promise<Doctor[]> => {
    const response = await axios.get<Doctor[]>(`${API_URL}/Doctors`, { headers: getAuthHeaders() });
    
    if (!searchTerm) return [];

    const lowercasedTerm = searchTerm.toLowerCase();
    return response.data.filter(d =>
        d.firstName.toLowerCase().includes(lowercasedTerm) ||
        d.lastName.toLowerCase().includes(lowercasedTerm)
    );
};

export default { searchDoctors };