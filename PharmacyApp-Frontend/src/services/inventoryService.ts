import axios from 'axios';
import authService from './authService';

// This interface matches the GetInventoryItemDTO from the backend
export interface InventoryItem {
  id: number;
  medicationId: number;
  medicationName: string;
  price: number;
  totalQuantity: number;
  batches: {
    id: number;
    quantity: number;
    expirationDate: string;
  }[];
}

// This interface matches the AddStockDTO from the backend
export interface AddStockPayload {
  medicationId: number;
  price: number;
  quantity: number;
  expirationDate: string;
}

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/Inventory`;

const getInventory = async (): Promise<InventoryItem[]> => {
  const user = authService.getCurrentUser();
  const token = user?.token;

  const response = await axios.get(API_URL, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  return response.data;
};

const addStock = async (stockData: AddStockPayload): Promise<InventoryItem> => {
    const user = authService.getCurrentUser();
    const token = user?.token;

    const response = await axios.post(`${API_URL}/stock`, stockData, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}

const inventoryService = {
  getInventory,
  addStock
};

export default inventoryService;