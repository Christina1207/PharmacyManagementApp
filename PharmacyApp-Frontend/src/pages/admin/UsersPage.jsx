import React, { useState, useEffect } from 'react';
import { Button } from '../../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../../components/ui/dialog';
import { Badge } from '../../components/ui/badge';
import { useToast } from '../../hooks/use-toast';
import userService from '../../services/userService';
import { Settings, User, Plus, Power, PowerOff, KeyRound } from 'lucide-react';

const UsersPage = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [dialogMode, setDialogMode] = useState('add'); // 'add' or 'reset'
    const [selectedUser, setSelectedUser] = useState(null);
    const [formData, setFormData] = useState({
        username: '',
        firstName: '',
        lastName: '',
        email: '',
        password: ''
    });
    const [newPassword, setNewPassword] = useState('');

    const { toast } = useToast();

    const fetchUsers = async () => {
        setLoading(true);
        try {
            const data = await userService.getUsers();
            setUsers(data);
        } catch (error) {
            toast({ title: "Error", description: "Could not fetch users.", variant: "destructive" });
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchUsers();
    }, []);

    const handleDeactivate = async (id) => {
        try {
            await userService.deactivateUser(id);
            toast({ title: "Success", description: "User has been deactivated." });
            fetchUsers();
        } catch (error) {
            toast({ title: "Error", description: "Failed to deactivate user.", variant: "destructive" });
        }
    };
    
    const handleActivate = async (id) => {
        try {
            await userService.activateUser(id);
            toast({ title: "Success", description: "User has been activated." });
            fetchUsers();
        } catch (error) {
            toast({ title: "Error", description: "Failed to activate user.", variant: "destructive" });
        }
    };

    const handleOpenDialog = (mode, user = null) => {
        setDialogMode(mode);
        setSelectedUser(user);
        setIsDialogOpen(true);
        setFormData({ username: '', firstName: '', lastName: '', email: '', password: '' });
        setNewPassword('');
    };

    const handleFormSubmit = async (e) => {
        e.preventDefault();
        if (dialogMode === 'add') {
            try {
                await userService.registerAdmin(formData);
                toast({ title: "Success", description: "New admin registered successfully."});
                setIsDialogOpen(false);
                fetchUsers();
            } catch (error) {
                toast({ title: "Error", description: "Failed to register admin.", variant: "destructive" });
            }
        } else if (dialogMode === 'reset') {
            try {
                await userService.resetPassword(selectedUser.id, newPassword);
                toast({ title: "Success", description: `Password for ${selectedUser.userName} has been reset.`});
                setIsDialogOpen(false);
            } catch (error) {
                 toast({ title: "Error", description: "Failed to reset password.", variant: "destructive" });
            }
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-2xl font-bold text-foreground">User Management</h1>
                    <p className="text-muted-foreground">Manage system users and their status.</p>
                </div>
                <Button onClick={() => handleOpenDialog('add')}>
                    <Plus className="w-4 h-4 mr-2" />
                    Add New Admin
                </Button>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle className="flex items-center space-x-2"><Settings className="w-5 h-5" /><span>System Users</span></CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead>
                                <tr className="border-b">
                                    <th className="text-left p-2">User</th>
                                    <th className="text-left p-2">Role</th>
                                    <th className="text-left p-2">Status</th>
                                    <th className="text-left p-2">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {users.map((user) => (
                                    <tr key={user.id} className="border-b hover:bg-muted/50">
                                        <td className="p-2">
                                            <div className="font-medium">{user.firstName} {user.lastName}</div>
                                            <div className="text-sm text-muted-foreground">@{user.userName}</div>
                                        </td>
                                        <td className="p-2"><Badge variant={user.role === 'Admin' ? 'default' : 'secondary'}>{user.role}</Badge></td>
                                        <td className="p-2">
                                            <Badge variant={user.isActive ? 'success' : 'destructive'}>
                                                {user.isActive ? 'Active' : 'Deactivated'}
                                            </Badge>
                                        </td>
                                        <td className="p-2 flex space-x-2">
                                            {user.isActive ? (
                                                <Button variant="outline" size="sm" onClick={() => handleDeactivate(user.id)}><PowerOff className="w-4 h-4 mr-2"/>Deactivate</Button>
                                            ) : (
                                                <Button variant="outline" size="sm" onClick={() => handleActivate(user.id)}><Power className="w-4 h-4 mr-2"/>Activate</Button>
                                            )}
                                             <Button variant="outline" size="sm" onClick={() => handleOpenDialog('reset', user)}><KeyRound className="w-4 h-4 mr-2"/>Reset Password</Button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </CardContent>
            </Card>

            <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>{dialogMode === 'add' ? 'Register New Admin' : `Reset Password for ${selectedUser?.userName}`}</DialogTitle>
                    </DialogHeader>
                    <form onSubmit={handleFormSubmit} className="space-y-4">
                        {dialogMode === 'add' ? (
                            <>
                                <Input placeholder="Username" value={formData.username} onChange={e => setFormData({...formData, username: e.target.value})} required/>
                                <Input placeholder="First Name" value={formData.firstName} onChange={e => setFormData({...formData, firstName: e.target.value})} required/>
                                <Input placeholder="Last Name" value={formData.lastName} onChange={e => setFormData({...formData, lastName: e.target.value})} required/>
                                <Input type="email" placeholder="Email" value={formData.email} onChange={e => setFormData({...formData, email: e.target.value})} required/>
                                <Input type="password" placeholder="Password" value={formData.password} onChange={e => setFormData({...formData, password: e.target.value})} required/>
                            </>
                        ) : (
                            <div>
                                <Label htmlFor="new-password">New Password</Label>
                                <Input id="new-password" type="password" value={newPassword} onChange={e => setNewPassword(e.target.value)} required/>
                            </div>
                        )}
                        <Button type="submit" className="w-full">
                            {dialogMode === 'add' ? 'Create Admin' : 'Set New Password'}
                        </Button>
                    </form>
                </DialogContent>
            </Dialog>
        </div>
    );
};

export default UsersPage;