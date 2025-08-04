import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import { useToast } from '../hooks/use-toast';
import _ from 'lodash';

import medicationService from '../services/medicationService';
import inventoryCheckService from '../services/inventoryCheckService';

import { ClipboardCheck, Search, Plus, Trash2, Loader2 } from 'lucide-react';

const NewInventoryCheckPage = () => {
    const [checkItems, setCheckItems] = useState([]);
    const [notes, setNotes] = useState('');
    const [medicationSearch, setMedicationSearch] = useState('');
    const [medicationSuggestions, setMedicationSuggestions] = useState([]);
    const [loading, setLoading] = useState(false);
    
    const navigate = useNavigate();
    const { toast } = useToast();

    const debouncedSearch = useCallback(_.debounce(async (term) => {
        if (term.length > 2) {
            try {
                const results = await medicationService.searchMedications(term);
                setMedicationSuggestions(results);
            } catch (error) {
                console.error("Medication search failed:", error);
            }
        } else {
            setMedicationSuggestions([]);
        }
    }, 500), []);

    useEffect(() => {
        debouncedSearch(medicationSearch);
    }, [medicationSearch, debouncedSearch]);

    const addMedicationToCheck = (med) => {
        if (!checkItems.some(item => item.medicationId === med.id)) {
            setCheckItems([...checkItems, {
                medicationId: med.id,
                medicationName: `${med.name} ${med.dose}`,
                countedQuantity: ''
            }]);
        }
        setMedicationSearch('');
        setMedicationSuggestions([]);
    };
    
    const updateCountedQuantity = (medId, quantity) => {
        setCheckItems(checkItems.map(item => 
            item.medicationId === medId ? { ...item, countedQuantity: quantity } : item
        ));
    };

    const removeItem = (medId) => {
        setCheckItems(checkItems.filter(item => item.medicationId !== medId));
    };

    const handleSubmit = async () => {
        if (!notes || checkItems.length === 0) {
            toast({ title: "Validation Error", description: "Please provide notes and add at least one item to the count.", variant: "destructive" });
            return;
        }
        
        if (checkItems.some(item => item.countedQuantity === '' || isNaN(parseInt(item.countedQuantity)) || parseInt(item.countedQuantity) < 0)) {
            toast({ title: "Validation Error", description: "Please enter a valid, non-negative quantity for all items.", variant: "destructive" });
            return;
        }

        const payload = {
            notes,
            items: checkItems.map(({ medicationId, countedQuantity }) => ({
                medicationId,
                countedQuantity: parseInt(countedQuantity)
            }))
        };

        setLoading(true);
        try {
            await inventoryCheckService.createInventoryCheck(payload);
            toast({ title: "Success", description: "Inventory check submitted successfully."});
            navigate('/inventory'); // Navigate back to the inventory overview
        } catch (error) {
            toast({ title: "Error", description: "Failed to submit inventory check.", variant: "destructive" });
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="space-y-6 max-w-4xl mx-auto">
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-2xl font-bold">New Stock Count</h1>
                    <p className="text-muted-foreground">Search for medications and record their physical quantities.</p>
                </div>
                <Button onClick={handleSubmit} disabled={loading}>
                    {loading ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <ClipboardCheck className="mr-2 h-4 w-4" />}
                    {loading ? "Submitting..." : "Submit Count"}
                </Button>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Check Details</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                    <div>
                        <Label htmlFor="notes">Notes</Label>
                        <Textarea id="notes" placeholder="e.g., 'End of month stock count for refrigerated items'" value={notes} onChange={e => setNotes(e.target.value)} />
                    </div>
                    
                    <div className="relative">
                        <Label htmlFor="med-search">Find Medication to Count</Label>
                        <Search className="absolute left-3 bottom-3 h-4 w-4 text-muted-foreground"/>
                        <Input id="med-search" placeholder="Search by name or barcode..." value={medicationSearch} onChange={e => setMedicationSearch(e.target.value)} className="pl-10"/>
                        {medicationSuggestions.length > 0 && (
                            <div className="absolute z-10 w-full mt-1 bg-background border rounded-md shadow-lg max-h-60 overflow-y-auto">
                                {medicationSuggestions.map(med => (
                                    <div key={med.id} onClick={() => addMedicationToCheck(med)} className="p-3 hover:bg-muted cursor-pointer">
                                        <p className="font-medium">{med.name} {med.dose}</p>
                                        <p className="text-sm text-muted-foreground">{med.manufacturerName}</p>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Counted Items</CardTitle>
                    <CardDescription>Enter the physically counted quantity for each medication.</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className="space-y-4">
                        {checkItems.length > 0 ? checkItems.map(item => (
                            <div key={item.medicationId} className="flex items-center justify-between gap-4 border p-4 rounded-lg">
                                <span className="font-semibold flex-grow">{item.medicationName}</span>
                                <div className="flex items-center gap-2">
                                    <Label htmlFor={`qty-${item.medicationId}`}>Counted Qty:</Label>
                                    <Input 
                                        id={`qty-${item.medicationId}`} 
                                        type="number" 
                                        min="0"
                                        placeholder="0"
                                        value={item.countedQuantity} 
                                        onChange={e => updateCountedQuantity(item.medicationId, e.target.value)} 
                                        className="w-28"
                                    />
                                </div>
                                <Button variant="ghost" size="icon" onClick={() => removeItem(item.medicationId)}>
                                    <Trash2 className="h-4 w-4 text-destructive"/>
                                </Button>
                            </div>
                        )) : (
                            <div className="text-center py-12 text-muted-foreground">
                                <p>No medications added to this count yet.</p>
                            </div>
                        )}
                    </div>
                </CardContent>
            </Card>
        </div>
    );
};

export default NewInventoryCheckPage;