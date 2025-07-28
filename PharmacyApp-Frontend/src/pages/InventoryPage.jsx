import React, { useState, useEffect } from 'react';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../components/ui/dialog';
import { Badge } from '../components/ui/badge';
import { useToast } from '../hooks/use-toast';
import { getInventory, addStock, getMedications } from '../api/mockApi.js';
import { Package, Plus, Search, AlertTriangle, Calendar } from 'lucide-react';

const InventoryPage = () => {
  const [inventory, setInventory] = useState([]);
  const [medications, setMedications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [formData, setFormData] = useState({
    medicationId: '',
    medicationName: '',
    batchNumber: '',
    expiryDate: '',
    quantity: '',
    costPrice: '',
    sellingPrice: '',
    supplier: ''
  });
  const { toast } = useToast();

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [inventoryData, medicationsData] = await Promise.all([
        getInventory(),
        getMedications()
      ]);
      setInventory(inventoryData);
      setMedications(medicationsData);
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to fetch data",
        variant: "destructive"
      });
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    const stockData = {
      ...formData,
      quantity: parseInt(formData.quantity),
      costPrice: parseFloat(formData.costPrice),
      sellingPrice: parseFloat(formData.sellingPrice)
    };

    try {
      await addStock(stockData);
      toast({
        title: "Success",
        description: "Stock added successfully"
      });
      
      fetchData();
      setIsDialogOpen(false);
      resetForm();
    } catch (error) {
      toast({
        title: "Error",
        description: error.message,
        variant: "destructive"
      });
    }
  };

  const resetForm = () => {
    setFormData({
      medicationId: '',
      medicationName: '',
      batchNumber: '',
      expiryDate: '',
      quantity: '',
      costPrice: '',
      sellingPrice: '',
      supplier: ''
    });
  };

  const handleMedicationChange = (medicationId) => {
    const medication = medications.find(m => m.id === parseInt(medicationId));
    if (medication) {
      setFormData({
        ...formData,
        medicationId,
        medicationName: `${medication.name} ${medication.strength}`,
        sellingPrice: medication.price.toString()
      });
    }
  };

  const getStockStatus = (quantity) => {
    if (quantity === 0) return { label: 'Out of Stock', variant: 'destructive' };
    if (quantity < 20) return { label: 'Critical', variant: 'destructive' };
    if (quantity < 50) return { label: 'Low Stock', variant: 'warning' };
    return { label: 'In Stock', variant: 'success' };
  };

  const isExpiringSoon = (expiryDate) => {
    const today = new Date();
    const expiry = new Date(expiryDate);
    const daysUntilExpiry = Math.ceil((expiry - today) / (1000 * 60 * 60 * 24));
    return daysUntilExpiry <= 90 && daysUntilExpiry > 0;
  };

  const isExpired = (expiryDate) => {
    return new Date(expiryDate) < new Date();
  };

  const filteredInventory = inventory.filter(item =>
    item.medicationName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    item.batchNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
    item.supplier.toLowerCase().includes(searchTerm.toLowerCase())
  );

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
        
        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogTrigger asChild>
            <Button onClick={() => resetForm()}>
              <Plus className="w-4 h-4 mr-2" />
              Add Stock
            </Button>
          </DialogTrigger>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Add New Stock</DialogTitle>
              <DialogDescription>
                Add new stock batch to inventory
              </DialogDescription>
            </DialogHeader>
            
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <Label htmlFor="medicationId">Medication</Label>
                <Select value={formData.medicationId} onValueChange={handleMedicationChange}>
                  <SelectTrigger>
                    <SelectValue placeholder="Select medication" />
                  </SelectTrigger>
                  <SelectContent>
                    {medications.map((medication) => (
                      <SelectItem key={medication.id} value={medication.id.toString()}>
                        {medication.name} {medication.strength} - {medication.dosageForm}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="batchNumber">Batch Number</Label>
                  <Input
                    id="batchNumber"
                    value={formData.batchNumber}
                    onChange={(e) => setFormData({...formData, batchNumber: e.target.value})}
                    required
                  />
                </div>
                <div>
                  <Label htmlFor="supplier">Supplier</Label>
                  <Input
                    id="supplier"
                    value={formData.supplier}
                    onChange={(e) => setFormData({...formData, supplier: e.target.value})}
                    required
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="quantity">Quantity</Label>
                  <Input
                    id="quantity"
                    type="number"
                    value={formData.quantity}
                    onChange={(e) => setFormData({...formData, quantity: e.target.value})}
                    required
                  />
                </div>
                <div>
                  <Label htmlFor="expiryDate">Expiry Date</Label>
                  <Input
                    id="expiryDate"
                    type="date"
                    value={formData.expiryDate}
                    onChange={(e) => setFormData({...formData, expiryDate: e.target.value})}
                    required
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="costPrice">Cost Price ($)</Label>
                  <Input
                    id="costPrice"
                    type="number"
                    step="0.01"
                    value={formData.costPrice}
                    onChange={(e) => setFormData({...formData, costPrice: e.target.value})}
                    required
                  />
                </div>
                <div>
                  <Label htmlFor="sellingPrice">Selling Price ($)</Label>
                  <Input
                    id="sellingPrice"
                    type="number"
                    step="0.01"
                    value={formData.sellingPrice}
                    onChange={(e) => setFormData({...formData, sellingPrice: e.target.value})}
                    required
                  />
                </div>
              </div>

              <div className="flex justify-end space-x-2">
                <Button type="button" variant="outline" onClick={() => setIsDialogOpen(false)}>
                  Cancel
                </Button>
                <Button type="submit">Add Stock</Button>
              </div>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      {/* Search and Stats */}
      <div className="flex items-center justify-between">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search inventory..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        <div className="flex space-x-2">
          <Badge variant="secondary">
            {filteredInventory.length} items
          </Badge>
          <Badge variant="warning">
            {filteredInventory.filter(item => item.quantity < 50).length} low stock
          </Badge>
          <Badge variant="destructive">
            {filteredInventory.filter(item => isExpiringSoon(item.expiryDate) || isExpired(item.expiryDate)).length} expiring
          </Badge>
        </div>
      </div>

      {/* Inventory Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <Package className="w-5 h-5" />
            <span>Inventory Stock</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b">
                  <th className="text-left p-2">Medication</th>
                  <th className="text-left p-2">Batch</th>
                  <th className="text-left p-2">Quantity</th>
                  <th className="text-left p-2">Expiry Date</th>
                  <th className="text-left p-2">Supplier</th>
                  <th className="text-left p-2">Prices</th>
                  <th className="text-left p-2">Status</th>
                </tr>
              </thead>
              <tbody>
                {filteredInventory.map((item) => {
                  const stockStatus = getStockStatus(item.quantity);
                  const expiring = isExpiringSoon(item.expiryDate);
                  const expired = isExpired(item.expiryDate);
                  
                  return (
                    <tr key={item.id} className="border-b hover:bg-muted/50">
                      <td className="p-2">
                        <div className="font-medium">{item.medicationName}</div>
                      </td>
                      <td className="p-2">
                        <div className="font-mono text-sm">{item.batchNumber}</div>
                        <div className="text-xs text-muted-foreground">{item.receivedDate}</div>
                      </td>
                      <td className="p-2">
                        <div className="font-mono font-medium">{item.quantity}</div>
                      </td>
                      <td className="p-2">
                        <div className={`flex items-center space-x-1 ${expired ? 'text-destructive' : expiring ? 'text-warning' : ''}`}>
                          {(expired || expiring) && <Calendar className="w-4 h-4" />}
                          <span className="text-sm">{item.expiryDate}</span>
                        </div>
                        {expired && <div className="text-xs text-destructive">Expired</div>}
                        {expiring && !expired && <div className="text-xs text-warning">Expiring Soon</div>}
                      </td>
                      <td className="p-2 text-sm">{item.supplier}</td>
                      <td className="p-2">
                        <div className="text-sm space-y-1">
                          <div>Cost: <span className="font-mono">${item.costPrice.toFixed(2)}</span></div>
                          <div>Sale: <span className="font-mono">${item.sellingPrice.toFixed(2)}</span></div>
                        </div>
                      </td>
                      <td className="p-2">
                        <div className="space-y-1">
                          <Badge variant={stockStatus.variant}>
                            {stockStatus.label}
                          </Badge>
                          {item.quantity < 20 && (
                            <div className="flex items-center space-x-1 text-destructive">
                              <AlertTriangle className="w-3 h-3" />
                              <span className="text-xs">Reorder needed</span>
                            </div>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default InventoryPage;