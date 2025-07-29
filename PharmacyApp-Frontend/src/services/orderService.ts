import axios from 'axios';
import authService from './authService';

// Define the payload structure for creating an order
interface CreateOrderItemPayload {
    medicationId: number;
    quantity: number;
    unitPrice: number;
    expirationDate: string;
}

interface CreateOrderPayload {
    supplierId: number;
    orderItems: CreateOrderItemPayload[];
}

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/api/Orders`;

const createOrder = async (orderData: CreateOrderPayload) => {
    const user = authService.getCurrentUser();
    const token = user?.token;

    const response = await axios.post(API_URL, orderData, {
        headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
};

export default { createOrder };