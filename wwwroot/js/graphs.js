import React, { useState } from 'react';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { Users, Package, ShoppingCart, UserCheck, TrendingUp } from 'lucide-react';

// Sample data for a week-old store - replace with your actual API data
const initialData = {
    dailyMetrics: [
        { date: 'Mon', orders: 3, newUsers: 8, activeUsers: 6 },
        { date: 'Tue', orders: 5, newUsers: 12, activeUsers: 10 },
        { date: 'Wed', orders: 4, newUsers: 7, activeUsers: 8 },
        { date: 'Thu', orders: 6, newUsers: 9, activeUsers: 11 },
        { date: 'Fri', orders: 8, newUsers: 15, activeUsers: 13 },
        { date: 'Sat', orders: 7, newUsers: 11, activeUsers: 12 },
        { date: 'Sun', orders: 5, newUsers: 10, activeUsers: 9 }
    ],
    recentOrders: [
        { id: '001', date: '2024-11-10', status: 'Delivered', amount: 299 },
        { id: '002', date: '2024-11-09', status: 'Processing', amount: 549 },
        { id: '003', date: '2024-11-09', status: 'Pending', amount: 199 },
        { id: '004', date: '2024-11-08', status: 'Delivered', amount: 399 },
        { id: '005', date: '2024-11-08', status: 'Processing', amount: 749 }
    ]
};

const AdminDashboard = () => {
    const [data] = useState(initialData);

    // Calculate summary metrics
    const totalOrders = data.recentOrders.length;
    const totalUsers = data.dailyMetrics.reduce((sum, day) => sum + day.newUsers, 0);
    const activeUsers = data.dailyMetrics[data.dailyMetrics.length - 1].activeUsers;
    const totalRevenue = data.recentOrders.reduce((sum, order) => sum + order.amount, 0);

    return (
        <div className="p-6 space-y-6 bg-gray-50">
            {/* Key Metrics */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                <Card>
                    <CardHeader className="flex flex-row items-center space-x-2">
                        <Package className="w-6 h-6 text-blue-500" />
                        <CardTitle className="text-lg">Total Orders</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{totalOrders}</div>
                        <p className="text-sm text-gray-500">Since launch</p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center space-x-2">
                        <Users className="w-6 h-6 text-green-500" />
                        <CardTitle className="text-lg">Total Users</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{totalUsers}</div>
                        <p className="text-sm text-gray-500">Since launch</p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center space-x-2">
                        <UserCheck className="w-6 h-6 text-purple-500" />
                        <CardTitle className="text-lg">Active Users</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{activeUsers}</div>
                        <p className="text-sm text-gray-500">Today</p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center space-x-2">
                        <TrendingUp className="w-6 h-6 text-orange-500" />
                        <CardTitle className="text-lg">Total Revenue</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">${totalRevenue}</div>
                        <p className="text-sm text-gray-500">Since launch</p>
                    </CardContent>
                </Card>
            </div>

            {/* Daily Activity Chart */}
            <Card>
                <CardHeader>
                    <CardTitle>Daily Activity</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="h-80">
                        <ResponsiveContainer width="100%" height="100%">
                            <LineChart data={data.dailyMetrics}>
                                <CartesianGrid strokeDasharray="3 3" />
                                <XAxis dataKey="date" />
                                <YAxis />
                                <Tooltip />
                                <Legend />
                                <Line
                                    type="monotone"
                                    dataKey="orders"
                                    stroke="#8884d8"
                                    name="Orders"
                                />
                                <Line
                                    type="monotone"
                                    dataKey="newUsers"
                                    stroke="#82ca9d"
                                    name="New Users"
                                />
                                <Line
                                    type="monotone"
                                    dataKey="activeUsers"
                                    stroke="#ffc658"
                                    name="Active Users"
                                />
                            </LineChart>
                        </ResponsiveContainer>
                    </div>
                </CardContent>
            </Card>

            {/* Recent Orders Table */}
            <Card>
                <CardHeader>
                    <CardTitle>Recent Orders</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead>
                            <tr className="border-b">
                                <th className="p-2 text-left">Order ID</th>
                                <th className="p-2 text-left">Date</th>
                                <th className="p-2 text-left">Status</th>
                                <th className="p-2 text-right">Amount</th>
                            </tr>
                            </thead>
                            <tbody>
                            {data.recentOrders.map((order) => (
                                <tr key={order.id} className="border-b">
                                    <td className="p-2">{order.id}</td>
                                    <td className="p-2">{order.date}</td>
                                    <td className="p-2">
                      <span className={`px-2 py-1 rounded-full text-xs ${
                          order.status === 'Delivered' ? 'bg-green-100 text-green-800' :
                              order.status === 'Processing' ? 'bg-blue-100 text-blue-800' :
                                  'bg-yellow-100 text-yellow-800'
                      }`}>
                        {order.status}
                      </span>
                                    </td>
                                    <td className="p-2 text-right">${order.amount}</td>
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

export default AdminDashboard;