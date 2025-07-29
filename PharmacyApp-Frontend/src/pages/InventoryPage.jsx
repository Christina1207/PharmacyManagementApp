import React, { useState, useEffect } from 'react';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../components/ui/dialog';
import { Badge } from '../components/ui/badge';
import { useToast } from '../hooks/use-toast';
import inventoryService from '../services/inventoryService';
import { getMedications } from '../api/mockApi';
import { Package, Plus, Search, AlertTriangle, Calendar, Info, FlaskConical } from 'lucide-react';

const InventoryPage = () => {
  const [inventory, setInventory] = useState([]);
  const [medications, setMedications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isAddStockOpen, setIsAddStockOpen] = useState(false);
  const [selectedItem, setSelectedItem] = useState(null);

  const [formData, setFormData] = useState({
    medicationId: '',
    price: '',
    quantity: '',
    expirationDate: '',
  });
  const { toast } = useToast();

  const fetchData = async () => {
    setLoading(true);
    try {
      const [inventoryData, medicationsData] = await Promise.all([
        inventoryService.getInventory(),
        getMedications()
      ]);
      setInventory(inventoryData);
      setMedications(medicationsData);
    } catch (error) {
      toast({ title: "Error", description: "Failed to fetch inventory data.", variant: "destructive" });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    const stockData = {
      medicationId: parseInt(formData.medicationId),
      quantity: parseInt(formData.quantity),
      price: parseFloat(formData.price),
      expirationDate: formData.expirationDate,
    };
    try {
      await inventoryService.addStock(stockData);
      toast({ title: "Success", description: "Stock added successfully" });
      fetchData();
      setIsAddStockOpen(false);
      resetForm();
    } catch (error) {
      toast({ title: "Error", description: error.response?.data?.Message || "Failed to add stock.", variant: "destructive" });
    }
  };

  const resetForm = () => {
    setFormData({ medicationId: '', price: '', quantity: '', expirationDate: '' });
  };

  const handleMedicationChange = (medicationId) => {
    const medication = medications.find(m => m.id === parseInt(medicationId));
    if (medication) {
      setFormData({ ...formData, medicationId, price: medication.price.toString() });
    }
  };
  
  const getItemStatus = (item) => {
    if (!item) return { label: 'N/A', variant: 'secondary' };
    const isExpired = item.batches.some(batch => new Date(batch.expirationDate) < new Date());
    const isLowStock = item.totalQuantity < item.minQuantity;

    if (isExpired) return { label: 'Expired', variant: 'destructive' };
    if (isLowStock) return { label: 'Low Stock', variant: 'warning' };
    if (item.totalQuantity === 0) return { label: 'Out of Stock', variant: 'destructive' };
    return { label: 'In Stock', variant: 'success' };
  };

  // **FIXED: Safe search logic**
  const filteredInventory = inventory.filter(item => {
    const term = searchTerm.toLowerCase();
    const medName = (item.medicationName || '').toLowerCase();
    const manuName = (item.manufacturerName || '').toLowerCase(); // Safely handle null manufacturer
    return medName.includes(term) || manuName.includes(term);
  });

  if (loading) {
    return (
        <div className="flex items-center justify-center h-64">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
        </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
            <h1 className="text-2xl font-bold text-foreground">Inventory Management</h1>
            <p className="text-muted-foreground">Track medication stock levels and expiry dates</p>
        </div>
        <Dialog open={isAddStockOpen} onOpenChange={setIsAddStockOpen}>
            <DialogTrigger asChild><Button onClick={() => resetForm()}><Plus className="w-4 h-4 mr-2" />Add Stock</Button></DialogTrigger>
            <DialogContent className="max-w-lg">
                <DialogHeader><DialogTitle>Add New Stock</DialogTitle><DialogDescription>Add a new batch of medication to the inventory.</DialogDescription></DialogHeader>
                <form onSubmit={handleSubmit} className="space-y-4">{/* Form content from previous step */}</form>
            </DialogContent>
        </Dialog>
      </div>

      <div className="relative flex-1 max-w-sm">
        <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
        <Input placeholder="Search by medication or manufacturer..." value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} className="pl-10"/>
      </div>

      <Card>
        <CardHeader><CardTitle className="flex items-center space-x-2"><Package className="w-5 h-5" /><span>Inventory Stock</span></CardTitle></CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                {/* **FIXED: Removed whitespace to prevent DOM nesting warning** */}
                <tr className="border-b">
                  <th className="text-left p-3">Medication</th>
                  <th className="text-left p-3">Manufacturer</th>
                  <th className="text-left p-3">Total Quantity</th>
                  <th className="text-left p-3">Price</th>
                  <th className="text-left p-3">Status</th>
                </tr>
              </thead>
              <tbody>
                {filteredInventory.length > 0 ? (
                  filteredInventory.map((item) => {
                    const status = getItemStatus(item);
                    return (
                      <tr key={item.id} className="border-b hover:bg-muted/50 cursor-pointer" onClick={() => setSelectedItem(item)}>
                        <td className="p-3 font-medium">{item.medicationName}</td>
                        <td className="p-3 text-sm text-muted-foreground">{item.manufacturerName}</td>
                        <td className="p-3 font-mono font-medium">{item.totalQuantity}</td>
                        <td className="p-3 font-mono">${item.price.toFixed(2)}</td>
                        <td className="p-3"><Badge variant={status.variant}>{status.label}</Badge></td>
                      </tr>
                    );
                  })
                ) : (
                  <tr>
                    <td colSpan="5" className="text-center p-8 text-muted-foreground">
                      No medications found matching your search.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
      
      <Dialog open={!!selectedItem} onOpenChange={() => setSelectedItem(null)}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle className="flex items-center space-x-2">
              <FlaskConical className="w-6 h-6 text-primary"/>
              <span>{selectedItem?.medicationName}</span>
            </DialogTitle>
            <DialogDescription>
              {selectedItem?.manufacturerName} • Min Stock Level: {selectedItem?.minQuantity}
            </DialogDescription>
          </DialogHeader>
          <div className="grid grid-cols-3 gap-4 py-4 text-center">
              <div className="p-2 bg-muted rounded-md">
                  <p className="text-sm font-medium text-muted-foreground">Total Quantity</p>
                  <p className="text-2xl font-bold">{selectedItem?.totalQuantity}</p>
              </div>
              <div className="p-2 bg-muted rounded-md">
                  <p className="text-sm font-medium text-muted-foreground">Unit Price</p>
                  <p className="text-2xl font-bold">${selectedItem?.price?.toFixed(2)}</p>
              </div>
              <div className="p-2 bg-muted rounded-md">
                  <p className="text-sm font-medium text-muted-foreground">Status</p>
                  <Badge className="mt-2 text-base" variant={getItemStatus(selectedItem).variant}>
                      {getItemStatus(selectedItem).label}
                  </Badge>
              </div>
          </div>
          <div>
            <h4 className="font-semibold mb-2">Batches in Stock</h4>
            <div className="space-y-2 max-h-64 overflow-y-auto pr-2">
              {selectedItem?.batches.filter(b => b.quantity > 0).sort((a, b) => new Date(a.expirationDate) - new Date(b.expirationDate)).map(batch => (
                <div key={batch.id} className="flex justify-between items-center p-3 border rounded-md">
                  <div>
                    <span className="font-mono">Quantity: {batch.quantity}</span>
                  </div>
                  <div className="flex items-center space-x-2 text-sm">
                    <Calendar className="w-4 h-4 text-muted-foreground"/>
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