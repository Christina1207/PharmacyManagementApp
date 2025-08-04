import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Separator } from '../components/ui/separator';
import { useToast } from '../hooks/use-toast';
import _ from 'lodash';
import { useAuth } from '../context/AuthContext.jsx';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '../components/ui/dialog';

// --- ImportServices ---
import patientService from '../services/patientService';
import doctorService from '../services/doctorService';
import medicationService from '../services/medicationService';
import diagnosisService from '../services/diagnosisService';
import prescriptionService from '../services/prescriptionService';

import { User, UserCheck, Plus, Trash2, Receipt, Loader2, Stethoscope, Pill, X, Search } from 'lucide-react';

const DispensePage = () => {
    // state for selected entities
    const [employeeId, setEmployeeId] = useState('');
    const [employee, setEmployee] = useState(null);
    const [familyMembers, setFamilyMembers] = useState([]);
    const [selectedPatientId, setSelectedPatientId] = useState(null);
    const [selectedDoctor, setSelectedDoctor] = useState(null);
    const [selectedDiagnosis, setSelectedDiagnosis] = useState('');
    const [prescriptionItems, setPrescriptionItems] = useState([]);

    // state for search input and results
    const [doctorSearch, setDoctorSearch] = useState('');
    const [medicationSearch, setMedicationSearch] = useState('');
    const [doctorSuggestions, setDoctorSuggestions] = useState([]);
    const [medicationSuggestions, setMedicationSuggestions] = useState([]);

    // state for diagnoses
    const [diagnoses, setDiagnoses] = useState([]);
    const [isDiagnosisDialogOpen, setIsDiagnosisDialogOpen] = useState(false);
    const [newDiagnosis, setNewDiagnosis] = useState('');


    const [loading, setLoading] = useState(false);
    const { toast } = useToast();
    const { user } = useAuth();
    const navigate = useNavigate();

    // Fetch diagnoses on component mount
    const fetchDiagnoses = useCallback(async () => {
        try {
            const data = await diagnosisService.getDiagnoses();
            setDiagnoses(data);
        } catch (error) {
            toast({ title: "Error", description: "Could not fetch diagnoses.", variant: "destructive" });
        }
    }, [toast]);

    useEffect(() => {
        fetchDiagnoses();
    }, [fetchDiagnoses]);

    const debouncedSearch = useCallback(_.debounce(async (term, service, setter) => {
        if (term.length > 1) {
            try {
                const results = await service(term);
                setter(results);
            } catch (error) {
                console.error("Search failed:", error);
                setter([]);
            }
        } else {
            setter([]);
        }
    }, 300), []);

    useEffect(() => { debouncedSearch(doctorSearch, doctorService.searchDoctors, setDoctorSuggestions) }, [doctorSearch, debouncedSearch]);
    useEffect(() => { debouncedSearch(medicationSearch, medicationService.searchMedications, setMedicationSuggestions) }, [medicationSearch, debouncedSearch]);

    const handleEmployeeIdSearch = async () => {
        if (!employeeId) return;
        setEmployee(null);
        setFamilyMembers([]);
        setSelectedPatientId(null);
        try {
            const patientData = await patientService.getEmployeeDetailsById(employeeId);

            if (patientData) {
                setEmployee(patientData);
                setSelectedPatientId(patientData.id.toString());
                
                const familyMembersData = await patientService.getFamilyMembers(employeeId);
                setFamilyMembers(familyMembersData || []);
            } else {
                toast({ title: "Not an Employee", description: "The ID entered does not belong to an employee.", variant: "destructive" });
            }
        } catch (error) {
            toast({ title: "Error", description: "Patient not found.", variant: "destructive" });
            setEmployee(null);
            setFamilyMembers([]);
        }
    };

    const addMedication = (medication) => {
        if (!prescriptionItems.some(item => item.medicationId === medication.id)) {
            setPrescriptionItems([...prescriptionItems, {
                medicationId: medication.id,
                medicationName: `${medication.name} ${medication.dose}`,
                quantity: 1
            }]);
        }
        setMedicationSearch('');
        setMedicationSuggestions([]);
    };

    const updateQuantity = (medId, qty) => {
        setPrescriptionItems(prescriptionItems.map(item =>
            item.medicationId === medId ? { ...item, quantity: parseInt(qty) || 1 } : item
        ));
    };

    const removeMedication = (medId) => {
        setPrescriptionItems(prescriptionItems.filter(item => item.medicationId !== medId));
    };
    
    const handleCreateDiagnosis = async (e) => {
        e.preventDefault();
        try {
            const created = await diagnosisService.createDiagnosis({ description: newDiagnosis });
            toast({ title: "Success", description: "Diagnosis created." });
            fetchDiagnoses(); // Refresh list
            setSelectedDiagnosis(created.id.toString());
            setIsDiagnosisDialogOpen(false);
            setNewDiagnosis('');
        } catch (error) {
             toast({ title: "Error", description: "Failed to create diagnosis.", variant: "destructive" });
        }
    };

    const handleDispense = async () => {
        if (!selectedPatientId || !selectedDoctor || !selectedDiagnosis || prescriptionItems.length === 0) {
            toast({ title: "Validation Error", description: "Patient, doctor, diagnosis, and at least one medication are required.", variant: "destructive" });
            return;
        }

        setLoading(true);
        try {
            const prescriptionData = {
                PatientId: parseInt(selectedPatientId),
                DoctorId: selectedDoctor.id,
                DiagnosisId: parseInt(selectedDiagnosis),
                UserId: user.id,
                PrescriptionItems: prescriptionItems.map(({ medicationId, quantity }) => ({
                    MedicationId: medicationId,
                    Quantity: quantity
                }))
            };

            const result = await prescriptionService.dispensePrescription(prescriptionData);
            toast({ title: "Success", description: `Prescription #${result.prescriptionId} dispensed.` });
            navigate(`/sales/${result.id}`); // Navigate to the new receipt page

        } catch (error) {
            toast({ title: "Error", description: error.response?.data?.message || "Dispensing failed. Check stock levels.", variant: "destructive" });
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container mx-auto p-4 lg:p-6 space-y-6">
            <Card className="max-w-4xl mx-auto">
                <CardHeader>
                    <CardTitle className="text-2xl">New Prescription</CardTitle>
                    <CardDescription>Fill out the details below to dispense a new prescription.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-8">
                    {/* Patient Section */}
                    <div className="space-y-4">
                        <Label htmlFor="employeeId" className="font-semibold text-lg">Patient Information</Label>
                        <div className="flex items-center gap-2">
                            <Input
                                id="employeeId"
                                placeholder="Enter Employee ID"
                                value={employeeId}
                                onChange={(e) => setEmployeeId(e.target.value)}
                                className="max-w-xs"
                            />
                            <Button onClick={handleEmployeeIdSearch}>
                                <Search className="w-4 h-4 mr-2" /> Find
                            </Button>
                        </div>
                        {employee && (
                            <div className="p-4 border rounded-lg bg-secondary/50 space-y-3">
                                <h3 className="font-medium">{employee.firstName} {employee.lastName}</h3>
                                <Select value={selectedPatientId} onValueChange={setSelectedPatientId}>
                                    <SelectTrigger>
                                        <SelectValue placeholder="Select Patient..." />
                                    </SelectTrigger>
                                    <SelectContent>
                                        <SelectItem value={employee.id.toString()}>
                                            {employee.firstName} {employee.lastName} (Employee)
                                        </SelectItem>
                                        {familyMembers.map(member => (
                                            <SelectItem key={member.id} value={member.id.toString()}>
                                                {member.firstName} {member.lastName} ({member.relationship})
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </div>
                        )}
                    </div>

                    <Separator />

                    {/* Doctor Section */}
                    <div className="space-y-4">
                        <Label className="font-semibold text-lg">Prescribing Doctor</Label>
                        {selectedDoctor ? (
                            <div className="flex items-center justify-between p-3 border rounded-lg bg-muted">
                                <div className="font-semibold">{selectedDoctor.firstName} {selectedDoctor.lastName} <span className="text-sm text-muted-foreground">({selectedDoctor.speciality})</span></div>
                                <Button size="icon" variant="ghost" className="h-6 w-6" onClick={() => setSelectedDoctor(null)}><X className="w-4 h-4" /></Button>
                            </div>
                        ) : (
                            <div className="relative">
                                <Input placeholder="Search doctor by name..." value={doctorSearch} onChange={e => setDoctorSearch(e.target.value)} />
                                {doctorSuggestions.length > 0 && (
                                    <div className="absolute z-10 w-full mt-1 bg-background border rounded-md shadow-lg max-h-40 overflow-y-auto">
                                        {doctorSuggestions.map(d => (
                                            <div key={d.id} onClick={() => { setSelectedDoctor(d); setDoctorSearch(''); setDoctorSuggestions([]); }} className="p-3 hover:bg-muted cursor-pointer">
                                                <p className="font-medium">{d.firstName} {d.lastName}</p>
                                                <p className="text-sm text-muted-foreground">{d.speciality}</p>
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </div>
                        )}
                    </div>

                    <Separator />
                    
                    {/* Diagnosis Section */}
                    <div className="space-y-4">
                        <Label className="font-semibold text-lg">Diagnosis</Label>
                         <div className="flex items-center gap-2">
                            <Select value={selectedDiagnosis} onValueChange={setSelectedDiagnosis}>
                                <SelectTrigger><SelectValue placeholder="Select a diagnosis" /></SelectTrigger>
                                <SelectContent>
                                    {diagnoses.map(d => <SelectItem key={d.id} value={d.id.toString()}>{d.description}</SelectItem>)}
                                </SelectContent>
                            </Select>
                             <Dialog open={isDiagnosisDialogOpen} onOpenChange={setIsDiagnosisDialogOpen}>
                                <DialogTrigger asChild><Button variant="outline"><Plus className="w-4 h-4 mr-2" />New</Button></DialogTrigger>
                                <DialogContent>
                                    <DialogHeader><DialogTitle>Add New Diagnosis</DialogTitle></DialogHeader>
                                    <form onSubmit={handleCreateDiagnosis} className="space-y-4">
                                        <Input placeholder="Diagnosis description..." value={newDiagnosis} onChange={e => setNewDiagnosis(e.target.value)} required />
                                        <Button type="submit" className="w-full">Create Diagnosis</Button>
                                    </form>
                                </DialogContent>
                            </Dialog>
                        </div>
                    </div>

                    <Separator />

                    {/* Medications Section */}
                    <div className="space-y-4">
                        <Label className="font-semibold text-lg">Medications</Label>
                         <div className="relative">
                            <Input placeholder="Search for medications to add..." value={medicationSearch} onChange={e => setMedicationSearch(e.target.value)} />
                             {medicationSuggestions.length > 0 && (
                                <div className="absolute z-10 w-full mt-1 bg-background border rounded-md shadow-lg max-h-40 overflow-y-auto">
                                    {medicationSuggestions.map(m => (
                                        <div key={m.id} onClick={() => addMedication(m)} className="p-3 hover:bg-muted cursor-pointer">
                                            <p className="font-medium">{m.name} {m.dose}</p>
                                            <p className="text-sm text-muted-foreground">{m.manufacturerName}</p>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                        
                        <div className="space-y-3 pt-4">
                            {prescriptionItems.map(item => (
                                <div key={item.medicationId} className="flex items-center justify-between p-3 border rounded-lg">
                                    <span className="font-medium text-sm">{item.medicationName}</span>
                                    <div className="flex items-center gap-2">
                                        <Label htmlFor={`qty-${item.medicationId}`} className="text-sm">Qty:</Label>
                                        <Input id={`qty-${item.medicationId}`} type="number" min="1" value={item.quantity} onChange={(e) => updateQuantity(item.medicationId, e.target.value)} className="w-20 h-8" />
                                        <Button size="icon" variant="ghost" className="h-8 w-8" onClick={() => removeMedication(item.medicationId)}><Trash2 className="w-4 h-4 text-destructive" /></Button>
                                    </div>
                                </div>
                            ))}
                            {prescriptionItems.length === 0 && <div className="text-center py-8 text-muted-foreground"><Pill className="w-8 h-8 mx-auto mb-2 opacity-50" /><p>No medications added</p></div>}
                        </div>
                    </div>
                    
                    <Separator/>

                    <Button onClick={handleDispense} disabled={loading} className="w-full" size="lg">
                        {loading ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Receipt className="w-5 h-5 mr-2" />}
                        Dispense & Finalize Sale
                    </Button>
                </CardContent>
            </Card>
        </div>
    );
};

export default DispensePage;