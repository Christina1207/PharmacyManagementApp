import axios from 'axios';
import authService from './authService';

export interface Sale {
    id: number;
    prescriptionId: number;
    totalAmount: number;
    discount: number;
    amountReceived: number;
    pharmacistName: string;
}

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/api/Sales`;

const getSales = async (): Promise<Sale[]> => {
    const user = authService.getCurrentUser();
    const token = user?.token;
    const response = await axios.get(API_URL, {
        headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
};

const getSaleDetails = async (id: number): Promise<any> => { // Use 'any' for now, or define a detailed interface
    const user = authService.getCurrentUser();
    const token = user?.token;
    const response = await axios.get(`${API_URL}/${id}`, {
        headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
};

export default { getSales, getSaleDetails };