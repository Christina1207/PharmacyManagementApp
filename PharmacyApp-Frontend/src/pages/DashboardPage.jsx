import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Users, Pill, Package, FileText, TrendingUp, AlertTriangle } from 'lucide-react';
import { getPatients, getMedications, getInventory } from '../api/mockApi.js';

const DashboardPage = () => {
  const [stats, setStats] = useState({
    totalPatients: 0,
    totalMedications: 0,
    lowStockItems: 0,
    totalPrescriptions: 0
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const [patients, medications, inventory] = await Promise.all([
          getPatients(),
          getMedications(),
          getInventory()
        ]);

        const lowStockItems = inventory.filter(item => item.quantity < 50).length;

        setStats({
          totalPatients: patients.length,
          totalMedications: medications.length,
          lowStockItems,
          totalPrescriptions: 127 // Mock number
        });
      } catch (error) {
        console.error('Error fetching dashboard stats:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  const statCards = [
    {
      title: 'Total Patients',
      value: stats.totalPatients,
      description: 'Registered in system',
      icon: Users,
      color: 'text-primary'
    },
    {
      title: 'Medications',
      value: stats.totalMedications,
      description: 'Available medications',
      icon: Pill,
      color: 'text-secondary'
    },
    {
      title: 'Low Stock Items',
      value: stats.lowStockItems,
      description: 'Items below 50 units',
      icon: AlertTriangle,
      color: 'text-warning'
    },
    {
      title: 'Prescriptions',
      value: stats.totalPrescriptions,
      description: 'Total dispensed',
      icon: FileText,
      color: 'text-success'
    }
  ];

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-foreground">Dashboard</h1>
        <p className="text-muted-foreground">Overview of pharmacy operations</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {statCards.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <Card key={index}>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">
                  {stat.title}
                </CardTitle>
                <Icon className={`h-4 w-4 ${stat.color}`} />
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">{stat.value}</div>
                <p className="text-xs text-muted-foreground">
                  {stat.description}
                </p>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center space-x-2">
              <TrendingUp className="w-5 h-5" />
              <span>Recent Activity</span>
            </CardTitle>
            <CardDescription>Latest system activities</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <div className="flex justify-between items-center">
                <span className="text-sm">Prescription dispensed</span>
                <span className="text-xs text-muted-foreground">2 minutes ago</span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-sm">New patient registered</span>
                <span className="text-xs text-muted-foreground">15 minutes ago</span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-sm">Inventory updated</span>
                <span className="text-xs text-muted-foreground">1 hour ago</span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-sm">Medication added</span>
                <span className="text-xs text-muted-foreground">2 hours ago</span>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center space-x-2">
              <AlertTriangle className="w-5 h-5 text-warning" />
              <span>Alerts</span>
            </CardTitle>
            <CardDescription>Items requiring attention</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {stats.lowStockItems > 0 && (
                <div className="p-3 bg-warning/10 rounded-lg">
                  <p className="text-sm font-medium text-warning">Low Stock Alert</p>
                  <p className="text-xs text-muted-foreground">
                    {stats.lowStockItems} items are running low
                  </p>
                </div>
              )}
              <div className="p-3 bg-info/10 rounded-lg">
                <p className="text-sm font-medium text-info">System Update</p>
                <p className="text-xs text-muted-foreground">
                  All systems running normally
                </p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default DashboardPage;