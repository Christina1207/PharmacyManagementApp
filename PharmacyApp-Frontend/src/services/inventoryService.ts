import apiClient from './apiClient';

// This interface matches the GetInventoryItemDTO from the backend
export interface InventoryItem {
  id: number;
  medicationId: number;
  medicationName: string;
  manufacturerName: string;
  minQuantity: number;
  price: number;
  totalQuantity: number;
  batches: {
    id: number;
    quantity: number;
    expirationDate: string;
  }[];
}



const getInventory = async (): Promise<InventoryItem[]> => {
    const response = await apiClient.get('/api/inventory');
    return response.data;
};

export default {
    getInventory,
};