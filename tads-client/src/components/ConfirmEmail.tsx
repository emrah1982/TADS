import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { Container, Box, Typography, Alert, CircularProgress, Button } from '@mui/material';
import { CheckCircleOutline, ErrorOutline } from '@mui/icons-material';
import API_CONFIG from '../config/api.config';

const ConfirmEmail: React.FC = () => {
  const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
  const [message, setMessage] = useState('E-posta adresiniz doğrulanıyor...');
  const location = useLocation();
  const navigate = useNavigate();
  
  useEffect(() => {
    const confirmEmail = async () => {
      try {
        const params = new URLSearchParams(location.search);
        const token = params.get('token');
        const email = params.get('email');
        
        if (!token || !email) {
          setStatus('error');
          setMessage('Geçersiz doğrulama bağlantısı. Lütfen tekrar deneyin.');
          return;
        }
        
        const response = await axios.get(
          `${API_CONFIG.ENDPOINTS.AUTH.CONFIRM_EMAIL}?token=${token}&email=${email}`
        );
        
        if (response.data.success) {
          setStatus('success');
          setMessage('E-posta adresiniz başarıyla doğrulandı! Şimdi giriş yapabilirsiniz.');
        } else {
          setStatus('error');
          setMessage(response.data.message || 'E-posta doğrulama işlemi başarısız oldu.');
        }
      } catch (error: any) {
        setStatus('error');
        setMessage(error.response?.data?.message || 'E-posta doğrulama işlemi başarısız oldu.');
      }
    };
    
    confirmEmail();
  }, [location.search]);
  
  const handleGoToLogin = () => {
    navigate('/login');
  };
  
  return (
    <Container component="main" maxWidth="xs">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          textAlign: 'center'
        }}
      >
        <Typography component="h1" variant="h5" gutterBottom>
          E-posta Doğrulama
        </Typography>
        
        {status === 'loading' && (
          <Box sx={{ mt: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <CircularProgress size={60} sx={{ mb: 3 }} />
            <Typography variant="body1">{message}</Typography>
          </Box>
        )}
        
        {status === 'success' && (
          <Box sx={{ mt: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <CheckCircleOutline color="success" sx={{ fontSize: 60, mb: 3 }} />
            <Alert severity="success" sx={{ mb: 3, width: '100%' }}>
              {message}
            </Alert>
            <Button 
              variant="contained" 
              color="primary" 
              onClick={handleGoToLogin}
              sx={{ mt: 2 }}
            >
              Giriş Yap
            </Button>
          </Box>
        )}
        
        {status === 'error' && (
          <Box sx={{ mt: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <ErrorOutline color="error" sx={{ fontSize: 60, mb: 3 }} />
            <Alert severity="error" sx={{ mb: 3, width: '100%' }}>
              {message}
            </Alert>
            <Button 
              variant="contained" 
              color="primary" 
              onClick={handleGoToLogin}
              sx={{ mt: 2 }}
            >
              Giriş Sayfasına Dön
            </Button>
          </Box>
        )}
      </Box>
    </Container>
  );
};

export default ConfirmEmail;
