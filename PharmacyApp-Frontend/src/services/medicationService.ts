import axios from 'axios';
import authService from './authService';

export interface Medication {
  id: number;
  name: string;
  dose: string;
  barcode: string;

}

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/Medications`;

const searchMedications = async (searchTerm: string): Promise<Medication[]> => {
    const user = authService.getCurrentUser();
    const token = user?.token;
    
    //  endpoint for searching
    const response = await axios.get<Medication[]>(`${API_URL}?search=${searchTerm}`, {
        headers: { Authorization: `Bearer ${token}` }
    });
    
    return response.data;
};


const createMedication = async (medicationData: any): Promise<Medication> => {
    const user = authService.getCurrentUser();
    const token = user?.token;
    const response = await axios.post<Medication>(API_URL, medicationData, {
        headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
};

export default { searchMedications, createMedication };