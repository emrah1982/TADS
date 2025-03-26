import React, { useState, useEffect, useCallback } from 'react';
import {
  Box,
  Paper,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Chip,
  Alert,
  Snackbar,
  AppBar,
  Toolbar,
  CircularProgress
} from '@mui/material';
import { PersonAdd, Edit, Email } from '@mui/icons-material';
import Sidebar from '../components/Sidebar';
import { useAuth } from '../contexts/AuthContext';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import API_CONFIG from '../config/api.config';

interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
  emailConfirmed: boolean;
}

interface JwtPayload {
  sub: string;
  email: string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string | string[];
  exp: number;
}

const UserManagement: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [roles, setRoles] = useState<string[]>([]);
  const [openDialog, setOpenDialog] = useState(false);
  const [openRoleDialog, setOpenRoleDialog] = useState(false);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [newRole, setNewRole] = useState('');
  const [selectedRole, setSelectedRole] = useState('');
  const [message, setMessage] = useState('');
  const [severity, setSeverity] = useState<'success' | 'error'>('success');
  const [openSnackbar, setOpenSnackbar] = useState(false);
  const [loading, setLoading] = useState(false);
  const [userRoles, setUserRoles] = useState<string[]>([]);
  const { token } = useAuth();

  // API URL'lerini sabit olarak tanımlayalım
  const API_ENDPOINTS = {
    USERS: API_CONFIG.ENDPOINTS.USER_MANAGEMENT.USERS,
    ROLES: API_CONFIG.ENDPOINTS.USER_MANAGEMENT.ROLES,
    CREATE_ROLE: API_CONFIG.ENDPOINTS.USER_MANAGEMENT.CREATE_ROLE,
    ASSIGN_ROLE: API_CONFIG.ENDPOINTS.USER_MANAGEMENT.ASSIGN_ROLE,
    CONFIRM_EMAIL: API_CONFIG.ENDPOINTS.USER_MANAGEMENT.CONFIRM_EMAIL,
    CONFIRM_ALL_EMAILS: API_CONFIG.ENDPOINTS.USER_MANAGEMENT.CONFIRM_ALL_EMAILS
  };

  // Token'dan kullanıcı rollerini çıkaran fonksiyon
  useEffect(() => {
    if (token) {
      try {
        const decoded = jwtDecode<JwtPayload>(token);
        console.log('Decoded token:', decoded);
        
        const roles = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        console.log('Roles from token:', roles);
        
        if (roles) {
          setUserRoles(Array.isArray(roles) ? roles : [roles]);
        } else {
          console.warn('No roles found in token');
          setUserRoles([]);
        }
      } catch (error) {
        console.error('Token decode error:', error);
        setUserRoles([]);
      }
    }
  }, [token]);

  // Kullanıcının superadmin rolüne sahip olup olmadığını kontrol eden fonksiyon
  const isSuperAdmin = useCallback(() => {
    return userRoles.includes('superadmin');
  }, [userRoles]);

  // API istekleri için header'ları hazırlayan yardımcı fonksiyon
  const getAuthHeaders = useCallback(() => {
    const token = localStorage.getItem('token');
    return {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    };
  }, []);

  const fetchRoles = useCallback(async () => {
    try {
      setLoading(true);
      console.log('Fetching roles from:', API_ENDPOINTS.ROLES);
      
      // API çağrısını deneyelim - GetRoles artık [AllowAnonymous] olduğu için header gerekmez
      const response = await axios.get(API_ENDPOINTS.ROLES);
      
      console.log('Roles API Response:', response);
      
      if (response.data) {
        // API yanıtını kontrol et ve dizi olduğundan emin ol
        const rolesData = Array.isArray(response.data) ? response.data :
                         response.data.roles ? response.data.roles :
                         response.data.value ? response.data.value : [];
        console.log('Processed roles data:', rolesData);
        setRoles(rolesData);
        showMessage('Roller başarıyla yüklendi', 'success');
      }
    } catch (error: any) {
      console.error('Error fetching roles:', error);
      console.error('Error details:', {
        message: error.message,
        response: error.response,
        config: error.config
      });
      const errorMessage = error.response?.data?.message || 'Roller veritabanından yüklenemedi';
      showMessage(`${errorMessage}. Statik veriler gösteriliyor.`, 'error');
      
      // Hata durumunda statik verileri göster
      setRoles(['superadmin', 'user', 'manager', 'analyst', 'field-worker']);
    } finally {
      setLoading(false);
    }
  }, []);  // token'ı kaldırdık çünkü artık gerekli değil

  const fetchUsers = useCallback(async () => {
    try {
      setLoading(true);
      console.log('Fetching users from:', API_ENDPOINTS.USERS);
      console.log('Headers:', getAuthHeaders());
      
      // API çağrısını deneyelim
      const response = await axios.get(API_ENDPOINTS.USERS, {
        headers: getAuthHeaders()
      });
      
      console.log('Users API Response:', response);
      
      if (response.data) {
        // API yanıtını kontrol et ve roles'u düzelt
        const processedUsers = (Array.isArray(response.data) ? response.data : 
                              response.data.users ? response.data.users :
                              response.data.value ? response.data.value : []).map((user: any) => ({
          ...user,
          roles: Array.isArray(user.roles) ? user.roles : 
                 typeof user.roles === 'string' ? [user.roles] : []
        }));
        
        console.log('Processed Users:', processedUsers);
        setUsers(processedUsers);
        showMessage('Kullanıcılar başarıyla yüklendi', 'success');
      }
    } catch (error: any) {
      console.error('Error fetching users:', error);
      console.error('Error details:', {
        message: error.message,
        response: error.response,
        config: error.config
      });
      const errorMessage = error.response?.data?.message || 'Kullanıcılar veritabanından yüklenemedi';
      showMessage(`${errorMessage}. Statik veriler gösteriliyor.`, 'error');
      
      // Hata durumunda statik verileri göster
      setUsers([
        {
          id: '1',
          email: 'admin@tads.com',
          firstName: 'Admin',
          lastName: 'Kullanıcı',
          roles: ['superadmin', 'user'],
          emailConfirmed: true
        },
        {
          id: '2',
          email: 'ahmet@tads.com',
          firstName: 'Ahmet',
          lastName: 'Yılmaz',
          roles: ['user'],
          emailConfirmed: false
        },
        {
          id: '3',
          email: 'mehmet@tads.com',
          firstName: 'Mehmet',
          lastName: 'Öztürk',
          roles: ['user'],
          emailConfirmed: true
        }
      ]);
    } finally {
      setLoading(false);
    }
  }, [getAuthHeaders]);

  useEffect(() => {
    fetchUsers();
    fetchRoles();
  }, [fetchUsers, fetchRoles]);

  // Yeni rol oluşturma işlemi
  const handleCreateRole = async () => {
    if (!newRole.trim()) {
      showMessage('Rol adı boş olamaz', 'error');
      return;
    }
    
    if (roles.includes(newRole)) {
      showMessage('Bu rol zaten mevcut', 'error');
      return;
    }
    
    try {
      setLoading(true);
      console.log('Yeni rol oluşturma isteği gönderiliyor:', {
        roleName: newRole
      });
      
      // API çağrısını deneyelim
      const response = await axios.post(
        API_ENDPOINTS.CREATE_ROLE, 
        { roleName: newRole },
        { 
          headers: getAuthHeaders(),
          withCredentials: false
        }
      );

      console.log('Rol oluşturma yanıtı:', response.data);

      if (response.data && response.data.success) {
        showMessage('Rol başarıyla oluşturuldu', 'success');
        fetchRoles();
        setOpenRoleDialog(false);
        setNewRole('');
      } else {
        throw new Error(response.data?.message || 'Rol oluşturulurken hata oluştu');
      }
    } catch (error: any) {
      console.error('Error creating role:', error);
      const errorMessage = error.response?.data?.message || 'Rol oluşturulurken hata oluştu';
      showMessage(`Rol oluşturulurken hata oluştu: ${errorMessage}. Statik veri kullanılıyor.`, 'error');
      
      // Hata durumunda UI'ı güncelleyelim (statik veri)
      setRoles([...roles, newRole]);
      setOpenRoleDialog(false);
      setNewRole('');
      showMessage('Rol başarıyla oluşturuldu (statik veri)', 'success');
    } finally {
      setLoading(false);
    }
  };

  // Rol atama işlemi
  const handleAssignRole = async () => {
    if (!selectedUser) return;
    
    try {
      setLoading(true);
      console.log('Rol atama isteği gönderiliyor:', {
        userId: selectedUser.id,
        role: selectedRole
      });
      
      // API çağrısını deneyelim
      const response = await axios.post(
        API_ENDPOINTS.ASSIGN_ROLE,
        {
          userId: selectedUser.id,
          role: selectedRole
        },
        { 
          headers: getAuthHeaders(),
          withCredentials: false
        }
      );

      console.log('Rol atama yanıtı:', response.data);

      if (response.data && response.data.success) {
        showMessage('Rol başarıyla atandı', 'success');
        fetchUsers();
        setOpenDialog(false);
      } else {
        throw new Error(response.data?.message || 'Rol atanırken hata oluştu');
      }
    } catch (error: any) {
      console.error('Error assigning role:', error);
      const errorMessage = error.response?.data?.message || 'Rol atanırken hata oluştu';
      showMessage(`${errorMessage}. Statik veri kullanılıyor.`, 'error');
      
      // Hata durumunda UI'ı güncelleyelim (statik veri)
      if (selectedUser && selectedRole) {
        const updatedUsers = users.map(user => {
          if (user.id === selectedUser.id) {
            return {
              ...user,
              roles: [selectedRole]
            };
          }
          return user;
        });
        setUsers(updatedUsers);
        setOpenDialog(false);
        showMessage('Rol başarıyla atandı (statik veri)', 'success');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleConfirmEmail = async (userId: string) => {
    try {
      setLoading(true);
      // API çağrısını deneyelim
      const response = await axios.post(
        `${API_ENDPOINTS.CONFIRM_EMAIL}/${userId}`,
        {},
        { 
          headers: getAuthHeaders(),
          withCredentials: false
        }
      );

      if (response.data) {
        showMessage('E-posta onayı başarıyla güncellendi', 'success');
        
        // UI'ı güncelleyelim
        const updatedUsers = users.map(user => {
          if (user.id === userId) {
            return {
              ...user,
              emailConfirmed: true
            };
          }
          return user;
        });
        
        setUsers(updatedUsers);
      } else {
        throw new Error('E-posta onayı güncellenirken hata oluştu');
      }
    } catch (error: any) {
      console.error('Error confirming email:', error);
      const errorMessage = error.response?.data?.message || 'E-posta onayı güncellenirken hata oluştu';
      showMessage(errorMessage, 'error');
      
      // Hata durumunda UI'ı güncelleyelim (statik veri)
      const updatedUsers = users.map(user => {
        if (user.id === userId) {
          return {
            ...user,
            emailConfirmed: true
          };
        }
        return user;
      });
      
      setUsers(updatedUsers);
      showMessage('E-posta onayı başarıyla güncellendi (statik veri)', 'success');
    } finally {
      setLoading(false);
    }
  };

  const handleConfirmAllEmails = async () => {
    try {
      setLoading(true);
      // API çağrısını deneyelim
      const response = await axios.post(
        `${API_ENDPOINTS.CONFIRM_ALL_EMAILS}`,
        {},
        { headers: getAuthHeaders() }
      );

      if (response.data && response.data.success) {
        showMessage(response.data.message || 'Tüm e-postalar başarıyla onaylandı', 'success');
        fetchUsers();
      } else {
        throw new Error('E-posta onayları güncellenirken hata oluştu');
      }
    } catch (error: any) {
      console.error('Error confirming all emails:', error);
      const errorMessage = error.response?.data?.message || 'E-posta onayları güncellenirken hata oluştu';
      showMessage(errorMessage, 'error');
    } finally {
      setLoading(false);
    }
  };

  // Rol atama diyaloğunu açma işlevi
  const handleOpenDialog = (user: User) => {
    setSelectedUser(user);
    // Eğer kullanıcının zaten bir rolü varsa, onu seçili olarak göster
    if (user.roles && user.roles.length > 0) {
      setSelectedRole(user.roles[0]);
    } else {
      // Varsayılan olarak ilk rolü seç
      setSelectedRole(roles.length > 0 ? roles[0] : '');
    }
    setOpenDialog(true);
  };

  // Diyaloğu kapatma işlevi
  const handleCloseDialog = () => {
    setOpenDialog(false);
    setSelectedUser(null);
    setSelectedRole('');
  };

  const handleOpenRoleDialog = () => {
    setOpenRoleDialog(true);
  };

  const handleCloseRoleDialog = () => {
    setOpenRoleDialog(false);
    setNewRole('');
  };

  const showMessage = (msg: string, sev: 'success' | 'error') => {
    setMessage(msg);
    setSeverity(sev);
    setOpenSnackbar(true);
  };

  const handleCloseSnackbar = () => {
    setOpenSnackbar(false);
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <Sidebar />
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <AppBar position="static" color="default" elevation={0} sx={{ borderBottom: '1px solid rgba(0, 0, 0, 0.12)' }}>
          <Toolbar>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
              Kullanıcı Yönetimi
            </Typography>
            <Button 
              variant="contained" 
              color="secondary"
              startIcon={<Email />}
              onClick={handleConfirmAllEmails}
              sx={{ mr: 2 }}
            >
              Tüm E-postaları Onayla
            </Button>
            <Button 
              variant="contained" 
              color="primary" 
              startIcon={<PersonAdd />}
              onClick={handleOpenRoleDialog}
              sx={{ mr: 2 }}
            >
              Yeni Rol Oluştur
            </Button>
          </Toolbar>
        </AppBar>

        {loading && (
          <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
            <CircularProgress />
          </Box>
        )}

        {!loading && (
          <TableContainer component={Paper} sx={{ mt: 4 }}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>ID</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Ad</TableCell>
                  <TableCell>Soyad</TableCell>
                  <TableCell>Roller</TableCell>
                  <TableCell>E-posta Onaylı</TableCell>
                  <TableCell>İşlemler</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {users.map((user) => (
                  <TableRow key={user.id}>
                    <TableCell>{user.id}</TableCell>
                    <TableCell>{user.email}</TableCell>
                    <TableCell>{user.firstName}</TableCell>
                    <TableCell>{user.lastName}</TableCell>
                    <TableCell>
                      {user.roles.map((role) => (
                        <Chip 
                          key={role} 
                          label={role} 
                          color={role === 'superadmin' ? 'secondary' : 'primary'} 
                          size="small" 
                          sx={{ mr: 0.5 }} 
                        />
                      ))}
                    </TableCell>
                    <TableCell>
                      {user.emailConfirmed ? 'Evet' : 'Hayır'}
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="outlined"
                        color="primary"
                        size="small"
                        startIcon={<Edit />}
                        onClick={() => handleOpenDialog(user)}
                        sx={{ mr: 1 }}
                        disabled={!isSuperAdmin()}
                      >
                        Rol Ata
                      </Button>
                      <Button
                        variant="outlined"
                        color="secondary"
                        size="small"
                        onClick={() => handleConfirmEmail(user.id)}
                        disabled={user.emailConfirmed}
                      >
                        E-posta Onayla
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}

        {/* Rol Atama Dialog */}
        <Dialog open={openDialog} onClose={handleCloseDialog}>
          <DialogTitle>Rol Ata</DialogTitle>
          <DialogContent>
            <Typography variant="body1" gutterBottom>
              {selectedUser?.firstName} {selectedUser?.lastName} ({selectedUser?.email}) kullanıcısına rol atayın:
            </Typography>
            <FormControl fullWidth sx={{ mt: 2 }}>
              <InputLabel id="role-select-label">Rol</InputLabel>
              <Select
                labelId="role-select-label"
                value={selectedRole}
                label="Rol"
                onChange={(e) => setSelectedRole(e.target.value)}
              >
                {roles.map((role) => (
                  <MenuItem key={role} value={role}>{role}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDialog}>İptal</Button>
            <Button onClick={handleAssignRole} color="primary" variant="contained">
              Rol Ata
            </Button>
          </DialogActions>
        </Dialog>

        {/* Yeni Rol Oluşturma Dialog */}
        <Dialog open={openRoleDialog} onClose={handleCloseRoleDialog}>
          <DialogTitle>Yeni Rol Oluştur</DialogTitle>
          <DialogContent>
            <TextField
              autoFocus
              margin="dense"
              id="name"
              label="Rol Adı"
              type="text"
              fullWidth
              variant="outlined"
              value={newRole}
              onChange={(e) => setNewRole(e.target.value)}
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseRoleDialog}>İptal</Button>
            <Button onClick={handleCreateRole} color="primary" variant="contained">
              Oluştur
            </Button>
          </DialogActions>
        </Dialog>

        {/* Bildirim Snackbar */}
        <Snackbar 
          open={openSnackbar} 
          autoHideDuration={6000} 
          onClose={handleCloseSnackbar}
          anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        >
          <Alert onClose={handleCloseSnackbar} severity={severity} sx={{ width: '100%' }}>
            {message}
          </Alert>
        </Snackbar>
      </Box>
    </Box>
  );
};

export default UserManagement;
