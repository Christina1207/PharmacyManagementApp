import apiClient from './apiClient';

// --- Interfaces matching Backend DTOs ---

export interface InventoryCheckItemPayload {
    medicationId: number;
    countedQuantity: number;
}

export interface InventoryCheckPayload {
    notes: string;
    items: InventoryCheckItemPayload[];
}
const API_URL = '/api/inventoryChecks'

// --- Service Methods ---

/**
 * Submits a new inventory check (stock count).
 * Corresponds to: POST /api/inventory-checks
 */
const createInventoryCheck = async (payload: InventoryCheckPayload): Promise<any> => {
    const response = await apiClient.post(API_URL, payload);
    return response.data;
};

/**
 * [Admin-Only] Reconciles a completed inventory check, updating stock levels.
 * Corresponds to: POST /api/inventory-checks/{id}/reconcile
 */
const reconcileInventoryCheck = async (id: number): Promise<void> => {
    await apiClient.post(`${API_URL}/${id}/reconcile`);
};

/**
 * Fetches all past inventory checks for viewing/auditing.
 * Corresponds to: GET /api/inventory-checks
 */
const getInventoryChecks = async (): Promise<any[]> => { // Define a proper interface for GetInventoryCheckDTO
    const response = await apiClient.get(API_URL);
    return response.data;
};

const getInventoryCheckById = async (id: number): Promise<InventoryCheckPayload> => {
    const response = await apiClient.get(`${API_URL}/${id}`);
    return response.data;
};



export default {
    createInventoryCheck,
    reconcileInventoryCheck,
    getInventoryChecks,
    getInventoryCheckById,
};