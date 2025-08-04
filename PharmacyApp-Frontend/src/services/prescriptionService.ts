import apiClient from './apiClient';

// Matches the CreatePrescriptionDTO on the backend
export interface PrescriptionItemPayload {
    MedicationId: number;
    Quantity: number;
}

export interface PrescriptionPayload {
    PatientId: number;
    DoctorId: number;
    DiagnosisId: number;
    UserId: number;
    PrescriptionItems: PrescriptionItemPayload[];
}

// Matches the Sale DTO returned from the backend
export interface SaleResult {
    id: number;
    prescriptionId: number;
    totalAmount: number;
    discount: number;
    amountReceived: number;
}


const API_URL = '/api/prescriptions';

const dispensePrescription = async (prescriptionData: PrescriptionPayload): Promise<SaleResult> => {
    // Now using the global apiClient, which handles auth automatically
    const response = await apiClient.post<SaleResult>(`${API_URL}/dispense`, prescriptionData);
    return response.data;
};

export default { dispensePrescription };