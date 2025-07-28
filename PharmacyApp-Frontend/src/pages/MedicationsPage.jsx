import React, { useState, useEffect } from 'react';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../components/ui/dialog';
import { Badge } from '../components/ui/badge';
import { useToast } from '../hooks/use-toast';
import { getMedications, createMedication, updateMedication, deleteMedication } from '../api/mockApi.js';
import { Pill, Plus, Search, Edit, Trash2 } from 'lucide-react';

const MedicationsPage = () => {
  const [medications, setMedications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingMedication, setEditingMedication] = useState(null);
  const [formData, setFormData] = useState({
    name: '',
    genericName: '',
    strength: '',
    dosageForm: '',
    manufacturer: '',
    price: '',
    ingredients: '',
    category: ''
  });
  const { toast } = useToast();

  useEffect(() => {
    fetchMedications();
  }, []);

  const fetchMedications = async () => {
    try {
      const data = await getMedications();
      setMedications(data);
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to fetch medications",
        variant: "destructive"
      });
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    const medicationData = {
      ...formData,
      price: parseFloat(formData.price),
      ingredients: formData.ingredients.split(',').map(i => i.trim())
    };

    try {
      if (editingMedication) {
        await updateMedication(editingMedication.id, medicationData);
        toast({
          title: "Success",
          description: "Medication updated successfully"
        });
      } else {
        await createMedication(medicationData);
        toast({
          title: "Success",
          description: "Medication created successfully"
        });
      }
      
      fetchMedications();
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

  const handleEdit = (medication) => {
    setEditingMedication(medication);
    setFormData({
      ...medication,
      ingredients: medication.ingredients.join(', ')
    });
    setIsDialogOpen(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this medication?')) {
      try {
        await deleteMedication(id);
        toast({
          title: "Success",
          description: "Medication deleted successfully"
        });
        fetchMedications();
      } catch (error) {
        toast({
          title: "Error",
          description: error.message,
          variant: "destructive"
        });
      }
    }
  };

  const resetForm = () => {
    setFormData({
      name: '',
      genericName: '',
      strength: '',
      dosageForm: '',
      manufacturer: '',
      price: '',
      ingredients: '',
      category: ''
    });
    setEditingMedication(null);
  };

  const filteredMedications = medications.filter(medication =>
    medication.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    medication.genericName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    medication.category.toLowerCase().includes(searchTerm.toLowerCase())
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
          <h1 className="text-2xl font-bold text-foreground">Medication Management</h1>
          <p className="text-muted-foreground">Manage pharmacy medications and drug information</p>
        </div>
        
        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogTrigger asChild>
            <Button onClick={() => resetForm()}>
              <Plus className="w-4 h-4 mr-2" />
              Add Medication
            </Button>
          </DialogTrigger>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>
                {editingMedication ? 'Edit Medication' : 'Add New Medication'}
              </DialogTitle>
              <DialogDescription>
                Enter medication details below
              </DialogDescription>
            </DialogHeader>
            
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="name">Brand Name</Label>
                  <Input
                    id="name"
                    value={formData.name}
                    onChange={(e) => setFormData({...formData, name: e.target.value})}
                    required
                  />
                </div>
                <div>
                  <Label htmlFor="genericName">Generic Name</Label>
                  <Input
                    id="genericName"
                    value={formData.genericName}
                    onChange={(e) => setFormData({...formData, genericName: e.target.value})}
                    required
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="strength">Strength</Label>
                  <Input
                    id="strength"
                    value={formData.strength}
                    onChange={(e) => setFormData({...formData, strength: e.target.value})}
                    placeholder="e.g., 500mg"
                    required
                  />
                </div>
                <div>
                  <Label htmlFor="dosageForm">Dosage Form</Label>
                  <Select value={formData.dosageForm} onValueChange={(value) => setFormData({...formData, dosageForm: value})}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select form" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="Tablet">Tablet</SelectItem>
                      <SelectItem value="Capsule">Capsule</SelectItem>
                      <SelectItem value="Syrup">Syrup</SelectItem>
                      <SelectItem value="Injection">Injection</SelectItem>
                      <SelectItem value="Cream">Cream</SelectItem>
                      <SelectItem value="Drops">Drops</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="manufacturer">Manufacturer</Label>
                  <Input
                    id="manufacturer"
                    value={formData.manufacturer}
                    onChange={(e) => setFormData({...formData, manufacturer: e.target.value})}
                    required
                  />
                </div>
                <div>
                  <Label htmlFor="price">Price ($)</Label>
                  <Input
                    id="price"
                    type="number"
                    step="0.01"
                    value={formData.price}
                    onChange={(e) => setFormData({...formData, price: e.target.value})}
                    required
                  />
                </div>
              </div>

              <div>
                <Label htmlFor="category">Category</Label>
                <Input
                  id="category"
                  value={formData.category}
                  onChange={(e) => setFormData({...formData, category: e.target.value})}
                  placeholder="e.g., Antibiotic, NSAID"
                  required
                />
              </div>

              <div>
                <Label htmlFor="ingredients">Ingredients (comma-separated)</Label>
                <Input
                  id="ingredients"
                  value={formData.ingredients}
                  onChange={(e) => setFormData({...formData, ingredients: e.target.value})}
                  placeholder="e.g., Amoxicillin 500mg, Gelatin, Magnesium Stearate"
                  required
                />
              </div>

              <div className="flex justify-end space-x-2">
                <Button type="button" variant="outline" onClick={() => setIsDialogOpen(false)}>
                  Cancel
                </Button>
                <Button type="submit">
                  {editingMedication ? 'Update' : 'Create'} Medication
                </Button>
              </div>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      {/* Search */}
      <div className="flex items-center space-x-2">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search medications..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        <Badge variant="secondary">
          {filteredMedications.length} medications
        </Badge>
      </div>

      {/* Medications Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <Pill className="w-5 h-5" />
            <span>Medications</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b">
                  <th className="text-left p-2">Medication</th>
                  <th className="text-left p-2">Strength & Form</th>
                  <th className="text-left p-2">Category</th>
                  <th className="text-left p-2">Manufacturer</th>
                  <th className="text-left p-2">Price</th>
                  <th className="text-left p-2">Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredMedications.map((medication) => (
                  <tr key={medication.id} className="border-b hover:bg-muted/50">
                    <td className="p-2">
                      <div>
                        <div className="font-medium">{medication.name}</div>
                        <div className="text-sm text-muted-foreground">{medication.genericName}</div>
                      </div>
                    </td>
                    <td className="p-2">
                      <div className="text-sm">
                        <div>{medication.strength}</div>
                        <div className="text-muted-foreground">{medication.dosageForm}</div>
                      </div>
                    </td>
                    <td className="p-2">
                      <Badge variant="outline">
                        {medication.category}
                      </Badge>
                    </td>
                    <td className="p-2 text-sm">{medication.manufacturer}</td>
                    <td className="p-2 font-mono">${medication.price.toFixed(2)}</td>
                    <td className="p-2">
                      <div className="flex space-x-2">
                        <Button
                          size="sm"
                          variant="outline"
                          onClick={() => handleEdit(medication)}
                        >
                          <Edit className="w-4 h-4" />
                        </Button>
                        <Button
                          size="sm"
                          variant="outline"
                          onClick={() => handleDelete(medication.id)}
                        >
                          <Trash2 className="w-4 h-4" />
                        </Button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default MedicationsPage;