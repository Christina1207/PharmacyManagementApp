import axios from 'axios';
import authService from './authService';

export interface Patient {
    id: number;
    firstName: string;
    lastName: string;
    status: string;
    type: boolean; // false=Employee, true=FamilyMember
}

// AdminDashboardController has the patient endpoints
const API_URL = `${import.meta.env.VITE_API_BASE_URL}/api/Admin/AdminDashboard`;

const getAuthHeaders = () => {
    const user = authService.getCurrentUser();
    return { Authorization: `Bearer ${user?.token}` };
};

const searchPatients = async (searchTerm: string): Promise<Patient[]> => {
    // We'll fetch all and filter on the client. For a larger app, you'd add a search parameter.
    const response = await axios.get<Patient[]>(`${API_URL}/Patients`, { headers: getAuthHeaders() });
    
    if (!searchTerm) return [];
    
    const lowercasedTerm = searchTerm.toLowerCase();
    return response.data.filter(p =>
        p.firstName.toLowerCase().includes(lowercasedTerm) ||
        p.lastName.toLowerCase().includes(lowercasedTerm)
    );
};

export default { searchPatients };