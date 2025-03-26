import React, { useState, useEffect } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { CircularProgress, Box } from '@mui/material';

interface ProtectedRouteProps {
  children: React.ReactNode;
  path?: string;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, path }) => {
  const { isAuthenticated } = useAuth();
  const [loading, setLoading] = useState<boolean>(true);
  const [hasAccess, setHasAccess] = useState<boolean>(true); 

  useEffect(() => {
    const checkAccess = async () => {
      try {
        if (!isAuthenticated) {
          setLoading(false);
          return;
        }

        setHasAccess(true);
        setLoading(false);
        
        /* 
        // Path yoksa varsayılan olarak erişime izin ver
        if (!path) {
          setHasAccess(true);
          setLoading(false);
          return;
        }

        // API'yi çağırarak sayfa erişim izni kontrol edilir
        const response = await fetch(`/api/user/check-access?path=${path}`, {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (response.ok) {
          const data = await response.json();
          setHasAccess(data.hasAccess);
        } else {
          setHasAccess(false);
        }
        */
      } catch (error) {
        console.error('Erişim kontrolü yapılırken hata oluştu:', error);
        setHasAccess(true);
      } finally {
        setLoading(false);
      }
    };

    checkAccess();
  }, [isAuthenticated, path]);

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }

  if (!hasAccess) {
    return <Navigate to="/dashboard" />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
