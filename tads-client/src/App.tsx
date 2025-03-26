import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import Dashboard from './components/Dashboard';
import UserManagement from './pages/UserManagement';
import PageManagement from './pages/PageManagement';
import FieldManagement from './pages/FieldManagement';
import IrrigationFertilizationAnalysis from './pages/IrrigationFertilizationAnalysis';
import ConfirmEmail from './components/ConfirmEmail';
import { Box, CssBaseline } from '@mui/material';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';

const App: React.FC = () => {
  return (
    <AuthProvider>
      <Box sx={{ display: 'flex' }}>
        <CssBaseline />
        <Router>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/confirm-email" element={<ConfirmEmail />} />
            <Route path="/dashboard" element={
              <ProtectedRoute path="/dashboard">
                <Dashboard />
              </ProtectedRoute>
            } />
            <Route path="/user-management" element={
              <ProtectedRoute path="/user-management">
                <UserManagement />
              </ProtectedRoute>
            } />
            <Route path="/page-management" element={
              <ProtectedRoute path="/page-management">
                <PageManagement />
              </ProtectedRoute>
            } />
            <Route path="/field-management" element={
              <ProtectedRoute path="/field-management">
                <FieldManagement />
              </ProtectedRoute>
            } />
            <Route path="/irrigation-fertilization-analysis" element={
              <ProtectedRoute path="/irrigation-fertilization-analysis">
                <IrrigationFertilizationAnalysis />
              </ProtectedRoute>
            } />
            <Route path="/" element={<Navigate to="/login" replace />} />
            <Route path="*" element={<Navigate to="/login" replace />} />
          </Routes>
        </Router>
      </Box>
    </AuthProvider>
  );
};

export default App;
