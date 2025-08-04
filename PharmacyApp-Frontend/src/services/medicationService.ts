import apiClient from './apiClient';

// --- Interfaces matching Backend DTOs ---

export interface Medication {
  id: number;
  name: string;
  barcode: string;
  dose: string;
  minQuantity: number;
  manufacturerName: string;
  formName: string;
  className: string;
  activeIngredients: {
      ingredientId: number;
      ingredientName: string;
      amount: number;
  }[];
}

// DTO for creating/updating a medication
export interface MedicationPayload {
    name: string;
    barcode: string;
    dose: string;
    minQuantity: number;
    manufacturerId: number;
    formId: number;
    classId: number;
    activeIngredients: {
        ingredientId: number;
        amount: number;
    }[];
}

// Interfaces for dropdowns
export interface Manufacturer { id: number; name: string; }
export interface MedicationForm { id: number; name: string; }
export interface MedicationClass { id: number; name: string; }
export interface ActiveIngredient { id: number; name: string; }


// --- API Route Prefix ---

const API_URL = '/api/medications'; 


// --- Service Methods ---

/**
 * [Management] Fetches ALL medications for the main management page.
 * Corresponds to: GET /api/medications
 */
const getMedications = async (): Promise<Medication[]> => {
    const response = await apiClient.get(API_URL);
    return response.data;
};

/**
 * [Search] Fetches only medications matching a search term for autocomplete.
 * Corresponds to: GET /api/medications?search=...
 */
const searchMedications = async (term: string): Promise<Medication[]> => {
    const response = await apiClient.get(`${API_URL}?search=${term}`);
    return response.data;
};

/**
 * [Management] Creates a new medication.
 * Corresponds to: POST /api/medications
 */
const createMedication = async (medicationData: MedicationPayload): Promise<Medication> => {
    const response = await apiClient.post(API_URL, medicationData);
    return response.data;
};

/**
 * [Management] Updates an existing medication.
 * Corresponds to: PUT /api/medications/{id}
 */
const updateMedication = async (id: number, medicationData: MedicationPayload): Promise<void> => {
    await apiClient.put(`${API_URL}/${id}`, medicationData);
};

/**
 * [Management] Deletes a medication.
 * Corresponds to: DELETE /api/medications/{id}
 */
const deleteMedication = async (id: number): Promise<void> => {
    await apiClient.delete(`${API_URL}/${id}`);
};


// --- Methods for Populating Form Dropdowns ---

const getManufacturers = async (): Promise<Manufacturer[]> => {
    const response = await apiClient.get('/api/manufacturers');
    return response.data;
};

const getMedicationForms = async (): Promise<MedicationForm[]> => {
    const response = await apiClient.get(`${API_URL}/Forms`);
    return response.data;
};

const getMedicationClasses = async (): Promise<MedicationClass[]> => {
    const response = await apiClient.get(`${API_URL}/Classes`); 
    return response.data;
};

const getActiveIngredients = async (): Promise<ActiveIngredient[]> => {
    const response = await apiClient.get(`${API_URL}/activeIngredients`); 
    return response.data;
};


export default {
    getMedications,
    searchMedications,
    createMedication,
    updateMedication,
    deleteMedication,
    getManufacturers,
    getMedicationForms,
    getMedicationClasses,
    getActiveIngredients,
};