import apiClient from "./apiClient";
const API_URL = '/api/diagnosis';

export interface Diagnosis {
  id: number;
  description: string;
}


// --- Service Methods ---

/**
 * [Read-Only for Pharmacist/Admin] Fetches all diagnoses for dropdowns.
 * Corresponds to: GET /api/diagnosis
 */
const getDiagnoses = async (): Promise<Diagnosis[]> => {
    const response = await apiClient.get(API_URL);
    return response.data;
};

/**
 * [Admin-Only] Creates a new diagnosis.
 * Corresponds to: POST /api/diagnosis
 */
const createDiagnosis = async (data: { description: string }): Promise<Diagnosis> => {
    const response = await apiClient.post(API_URL, data);
    return response.data;
};

/**
 * [Admin-Only] Updates an existing diagnosis.
 * Corresponds to: PUT /api/diagnosis/{id}
 */
const updateDiagnosis = async (id: number, data: { description: string }): Promise<void> => {
    await apiClient.put(`${API_URL}/${id}`, data);
};

/**
 * [Admin-Only] Deletes a diagnosis.
 * Corresponds to: DELETE /api/diagnosis/{id}
 */
const deleteDiagnosis = async (id: number): Promise<void> => {
    await apiClient.delete(`${API_URL}/${id}`);
};


export default {
    getDiagnoses,
    createDiagnosis,
    updateDiagnosis,
    deleteDiagnosis,
};