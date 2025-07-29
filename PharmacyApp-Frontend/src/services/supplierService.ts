import axios from 'axios';
import authService from './authService';

export interface Supplier {
  id: number;
  name: string;
  phoneNumber?: string;
}

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/api/Pharmacist/PharmacistDashboard`;

const getSuppliers = async (): Promise<Supplier[]> => {
  const user = authService.getCurrentUser();
  const token = user?.token;
  const response = await axios.get(`${API_URL}/Suppliers`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return response.data;
};

const createSupplier = async (supplierData: { name: string, phoneNumber?: string }): Promise<Supplier> => {
    const user = authService.getCurrentUser();
    const token = user?.token;
    const response = await axios.post(`${API_URL}/Supplier`, supplierData, {
        headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
};

export default { getSuppliers, createSupplier };