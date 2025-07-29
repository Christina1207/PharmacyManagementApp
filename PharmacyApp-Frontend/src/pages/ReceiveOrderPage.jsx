import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../components/ui/dialog';
import { useToast } from '../hooks/use-toast';
import supplierService from '../services/supplierService';
import medicationService from '../services/medicationService';
import orderService from '../services/orderService';
import { Plus, Search, Trash2, Truck, HardHat } from 'lucide-react';
import _ from 'lodash'; // For debouncing
const ReceiveOrderPage = () => {
    const [suppliers, setSuppliers] = useState([]);
    const [selectedSupplier, setSelectedSupplier] = useState('');
    const [orderItems, setOrderItems] = useState([]);
    const [medicationSearch, setMedicationSearch] = useState('');
    const [searchResults, setSearchResults] = useState([]);
    const [loading, setLoading] = useState(false);
    const [isSupplierDialogOpen, setIsSupplierDialogOpen] = useState(false);
    const [newSupplier, setNewSupplier] = useState({ name: '', phoneNumber: '' });
    
    const navigate = useNavigate();
    const { toast } = useToast();

    useEffect(() => {
        supplierService.getSuppliers()
            .then(setSuppliers)
            .catch(() => toast({ title: "Error", description: "Could not fetch suppliers.", variant: "destructive" }));
    }, [toast]);

    const debouncedSearch = useCallback(
        _.debounce(async (searchTerm) => {
            if (searchTerm.length > 2) {
                const results = await medicationService.searchMedications(searchTerm);
                setSearchResults(results);
            } else {
                setSearchResults([]);
            }
        }, 300),
        []
    );

    useEffect(() => {
        debouncedSearch(medicationSearch);
    }, [medicationSearch, debouncedSearch]);


    const handleAddMedicationToOrder = (med) => {
        if (!orderItems.some(item => item.medicationId === med.id)) {
            setOrderItems([...orderItems, {
                medicationId: med.id,
                medicationName: `${med.name} ${med.dose}`,
                quantity: 1,
                unitPrice: '',
                expirationDate: ''
            }]);
        }
        setMedicationSearch('');
        setSearchResults([]);
    };
    
    const handleUpdateOrderItem = (medId, field, value) => {
        setOrderItems(orderItems.map(item => 
            item.medicationId === medId ? { ...item, [field]: value } : item
        ));
    };

    const handleRemoveOrderItem = (medId) => {
        setOrderItems(orderItems.filter(item => item.medicationId !== medId));
    };
    
    const handleCreateSupplier = async (e) => {
        e.preventDefault();
        try {
            const createdSupplier = await supplierService.createSupplier(newSupplier);
            toast({ title: "Success", description: "Supplier created." });
            const updatedSuppliers = [...suppliers, createdSupplier];
            setSuppliers(updatedSuppliers);
            setSelectedSupplier(createdSupplier.id.toString());
            setIsSupplierDialogOpen(false);
            setNewSupplier({ name: '', phoneNumber: '' });
        } catch (error) {
             toast({ title: "Error", description: "Failed to create supplier.", variant: "destructive" });
        }
    };

    const handleReceiveOrder = async () => {
        if (!selectedSupplier || orderItems.length === 0) {
            toast({ title: "Validation Error", description: "Please select a supplier and add at least one item.", variant: "destructive" });
            return;
        }

        const payload = {
            supplierId: parseInt(selectedSupplier),
            orderItems: orderItems.map(({ medicationId, quantity, unitPrice, expirationDate }) => ({
                medicationId,
                quantity: parseInt(quantity),
                unitPrice: parseFloat(unitPrice),
                expirationDate
            }))
        };
        
        if (payload.orderItems.some(item => !item.quantity || !item.unitPrice || !item.expirationDate)) {
             toast({ title: "Validation Error", description: "Please fill all fields for each item.", variant: "destructive" });
             return;
        }
        
        setLoading(true);
        try {
            await orderService.createOrder(payload);
            toast({ title: "Success", description: "Order received and stock updated successfully!"});
            navigate('/inventory');
        } catch (error) {
            toast({ title: "Error", description: "Failed to process order.", variant: "destructive" });
        } finally {
            setLoading(false);
        }
    };
  return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-2xl font-bold">Receive an Order / Add Stock</h1>
                    <p className="text-muted-foreground">Log a received order to update inventory levels.</p>
                </div>
                <Button onClick={handleReceiveOrder} disabled={loading}>
                    <Plus className="mr-2 h-4 w-4" />
                    {loading ? "Processing..." : "Add to Inventory"}
                </Button>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Left Column: Supplier and Medication Search */}
                <div className="lg:col-span-1 space-y-6">
                    <Card>
                        <CardHeader>
                            <CardTitle>Supplier Details</CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <div className="flex items-center gap-2">
                                <div className="flex-grow">
                                    <Label htmlFor="supplier">Supplier</Label>
                                    <Select value={selectedSupplier} onValueChange={setSelectedSupplier}>
                                        <SelectTrigger id="supplier"><SelectValue placeholder="Select a supplier" /></SelectTrigger>
                                        <SelectContent>
                                            {suppliers.map(s => <SelectItem key={s.id} value={s.id.toString()}>{s.name}</SelectItem>)}
                                        </SelectContent>
                                    </Select>
                                </div>
                                <Dialog open={isSupplierDialogOpen} onOpenChange={setIsSupplierDialogOpen}>
                                    <DialogTrigger asChild>
                                        <Button variant="outline" size="icon" className="mt-6"><Plus className="h-4 w-4" /></Button>
                                    </DialogTrigger>
                                    <DialogContent>
                                        <DialogHeader>
                                            <DialogTitle>Add New Supplier</DialogTitle>
                                            <DialogDescription>Add a new supplier to the system.</DialogDescription>
                                        </DialogHeader>
                                        <form onSubmit={handleCreateSupplier} className="space-y-4">
                                            <div>
                                                <Label htmlFor="s-name">Supplier Name</Label>
                                                <Input id="s-name" value={newSupplier.name} onChange={e => setNewSupplier({...newSupplier, name: e.target.value})} required/>
                                            </div>
                                            <div>
                                                <Label htmlFor="s-phone">Phone Number (Optional)</Label>
                                                <Input id="s-phone" value={newSupplier.phoneNumber} onChange={e => setNewSupplier({...newSupplier, phoneNumber: e.target.value})}/>
                                            </div>
                                            <Button type="submit" className="w-full">Create Supplier</Button>
                                        </form>
                                    </DialogContent>
                                </Dialog>
                            </div>
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader><CardTitle>Add Medications</CardTitle></CardHeader>
                        <CardContent>
                            <Label htmlFor="med-search">Search by Name or Barcode</Label>
                            <div className="relative">
                                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground"/>
                                <Input id="med-search" placeholder="e.g., Amoxicillin or 123456" value={medicationSearch} onChange={e => setMedicationSearch(e.target.value)} className="pl-10"/>
                            </div>
                            {searchResults.length > 0 && (
                                <div className="mt-2 border rounded-md max-h-48 overflow-y-auto">
                                    {searchResults.map(med => (
                                        <div key={med.id} onClick={() => handleAddMedicationToOrder(med)} className="p-2 hover:bg-muted cursor-pointer">
                                            <p className="font-medium">{med.name}</p>
                                        </div>
                                    ))}
                                </div>
                            )}
                            {medicationSearch.length > 2 && searchResults.length === 0 && (
                                <div className="mt-2 text-center text-sm text-muted-foreground">
                                    <p>No medication found.</p>
                                    <Button variant="link" className="p-0 h-auto">Add as a new medication</Button>
                                </div>
                            )}
                        </CardContent>
                    </Card>
                </div>
 {/* Right Column */}
                <div className="lg:col-span-2">
                    <Card>
                        <CardHeader>
                            <CardTitle>Received Items</CardTitle>
                            <CardDescription>Enter the quantity, price from invoice, and expiration date for each item.</CardDescription>
                        </CardHeader>
                        <CardContent>
                            <div className="space-y-4">
                                {orderItems.length > 0 ? orderItems.map(item => (
                                    <div key={item.medicationId} className="grid grid-cols-1 md:grid-cols-4 gap-4 items-end border p-4 rounded-lg">
                                        <div className="md:col-span-4 font-semibold">{item.medicationName}</div>
                                        <div>
                                            <Label>Quantity</Label>
                                            <Input type="number" value={item.quantity} onChange={e => handleUpdateOrderItem(item.medicationId, 'quantity', e.target.value)} />
                                        </div>
                                        <div>
                                            <Label>Unit Price (from Invoice)</Label>
                                            <Input type="number" step="0.01" value={item.unitPrice} onChange={e => handleUpdateOrderItem(item.medicationId, 'unitPrice', e.target.value)} />
                                        </div>
                                        <div>
                                            <Label>Expiration Date</Label>
                                            <Input type="date" value={item.expirationDate} onChange={e => handleUpdateOrderItem(item.medicationId, 'expirationDate', e.target.value)} />
                                        </div>
                                        <Button variant="outline" size="icon" onClick={() => handleRemoveOrderItem(item.medicationId)}><Trash2 className="h-4 w-4"/></Button>
                                    </div>
                                )) : (
                                    <div className="text-center py-12 text-muted-foreground">
                                        <Truck className="mx-auto h-12 w-12 opacity-50"/>
                                        <p className="mt-2">Search for medications to log the received items.</p>
                                    </div>
                                )}
                            </div>
                        </CardContent>
                    </Card>
                </div>
            </div>
        </div>
    );
};

export default ReceiveOrderPage;