import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import axios from 'axios';
import API_CONFIG from '../config/api.config';

interface AuthContextType {
  token: string | null;
  login: (email: string, password: string) => Promise<boolean>;
  logout: () => void;
  isAuthenticated: boolean;
  confirmEmail: (userId: string) => Promise<boolean>;
  checkAccess: (requiredRole: string) => Promise<boolean>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(!!token);

  // Token değiştiğinde isAuthenticated durumunu güncelle
  useEffect(() => {
    setIsAuthenticated(!!token);
    
    // Token varsa, axios'un default header'larına ekle
    if (token) {
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    } else {
      delete axios.defaults.headers.common['Authorization'];
    }
  }, [token]);

  const login = async (email: string, password: string): Promise<boolean> => {
    try {
      console.log('AuthContext login attempt with:', { email });
      console.log('Using API endpoint:', API_CONFIG.ENDPOINTS.AUTH.LOGIN);
      
      // Axios isteğini daha detaylı yapılandıralım
      const response = await axios({
        method: 'post',
        url: API_CONFIG.ENDPOINTS.AUTH.LOGIN,
        data: { email, password },
        headers: {
          'Content-Type': 'application/json'
        }
      });
      
      console.log('Login API response:', response.data);
      
      if (response.data && response.data.success && response.data.token) {
        const newToken = response.data.token;
        localStorage.setItem('token', newToken);
        setToken(newToken);
        
        // Token'ı axios'un default header'larına ekle
        axios.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
        
        return true;
      }
      return false;
    } catch (error: any) {
      console.error('Login error:', error);
      // Hata mesajını daha detaylı göster
      if (error.response) {
        console.error('Error response:', error.response.data);
        console.error('Status code:', error.response.status);
        console.error('Headers:', error.response.headers);
      } else if (error.request) {
        console.error('Error request:', error.request);
      } else {
        console.error('Error message:', error.message);
      }
      return false;
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setToken(null);
    
    // Token'ı axios'un default header'larından kaldır
    delete axios.defaults.headers.common['Authorization'];
    
    // Sayfayı yönlendirmek yerine, window.location.href kullanarak login sayfasına yönlendir
    window.location.href = '/login';
  };

  const confirmEmail = async (userId: string): Promise<boolean> => {
    try {
      // API endpoint'ini düzelttik ve boş bir nesne gönderiyoruz
      const response = await axios.post(`${API_CONFIG.ENDPOINTS.AUTH.CONFIRM_EMAIL}/${userId}`, {});
      return response.status === 200;
    } catch (error) {
      console.error('Email confirmation error:', error);
      return false;
    }
  };

  const checkAccess = async (requiredRole: string): Promise<boolean> => {
    if (!token) return false;

    try {
      const response = await axios.post(
        API_CONFIG.ENDPOINTS.AUTH.CHECK_ACCESS, 
        { role: requiredRole },
        { headers: { Authorization: `Bearer ${token}` } }
      );
      
      return response.data && response.data.hasAccess;
    } catch (error) {
      console.error('Check access error:', error);
      return false;
    }
  };

  return (
    <AuthContext.Provider value={{ token, login, logout, isAuthenticated, confirmEmail, checkAccess }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
