import React, { useState, useEffect } from 'react';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../components/ui/dialog';
import { Badge } from '../components/ui/badge';
import { useToast } from '../hooks/use-toast';
import inventoryService from '../services/inventoryService';
import { useNavigate } from 'react-router-dom';
import { Package, Search, Calendar, ClipboardCheck } from 'lucide-react';


const InventoryPage = () => {
    const [inventory, setInventory] = useState([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedItem, setSelectedItem] = useState(null);
    const { toast } = useToast();
    const navigate = useNavigate();

    const fetchInventory = async () => {
        try {
            const data = await inventoryService.getInventory();
            setInventory(data);
        } catch (error) {
            toast({ title: "Error", description: "Could not fetch inventory data.", variant: "destructive" });
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchInventory();
    }, []);

    const getItemStatus = (item) => {
        const isExpired = item.batches.some(batch => new Date(batch.expirationDate) < new Date());
        const isLowStock = item.totalQuantity < item.minQuantity;

        if (isExpired) return { label: 'Expired Batch', variant: 'destructive' };
        if (item.totalQuantity === 0) return { label: 'Out of Stock', variant: 'destructive' };
        if (isLowStock) return { label: 'Low Stock', variant: 'warning' };
        return { label: 'In Stock', variant: 'success' };
    };

    const filteredInventory = inventory.filter(item =>
        item.medicationName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (item.manufacturerName || '').toLowerCase().includes(searchTerm.toLowerCase())
    );


    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-2xl font-bold">Inventory Management</h1>
                    <p className="text-muted-foreground">View stock levels and expiry dates for all medications.</p>
                </div>
            </div>

            <div className="relative max-w-sm">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input placeholder="Search by medication or manufacturer..." value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} className="pl-10"/>
            </div>

            <Card>
                <CardHeader><CardTitle>Inventory Stock</CardTitle></CardHeader>
                <CardContent>
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead>
                                <tr className="border-b">
                                    <th className="text-left p-3">Medication</th>
                                    <th className="text-left p-3">Manufacturer</th>
                                    <th className="text-center p-3">Total Quantity</th>
                                    <th className="text-center p-3">Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredInventory.map((item) => (
                                    <tr key={item.id} className="border-b hover:bg-muted/50 cursor-pointer" onClick={() => setSelectedItem(item)}>
                                        <td className="p-3 font-medium">{item.medicationName}</td>
                                        <td className="p-3 text-sm text-muted-foreground">{item.manufacturerName}</td>
                                        <td className="p-3 text-center font-mono font-medium">{item.totalQuantity}</td>
                                        <td className="p-3 text-center"><Badge variant={getItemStatus(item).variant}>{getItemStatus(item).label}</Badge></td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </CardContent>
            </Card>

            <Dialog open={!!selectedItem} onOpenChange={() => setSelectedItem(null)}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>{selectedItem?.medicationName}</DialogTitle>
                        <DialogDescription>{selectedItem?.manufacturerName} • Min Stock: {selectedItem?.minQuantity}</DialogDescription>
                    </DialogHeader>
                    <div>
                        <h4 className="font-semibold mb-2 mt-4">Batches in Stock</h4>
                        <div className="space-y-2 max-h-64 overflow-y-auto pr-2">
                            {selectedItem?.batches.filter(b => b.quantity > 0).sort((a, b) => new Date(a.expirationDate) - new Date(b.expirationDate)).map(batch => (
                                <div key={batch.id} className="flex justify-between items-center p-3 border rounded-md">
                                    <span className="font-mono">Quantity: {batch.quantity}</span>
                                    <div className={`flex items-center space-x-2 text-sm ${new Date(batch.expirationDate) < new Date() ? 'text-destructive' : ''}`}>
                                        <Calendar className="w-4 h-4"/>
                                        <span>Expires: {new Date(batch.expirationDate).toLocaleDateString()}</span>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </DialogContent>
            </Dialog>
        </div>
    );
};

export default InventoryPage;