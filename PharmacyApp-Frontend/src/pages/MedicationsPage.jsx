import React, { useState, useEffect } from 'react';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../components/ui/dialog';
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from '../components/ui/alert-dialog';
import { Badge } from '../components/ui/badge';
import { useToast } from '../hooks/use-toast';
import medicationService from '../services/medicationService';
import { Pill, Plus, Search, Edit, Trash2, X } from 'lucide-react';

const MedicationsPage = () => {
    const [medications, setMedications] = useState([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [editingMedication, setEditingMedication] = useState(null);

    // State for form dropdowns
    const [manufacturers, setManufacturers] = useState([]);
    const [medForms, setMedForms] = useState([]);
    const [medClasses, setMedClasses] = useState([]);
    const [activeIngredients, setActiveIngredients] = useState([]);

    const initialFormData = {
        name: '', barcode: '', dose: '', minQuantity: 10, manufacturerId: '',
        formId: '', classId: '', activeIngredients: []
    };
    const [formData, setFormData] = useState(initialFormData);
    const { toast } = useToast();

    const fetchData = async () => {
        setLoading(true);
        try {
            const [meds, mans, forms, classes, ingredients] = await Promise.all([
                medicationService.getMedications(),
                medicationService.getManufacturers(),
                medicationService.getMedicationForms(),
                medicationService.getMedicationClasses(),
                medicationService.getActiveIngredients()
            ]);
            setMedications(meds);
            setManufacturers(mans);
            setMedForms(forms);
            setMedClasses(classes);
            setActiveIngredients(ingredients);
        } catch (error) {
            toast({ title: "Error", description: "Failed to fetch required data.", variant: "destructive" });
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const handleAddNewIngredient = () => setFormData(prev => ({ ...prev, activeIngredients: [...prev.activeIngredients, { ingredientId: '', amount: '' }] }));
    const handleIngredientChange = (index, field, value) => {
        const updatedIngredients = formData.activeIngredients.map((ing, i) => i === index ? { ...ing, [field]: value } : ing);
        setFormData(prev => ({ ...prev, activeIngredients: updatedIngredients }));
    };
    const handleRemoveIngredient = (index) => setFormData(prev => ({ ...prev, activeIngredients: prev.activeIngredients.filter((_, i) => i !== index) }));

    const resetForm = () => {
        setEditingMedication(null);
        setFormData(initialFormData);
    };

    const handleEdit = (med) => {
        setEditingMedication(med);
        setFormData({
            Id: med.id,
            name: med.name,
            barcode: med.barcode,
            dose: med.dose,
            minQuantity: med.minQuantity,
            manufacturerId: manufacturers.find(m => m.name === med.manufacturerName)?.id.toString() || '',
            formId: medForms.find(f => f.name === med.formName)?.id.toString() || '',
            classId: medClasses.find(c => c.name === med.className)?.id.toString() || '',
            activeIngredients: med.activeIngredients.map(ing => ({
                ingredientId: ing.ingredientId.toString(),
                amount: ing.amount.toString()
            }))
        });
        setIsDialogOpen(true);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const payload = {
            ...formData,
            minQuantity: parseInt(formData.minQuantity),
            manufacturerId: parseInt(formData.manufacturerId),
            formId: parseInt(formData.formId),
            classId: parseInt(formData.classId),
            activeIngredients: formData.activeIngredients.map(ing => ({
                ingredientId: parseInt(ing.ingredientId),
                amount: parseFloat(ing.amount)
            }))
        };
        try {
            if (editingMedication) {
              console.log('id',editingMedication.id);
              console.log('paylod',payload);
                await medicationService.updateMedication(editingMedication.id, payload);
                toast({ title: "Success", description: "Medication updated." });
            } else {
                await medicationService.createMedication(payload);
                toast({ title: "Success", description: "Medication created." });
            }
            setIsDialogOpen(false);
            fetchData();
        } catch (error) {
            console.log('err',error);
            toast({ title: "Error", description: "Operation failed.", variant: "destructive"});
        }
    };
    
    const handleDelete = async (id) => {
        try {
            await medicationService.deleteMedication(id);
            toast({ title: "Success", description: "Medication deleted." });
            fetchData();
        } catch (error) {
            toast({ title: "Error", description: "Could not delete medication.", variant: "destructive"});
        }
    };

    const filteredMedications = medications.filter(med =>
        med.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (med.manufacturerName || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
        (med.className || '').toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <div><h1 className="text-2xl font-bold">Medication Management</h1><p className="text-muted-foreground">Manage all registered medications in the system.</p></div>
                <Dialog open={isDialogOpen} onOpenChange={(isOpen) => { if (!isOpen) resetForm(); setIsDialogOpen(isOpen); }}>
                    <DialogTrigger asChild><Button onClick={() => setIsDialogOpen(true)}><Plus className="w-4 h-4 mr-2" />Add Medication</Button></DialogTrigger>
                    <DialogContent className="max-w-3xl">
                        <DialogHeader><DialogTitle>{editingMedication ? 'Edit Medication' : 'Add New Medication'}</DialogTitle></DialogHeader>
                        <form onSubmit={handleSubmit} className="space-y-4 max-h-[70vh] overflow-y-auto pr-6">
                            {/* Form fields are broken into sections for clarity */}
                            <div className="grid grid-cols-2 gap-4">
                                <div><Label>Name</Label><Input value={formData.name} onChange={e => setFormData({...formData, name: e.target.value})} required/></div>
                                <div><Label>Dose</Label><Input value={formData.dose} onChange={e => setFormData({...formData, dose: e.target.value})} placeholder="e.g., 500mg" required/></div>
                            </div>
                            <div className="grid grid-cols-2 gap-4">
                                <div><Label>Barcode</Label><Input value={formData.barcode} onChange={e => setFormData({...formData, barcode: e.target.value})} /></div>
                                <div><Label>Minimum Stock Quantity</Label><Input type="number" value={formData.minQuantity} onChange={e => setFormData({...formData, minQuantity: e.target.value})} required/></div>
                            </div>
                            <div className="grid grid-cols-3 gap-4">
                                <div><Label>Manufacturer</Label><Select value={formData.manufacturerId} onValueChange={val => setFormData({...formData, manufacturerId: val})}><SelectTrigger><SelectValue placeholder="Select..." /></SelectTrigger><SelectContent>{manufacturers.map(m => <SelectItem key={m.id} value={m.id.toString()}>{m.name}</SelectItem>)}</SelectContent></Select></div>
                                <div><Label>Form</Label><Select value={formData.formId} onValueChange={val => setFormData({...formData, formId: val})}><SelectTrigger><SelectValue placeholder="Select..." /></SelectTrigger><SelectContent>{medForms.map(f => <SelectItem key={f.id} value={f.id.toString()}>{f.name}</SelectItem>)}</SelectContent></Select></div>
                                <div><Label>Class</Label><Select value={formData.classId} onValueChange={val => setFormData({...formData, classId: val})}><SelectTrigger><SelectValue placeholder="Select..." /></SelectTrigger><SelectContent>{medClasses.map(c => <SelectItem key={c.id} value={c.id.toString()}>{c.name}</SelectItem>)}</SelectContent></Select></div>
                            </div>
                            <div>
                                <Label>Active Ingredients</Label>
                                <div className="space-y-2">
                                    {formData.activeIngredients.map((ing, index) => (
                                        <div key={index} className="flex items-center gap-2">
                                            <Select value={ing.ingredientId} onValueChange={val => handleIngredientChange(index, 'ingredientId', val)}><SelectTrigger><SelectValue placeholder="Select Ingredient" /></SelectTrigger><SelectContent>{activeIngredients.map(ai => <SelectItem key={ai.id} value={ai.id.toString()}>{ai.name}</SelectItem>)}</SelectContent></Select>
                                            <Input placeholder="Amount (mg)" value={ing.amount} onChange={e => handleIngredientChange(index, 'amount', e.target.value)} />
                                            <Button type="button" variant="ghost" size="icon" onClick={() => handleRemoveIngredient(index)}><X className="w-4 h-4 text-destructive"/></Button>
                                        </div>
                                    ))}
                                    <Button type="button" variant="outline" size="sm" onClick={handleAddNewIngredient}><Plus className="w-4 h-4 mr-2"/>Add Ingredient</Button>
                                </div>
                            </div>
                            <div className="flex justify-end pt-4"><Button type="submit">{editingMedication ? 'Save Changes' : 'Create Medication'}</Button></div>
                        </form>
                    </DialogContent>
                </Dialog>
            </div>
            
            <div className="relative max-w-sm"><Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" /><Input placeholder="Search medications..." value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} className="pl-10" /></div>
            
            <Card>
                <CardHeader><CardTitle>Medication List</CardTitle></CardHeader>
                <CardContent>
                    <table className="w-full">
                        <thead><tr className="border-b"><th className="text-left p-2">Medication</th><th className="text-left p-2">Manufacturer</th><th className="text-left p-2">Class</th><th className="text-right p-2">Actions</th></tr></thead>
                        <tbody>
                            {filteredMedications.map((med) => (
                                <tr key={med.id} className="border-b hover:bg-muted/50">
                                    <td className="p-2"><div className="font-medium">{med.name}</div><div className="text-sm text-muted-foreground">{med.dose}</div></td>
                                    <td className="p-2 text-sm">{med.manufacturerName}</td>
                                    <td className="p-2"><Badge variant="outline">{med.className}</Badge></td>
                                    <td className="p-2 flex justify-end space-x-2">
                                        <Button size="sm" variant="outline" onClick={() => handleEdit(med)}><Edit className="w-4 h-4" /></Button>
                                        <AlertDialog>
                                            <AlertDialogTrigger asChild><Button size="sm" variant="destructive"><Trash2 className="w-4 h-4" /></Button></AlertDialogTrigger>
                                            <AlertDialogContent>
                                                <AlertDialogHeader><AlertDialogTitle>Are you sure?</AlertDialogTitle><AlertDialogDescription>This action cannot be undone.</AlertDialogDescription></AlertDialogHeader>
                                                <AlertDialogFooter><AlertDialogCancel>Cancel</AlertDialogCancel><AlertDialogAction onClick={() => handleDelete(med.id)}>Delete</AlertDialogAction></AlertDialogFooter>
                                            </AlertDialogContent>
                                        </AlertDialog>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </CardContent>
            </Card>
        </div>
    );
};

export default MedicationsPage;