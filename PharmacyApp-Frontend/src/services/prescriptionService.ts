import axios from 'axios';
import authService from './authService';

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/Prescription`;

const dispensePrescription = async (prescriptionData: any) => {
    const user = authService.getCurrentUser();
    const token = user?.token;

    const response = await axios.post(`${API_URL}/dispense`, prescriptionData, {
        headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
};

export default { dispensePrescription };