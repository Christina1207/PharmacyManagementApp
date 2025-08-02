import React, { useState, useEffect, useCallback } from 'react';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { AlertDialog, AlertDialogAction, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from '../components/ui/alert-dialog';
import { Separator } from '../components/ui/separator';
import { useToast } from '../hooks/use-toast';
import _ from 'lodash';

// --- Import Real Services ---
import patientService from '../services/patientService';
import doctorService from '../services/doctorService';
import medicationService from '../services/medicationService';
import diagnosisService from '../services/diagnosisService';
import prescriptionService from '../services/prescriptionService';

import { FileText, User, UserCheck, Plus, Trash2, Receipt, Loader2, Stethoscope, Pill, X } from 'lucide-react';

const DispensePage = () => {
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [selectedDoctor, setSelectedDoctor] = useState(null);
  const [selectedDiagnosis, setSelectedDiagnosis] = useState('');
  const [prescriptionItems, setPrescriptionItems] = useState([]);
  
  const [patientSearch, setPatientSearch] = useState('');
  const [doctorSearch, setDoctorSearch] = useState('');
  const [medicationSearch, setMedicationSearch] = useState('');
  
  const [patientSuggestions, setPatientSuggestions] = useState([]);
  const [doctorSuggestions, setDoctorSuggestions] = useState([]);
  const [medicationSuggestions, setMedicationSuggestions] = useState([]);
  const [diagnoses, setDiagnoses] = useState([]);

  const [loading, setLoading] = useState(false);
  const [saleResult, setSaleResult] = useState(null);
  const { toast } = useToast();

  useEffect(() => {
    diagnosisService.getDiagnoses()
      .then(setDiagnoses)
      .catch(() => toast({ title: "Error", description: "Could not fetch diagnoses.", variant: "destructive" }));
  }, [toast]);
  
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

  useEffect(() => { debouncedSearch(patientSearch, patientService.searchPatients, setPatientSuggestions) }, [patientSearch, debouncedSearch]);
  useEffect(() => { debouncedSearch(doctorSearch, doctorService.searchDoctors, setDoctorSuggestions) }, [doctorSearch, debouncedSearch]);
  useEffect(() => { debouncedSearch(medicationSearch, medicationService.searchMedications, setMedicationSuggestions) }, [medicationSearch, debouncedSearch]);

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
          item.medicationId === medId ? {...item, quantity: parseInt(qty) || 1} : item
      ));
  };

  const removeMedication = (medId) => {
      setPrescriptionItems(prescriptionItems.filter(item => item.medicationId !== medId));
  };

  const resetForm = () => {
      setSelectedPatient(null);
      setSelectedDoctor(null);
      setSelectedDiagnosis('');
      setPrescriptionItems([]);
      setPatientSearch('');
      setDoctorSearch('');
      setMedicationSearch('');
  };

  const handleDispense = async () => {
    if (!selectedPatient || !selectedDoctor || !selectedDiagnosis || prescriptionItems.length === 0) {
      toast({ title: "Validation Error", description: "Patient, doctor, diagnosis, and at least one medication are required.", variant: "destructive" });
      return;
    }

    setLoading(true);
    try {
      const prescriptionData = {
        patientId: selectedPatient.id,
        doctorId: selectedDoctor.id,
        diagnosisId: parseInt(selectedDiagnosis),
        prescriptionItems: prescriptionItems.map(({ medicationId, quantity }) => ({ medicationId, quantity }))
      };

      const result = await prescriptionService.dispensePrescription(prescriptionData);
      setSaleResult(result);
      resetForm();
    } catch (error) {
      toast({ title: "Error", description: error.response?.data?.message || "Dispensing failed. Check stock levels.", variant: "destructive" });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-6">
        <div>
            <h1 className="text-2xl font-bold">Prescription Dispensing</h1>
            <p className="text-muted-foreground">Process and dispense patient prescriptions</p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 items-start">
            <div className="space-y-6">
                <Card>
                    <CardHeader><CardTitle className="flex items-center space-x-2"><User className="w-5 h-5" /><span>Patient</span></CardTitle></CardHeader>
                    <CardContent>
                        {selectedPatient ? (
                             <div className="flex items-center justify-between p-3 border rounded-lg bg-muted">
                                <div className="font-semibold">{selectedPatient.firstName} {selectedPatient.lastName}</div>
                                <Button size="icon" variant="ghost" className="h-6 w-6" onClick={() => setSelectedPatient(null)}><X className="w-4 h-4" /></Button>
                            </div>
                        ) : (
                            <div>
                                <Input placeholder="Search patient name..." value={patientSearch} onChange={e => setPatientSearch(e.target.value)} />
                                {patientSuggestions.length > 0 && (
                                    <div className="mt-2 border rounded-md max-h-40 overflow-y-auto">{patientSuggestions.map(p => <div key={p.id} onClick={() => { setSelectedPatient(p); setPatientSearch(''); setPatientSuggestions([]); }} className="p-2 hover:bg-muted cursor-pointer text-sm">{p.firstName} {p.lastName}</div>)}</div>
                                )}
                            </div>
                        )}
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader><CardTitle className="flex items-center space-x-2"><UserCheck className="w-5 h-5" /><span>Doctor</span></CardTitle></CardHeader>
                    <CardContent>
                         {selectedDoctor ? (
                             <div className="flex items-center justify-between p-3 border rounded-lg bg-muted">
                                <div className="font-semibold">{selectedDoctor.firstName} {selectedDoctor.lastName} <span className="text-sm text-muted-foreground">({selectedDoctor.speciality})</span></div>
                                <Button size="icon" variant="ghost" className="h-6 w-6" onClick={() => setSelectedDoctor(null)}><X className="w-4 h-4" /></Button>
                            </div>
                        ) : (
                            <div>
                                <Input placeholder="Search doctor name..." value={doctorSearch} onChange={e => setDoctorSearch(e.target.value)} />
                                {doctorSuggestions.length > 0 && (
                                    <div className="mt-2 border rounded-md max-h-40 overflow-y-auto">{doctorSuggestions.map(d => <div key={d.id} onClick={() => { setSelectedDoctor(d); setDoctorSearch(''); setDoctorSuggestions([]); }} className="p-2 hover:bg-muted cursor-pointer text-sm">{d.firstName} {d.lastName}</div>)}</div>
                                )}
                            </div>
                        )}
                    </CardContent>
                </Card>
                
                <Card>
                    <CardHeader><CardTitle className="flex items-center space-x-2"><Stethoscope className="w-5 h-5" /><span>Diagnosis</span></CardTitle></CardHeader>
                    <CardContent>
                        <Select value={selectedDiagnosis} onValueChange={setSelectedDiagnosis}>
                            <SelectTrigger><SelectValue placeholder="Select a diagnosis" /></SelectTrigger>
                            <SelectContent>
                                {diagnoses.map(d => <SelectItem key={d.id} value={d.id.toString()}>{d.description}</SelectItem>)}
                            </SelectContent>
                        </Select>
                    </CardContent>
                </Card>
            </div>

            <div className="space-y-6">
                <Card>
                    <CardHeader><CardTitle className="flex items-center space-x-2"><Pill className="w-5 h-5" /><span>Medications</span></CardTitle></CardHeader>
                    <CardContent className="space-y-4">
                        <div>
                            <Label htmlFor="medicationSearch">Add Medication</Label>
                            <Input id="medicationSearch" placeholder="Search medication name or barcode..." value={medicationSearch} onChange={e => setMedicationSearch(e.target.value)} />
                            {medicationSuggestions.length > 0 && (
                                <div className="mt-2 border rounded-md max-h-40 overflow-y-auto">{medicationSuggestions.map(m => <div key={m.id} onClick={() => addMedication(m)} className="p-2 hover:bg-muted cursor-pointer text-sm">{m.name} {m.dose}</div>)}</div>
                            )}
                        </div>

                        <div className="space-y-3">
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
                            {prescriptionItems.length === 0 && <div className="text-center py-8 text-muted-foreground"><Plus className="w-8 h-8 mx-auto mb-2 opacity-50" /><p>No medications added</p></div>}
                        </div>
                    </CardContent>
                </Card>

                <Button onClick={handleDispense} disabled={loading} className="w-full" size="lg">
                    {loading ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Receipt className="w-5 h-5 mr-2" />}
                    Dispense Prescription
                </Button>
            </div>
        </div>
        
        <AlertDialog open={!!saleResult} onOpenChange={() => setSaleResult(null)}>
            <AlertDialogContent>
                <AlertDialogHeader>
                    <AlertDialogTitle>Dispensing Successful!</AlertDialogTitle>
                    <AlertDialogDescription>Sale #{saleResult?.id} created for prescription #{saleResult?.prescriptionId}.</AlertDialogDescription>
                </AlertDialogHeader>
                <div className="space-y-2 text-sm">
                    <div className="flex justify-between"><span>Subtotal:</span><span className="font-mono">${saleResult?.totalAmount.toFixed(2)}</span></div>
                    <div className="flex justify-between"><span>Discount (Coverage):</span><span className="font-mono text-green-600">-${saleResult?.discount.toFixed(2)}</span></div>
                    <Separator />
                    <div className="flex justify-between font-bold text-base"><span>Amount Paid by Patient:</span><span className="font-mono">${saleResult?.amountReceived.toFixed(2)}</span></div>
                </div>
                <AlertDialogFooter>
                    <AlertDialogAction onClick={() => setSaleResult(null)}>Close</AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    </div>
  );
};

export default DispensePage;