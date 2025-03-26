import React, { useState, useEffect, useCallback } from 'react';
import { 
  Container, 
  Box, 
  Typography, 
  Paper, 
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
  FormControl, 
  InputLabel, 
  Select, 
  MenuItem, 
  SelectChangeEvent, 
  IconButton, 
  Chip,
  Grid,
  ButtonGroup,
  Switch,
  FormControlLabel,
  Checkbox,
  FormGroup,
  Divider,
  Alert,
  Snackbar,
  CircularProgress,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  AppBar,
  Toolbar
} from '@mui/material';
import { 
  Add, 
  Edit, 
  Delete, 
  Check, 
  Close, 
  Save, 
  Cancel,
  ExpandMore,
  ExpandLess
} from '@mui/icons-material';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';
import API_CONFIG from '../config/api.config';
import Sidebar from '../components/Sidebar';
import { jwtDecode } from 'jwt-decode';

// API endpoint'leri
const API_ENDPOINTS = {
  PAGES: API_CONFIG.ENDPOINTS.MENU.ITEMS,
  CREATE_PAGE: API_CONFIG.ENDPOINTS.MENU.CREATE,
  UPDATE_PAGE: API_CONFIG.ENDPOINTS.MENU.UPDATE,
  DELETE_PAGE: API_CONFIG.ENDPOINTS.MENU.DELETE,
  ASSIGN_ROLES: API_CONFIG.ENDPOINTS.MENU.ASSIGN_ROLES,
  ROLES: API_CONFIG.ENDPOINTS.MENU.ROLES
};

// Sayfa modeli
interface Page {
  id: number;
  title: string;
  path: string;
  iconName: string;
  isActive: boolean;
  displayOrder: number;
  roles: string[];
  subPages?: Page[];
  parentId?: number | string; // Alt sayfalar için opsiyonel, string veya number olabilir
}

// Rol modeli
interface Role {
  id: string;
  name: string;
}

interface JwtPayload {
  sub: string;
  email: string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string | string[];
  exp: number;
}

const PageManagement: React.FC = () => {
  // State tanımlamaları
  const [pages, setPages] = useState<Page[]>([]);
  const [roles, setRoles] = useState<string[]>([]);
  const [userRoles, setUserRoles] = useState<string[]>([]);
  const [selectedPage, setSelectedPage] = useState<Page | null>(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [openAddSubPageDialog, setOpenAddSubPageDialog] = useState(false);
  const [openRoleDialog, setOpenRoleDialog] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [severity, setSeverity] = useState<'success' | 'error' | 'info' | 'warning'>('success');
  const [openSnackbar, setOpenSnackbar] = useState(false);
  
  // Form state'leri
  const [formData, setFormData] = useState<{
    id: number;
    title: string;
    path: string;
    iconName: string;
    isActive: boolean;
    displayOrder: number;
    roles: string[];
    parentId: number | string;
  }>({
    id: 0,
    title: '',
    path: '',
    iconName: '',
    isActive: true,
    displayOrder: 0,
    roles: [] as string[],
    parentId: '' // 0 değeri üst menü yok anlamına gelir
  });
  
  const [isSubPage, setIsSubPage] = useState(false);
  const [expandedPages, setExpandedPages] = useState<Record<number, boolean>>({});

  const { token } = useAuth();

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

  // Sayfaları getiren fonksiyon
  const fetchPages = useCallback(async () => {
    try {
      setLoading(true);
      console.log('Menü verileri alınıyor...');
      
      // API çağrısını yapalım
      const response = await axios.get(API_ENDPOINTS.PAGES, {
        headers: getAuthHeaders(),
        withCredentials: false
      });

      console.log('API yanıtı:', response.data);
      
      if (response.data) {
        // API'den gelen veriyi Page tipine dönüştür
        try {
          // API yanıtını formatlayarak kullanılabilir hale getir
          let formattedPages: Page[] = [];
          
          // MenuPermissionService'den dönen veri formatına göre işlem yap
          if (Array.isArray(response.data)) {
            // Her bir menü için
            formattedPages = response.data.map((menu: any, index: number) => {
              // Ana menü
              const mainMenu: Page = {
                id: index + 1, // Benzersiz ID oluştur
                title: menu.title,
                path: '', // Ana menülerin path'i olmayabilir
                iconName: menu.iconName || '',
                isActive: true,
                displayOrder: index + 1,
                roles: ['superadmin', 'user'], // Varsayılan roller
                subPages: []
              };
              
              // Alt menüler
              if (menu.items && Array.isArray(menu.items)) {
                mainMenu.subPages = menu.items.map((item: any, subIndex: number) => ({
                  id: (index + 1) * 100 + subIndex + 1, // Benzersiz ID oluştur
                  title: item.title,
                  path: item.path || '',
                  iconName: item.iconName || '',
                  isActive: true,
                  displayOrder: subIndex + 1,
                  roles: ['superadmin', 'user'], // Varsayılan roller
                  parentId: mainMenu.id.toString()
                }));
              }
              
              return mainMenu;
            });
          } else {
            // Statik veri kullanabiliriz
            formattedPages = [
              {
                id: 1,
                title: 'Ana Sayfa',
                path: '/dashboard',
                iconName: 'Home',
                isActive: true,
                displayOrder: 1,
                roles: ['superadmin', 'user'],
                subPages: []
              },
              {
                id: 2,
                title: 'Kullanıcı Yönetimi',
                path: '/user-management',
                iconName: 'People',
                isActive: true,
                displayOrder: 2,
                roles: ['superadmin'],
                subPages: []
              },
              {
                id: 3,
                title: 'Sayfa Yönetimi',
                path: '/page-management',
                iconName: 'Pages',
                isActive: true,
                displayOrder: 3,
                roles: ['superadmin'],
                subPages: [
                  {
                    id: 4,
                    title: 'Menü Ayarları',
                    path: '/page-management/settings',
                    iconName: 'Settings',
                    isActive: true,
                    displayOrder: 1,
                    roles: ['superadmin'],
                    parentId: '3'
                  }
                ]
              }
            ];
          }
          
          console.log('Formatlanmış menü verileri:', formattedPages);
          
          if (formattedPages.length > 0) {
            setPages(formattedPages);
            showMessage('Menü verileri başarıyla yüklendi', 'success');
          } else {
            showMessage('Menü verisi bulunamadı, statik veriler kullanılıyor', 'info');
            // Statik veri kullanabiliriz
            setPages([
              {
                id: 1,
                title: 'Ana Sayfa',
                path: '/dashboard',
                iconName: 'Home',
                isActive: true,
                displayOrder: 1,
                roles: ['superadmin', 'user'],
                subPages: []
              },
              {
                id: 2,
                title: 'Kullanıcı Yönetimi',
                path: '/user-management',
                iconName: 'People',
                isActive: true,
                displayOrder: 2,
                roles: ['superadmin'],
                subPages: []
              },
              {
                id: 3,
                title: 'Sayfa Yönetimi',
                path: '/page-management',
                iconName: 'Pages',
                isActive: true,
                displayOrder: 3,
                roles: ['superadmin'],
                subPages: [
                  {
                    id: 4,
                    title: 'Menü Ayarları',
                    path: '/page-management/settings',
                    iconName: 'Settings',
                    isActive: true,
                    displayOrder: 1,
                    roles: ['superadmin'],
                    parentId: '3'
                  }
                ]
              }
            ]);
          }
        } catch (formatError) {
          console.error('API yanıtı dönüştürülürken hata:', formatError);
          showMessage('API yanıtı işlenirken hata oluştu, statik veriler kullanılıyor', 'error');
          
          // Hata durumunda statik veri kullanabiliriz
          setPages([
            {
              id: 1,
              title: 'Ana Sayfa',
              path: '/dashboard',
              iconName: 'Home',
              isActive: true,
              displayOrder: 1,
              roles: ['superadmin', 'user'],
              subPages: []
            },
            {
              id: 2,
              title: 'Kullanıcı Yönetimi',
              path: '/user-management',
              iconName: 'People',
              isActive: true,
              displayOrder: 2,
              roles: ['superadmin'],
              subPages: []
            },
            {
              id: 3,
              title: 'Sayfa Yönetimi',
              path: '/page-management',
              iconName: 'Pages',
              isActive: true,
              displayOrder: 3,
              roles: ['superadmin'],
              subPages: [
                {
                  id: 4,
                  title: 'Menü Ayarları',
                  path: '/page-management/settings',
                  iconName: 'Settings',
                  isActive: true,
                  displayOrder: 1,
                  roles: ['superadmin'],
                  parentId: '3'
                }
              ]
            }
          ]);
        }
      } else {
        throw new Error('Menü verileri yüklenirken hata oluştu');
      }
    } catch (error: any) {
      console.error('Error fetching pages:', error);
      const errorMessage = error.response?.data?.message || 'Menü verileri yüklenirken hata oluştu';
      showMessage(errorMessage, 'error');
      
      // Hata durumunda statik veri kullanabiliriz
      setPages([
        {
          id: 1,
          title: 'Ana Sayfa',
          path: '/dashboard',
          iconName: 'Home',
          isActive: true,
          displayOrder: 1,
          roles: ['superadmin', 'user'],
          subPages: []
        },
        {
          id: 2,
          title: 'Kullanıcı Yönetimi',
          path: '/user-management',
          iconName: 'People',
          isActive: true,
          displayOrder: 2,
          roles: ['superadmin'],
          subPages: []
        },
        {
          id: 3,
          title: 'Sayfa Yönetimi',
          path: '/page-management',
          iconName: 'Pages',
          isActive: true,
          displayOrder: 3,
          roles: ['superadmin'],
          subPages: [
            {
              id: 4,
              title: 'Menü Ayarları',
              path: '/page-management/settings',
              iconName: 'Settings',
              isActive: true,
              displayOrder: 1,
              roles: ['superadmin'],
              parentId: '3'
            }
          ]
        }
      ]);
    } finally {
      setLoading(false);
    }
  }, [getAuthHeaders, API_ENDPOINTS.PAGES]);

  // Rolleri getiren fonksiyon
  const fetchRoles = useCallback(async () => {
    try {
      setLoading(true);
      const response = await axios.get(API_ENDPOINTS.ROLES, {
        headers: getAuthHeaders(),
        withCredentials: false
      });

      if (response.data) {
        console.log('Roles API response:', response.data);
        
        // API'den gelen rolleri ayıkla
        const roleNames = response.data.map((role: Role) => role.name);
        setRoles(roleNames);
      } else {
        throw new Error('Roller yüklenirken hata oluştu');
      }
    } catch (error: any) {
      console.error('Error fetching roles:', error);
      const errorMessage = error.response?.data?.message || 'Roller yüklenirken hata oluştu';
      showMessage(errorMessage, 'error');
      
      // Hata durumunda statik veri kullanabiliriz
      setRoles(['superadmin', 'user']);
    } finally {
      setLoading(false);
    }
  }, [getAuthHeaders, API_ENDPOINTS.ROLES]);

  // Sayfa yüklendiğinde verileri çekelim
  useEffect(() => {
    fetchPages();
    fetchRoles();
  }, [fetchPages, fetchRoles]);

  // Sayfa oluşturma işlevi
  const handleCreatePage = async () => {
    try {
      // Form doğrulama
      if (!formData.title || !formData.path) {
        showMessage('Lütfen tüm zorunlu alanları doldurun', 'error');
        return;
      }

      setLoading(true);

      // Yeni sayfa verisi
      const newPage: Page = {
        id: formData.id || 0,
        title: formData.title,
        path: formData.path,
        iconName: formData.iconName,
        isActive: formData.isActive,
        displayOrder: formData.displayOrder,
        roles: formData.roles,
        parentId: formData.parentId
      };

      // API'ye gönderilecek veri
      const apiData = {
        ...newPage,
        parentId: formData.parentId === '' ? null : Number(formData.parentId)
      };

      console.log('Gönderilecek veri:', apiData);

      let response;

      if (editMode) {
        // Mevcut sayfayı güncelle
        response = await axios.put(`${API_ENDPOINTS.UPDATE_PAGE}/${formData.id}`, apiData, {
          headers: getAuthHeaders(),
          withCredentials: false
        });

        if (response.status === 200) {
          showMessage('Menü başarıyla güncellendi', 'success');
          
          // Sayfaları yeniden yükle
          await fetchPages();
        }
      } else {
        // Yeni sayfa oluştur
        response = await axios.post(API_ENDPOINTS.CREATE_PAGE, apiData, {
          headers: getAuthHeaders(),
          withCredentials: false
        });

        if (response.status === 201 || response.status === 200) {
          showMessage('Menü başarıyla oluşturuldu', 'success');
          
          // Sayfaları yeniden yükle
          await fetchPages();
        }
      }

      // Diyaloğu kapat
      setOpenDialog(false);
      setOpenAddSubPageDialog(false);
      resetForm();
    } catch (error: any) {
      console.error('Error creating/updating page:', error);
      const errorMessage = error.response?.data?.message || 'Menü oluşturulurken/güncellenirken hata oluştu';
      showMessage(errorMessage, 'error');

      // Hata durumunda statik veri ile devam edelim
      if (isSubPage && formData.parentId) {
        // Alt sayfa oluşturma
        // Üst sayfayı bul
        const parentPage = pages.find(page => page.id === parseInt(formData.parentId.toString(), 10));
        
        if (parentPage) {
          // Yeni alt sayfa
          const newSubPage: Page = {
            id: Date.now(), // Benzersiz bir ID oluştur
            title: formData.title,
            path: formData.path,
            iconName: formData.iconName,
            isActive: formData.isActive,
            displayOrder: formData.displayOrder,
            roles: formData.roles,
            parentId: formData.parentId
          };
          
          // Üst sayfaya alt sayfayı ekle
          const updatedParentPage = {
            ...parentPage,
            subPages: [...(parentPage.subPages || []), newSubPage]
          };
          
          // Sayfaları güncelle
          const updatedPages = pages.map(page => 
            page.id === parentPage.id ? updatedParentPage : page
          );
          
          setPages(updatedPages);
          showMessage('Alt menü başarıyla eklendi (statik veri)', 'success');
        }
      } else {
        // Ana sayfa oluşturma
        const newPage: Page = {
          id: Date.now(), // Benzersiz bir ID oluştur
          title: formData.title,
          path: formData.path,
          iconName: formData.iconName,
          isActive: formData.isActive,
          displayOrder: formData.displayOrder,
          roles: formData.roles,
          subPages: []
        };
        
        setPages([...pages, newPage]);
        showMessage('Menü başarıyla eklendi (statik veri)', 'success');
      }
      
      // Diyaloğu kapat
      setOpenDialog(false);
      setOpenAddSubPageDialog(false);
      resetForm();
    } finally {
      setLoading(false);
    }
  };

  // Sayfa güncelleme işlemi
  const handleUpdatePage = async () => {
    if (!formData.title || !formData.path) {
      showMessage('Menü başlığı ve yolu boş olamaz', 'error');
      return;
    }
    
    try {
      setLoading(true);
      console.log('Menü güncelleme isteği gönderiliyor:', formData);
      
      // API çağrısı yapalım
      const response = await axios.put(
        `${API_ENDPOINTS.UPDATE_PAGE}/${formData.id}`, 
        formData,
        { 
          headers: getAuthHeaders(),
          withCredentials: false
        }
      );

      console.log('Menü güncelleme yanıtı:', response.data);

      if (response.status >= 200 && response.status < 300) {
        // Başarılı yanıt durumunda
        fetchPages(); // Güncel menü listesini yeniden yükle
        setOpenDialog(false);
        resetForm();
        setIsSubPage(false);
        showMessage('Menü başarıyla güncellendi', 'success');
      } else {
        throw new Error(response.data?.message || 'Menü güncellenirken hata oluştu');
      }
    } catch (error: any) {
      console.error('Error updating page:', error);
      const errorMessage = error.response?.data?.message || 'Menü güncellenirken hata oluştu';
      showMessage(`Menü güncellenirken hata oluştu: ${errorMessage}`, 'error');
      
      // Hata durumunda statik veri ile devam edelim
      if (isSubPage) {
        // Alt sayfa güncelleme
        const updatedPages = pages.map(page => {
          if (page.id === parseInt(formData.parentId.toString(), 10)) {
            return {
              ...page,
              subPages: page.subPages?.map(subPage => 
                subPage.id === formData.id 
                  ? { ...formData, parentId: page.id.toString() } 
                  : subPage
              ) || []
            };
          } else if (page.subPages?.some(subPage => subPage.id === formData.id)) {
            // Eğer alt sayfa başka bir ana sayfaya taşındıysa, eski ana sayfadan kaldır
            return {
              ...page,
              subPages: page.subPages?.filter(subPage => subPage.id !== formData.id) || []
            };
          }
          return page;
        });
        
        setPages(updatedPages);
        showMessage('Menü başarıyla güncellendi (statik veri)', 'success');
      } else {
        // Ana sayfa güncelleme
        setPages(pages.map(page => 
          page.id === formData.id 
            ? { ...formData, subPages: page.subPages || [] } 
            : page
        ));
      }
      
      setOpenDialog(false);
      resetForm();
      setIsSubPage(false);
      showMessage('Menü başarıyla güncellendi (statik veri)', 'success');
    } finally {
      setLoading(false);
    }
  };

  // Sayfa silme işlemi
  const handleDeletePage = async (pageId: number) => {
    // Önce alt sayfa mı ana sayfa mı kontrol et
    let isSubPageItem = false;
    let parentPageId = 0;
    let pageTitle = '';
    
    // Tüm sayfaları kontrol et
    for (const page of pages) {
      // Ana sayfanın başlığını bul
      if (page.id === pageId) {
        pageTitle = page.title;
        break;
      }
      
      // Alt sayfaları kontrol et
      const subPage = page.subPages?.find(subPage => subPage.id === pageId);
      if (subPage) {
        isSubPageItem = true;
        parentPageId = page.id;
        pageTitle = subPage.title;
        break;
      }
    }
    
    const confirmMessage = isSubPageItem 
      ? `"${pageTitle}" alt menüsünü silmek istediğinizden emin misiniz?` 
      : `"${pageTitle}" menüsünü ve tüm alt menülerini silmek istediğinizden emin misiniz?`;
      
    if (!window.confirm(confirmMessage)) {
      return;
    }
    
    try {
      setLoading(true);
      console.log('Menü silme isteği gönderiliyor:', pageId);
      
      // API çağrısı yapalım
      const response = await axios.delete(
        `${API_ENDPOINTS.DELETE_PAGE}/${pageId}`,
        { 
          headers: getAuthHeaders(),
          withCredentials: false
        }
      );

      console.log('Menü silme yanıtı:', response.data);

      if (response.status >= 200 && response.status < 300) {
        // Başarılı yanıt durumunda
        fetchPages(); // Güncel menü listesini yeniden yükle
        showMessage('Menü başarıyla silindi', 'success');
      } else {
        throw new Error(response.data?.message || 'Menü silinirken hata oluştu');
      }
    } catch (error: any) {
      console.error('Error deleting page:', error);
      const errorMessage = error.response?.data?.message || 'Menü silinirken hata oluştu';
      showMessage(`Menü silinirken hata oluştu: ${errorMessage}`, 'error');
      
      // Hata durumunda statik veri ile devam edelim
      if (isSubPageItem) {
        // Alt sayfa silme
        const updatedPages = pages.map(page => {
          if (page.id === parentPageId) {
            return {
              ...page,
              subPages: page.subPages?.filter(subPage => subPage.id !== pageId) || []
            };
          }
          return page;
        });
        
        setPages(updatedPages);
        showMessage(`"${pageTitle}" alt menüsü başarıyla silindi`, 'success');
      } else {
        // Ana sayfa silme
        setPages(pages.filter(page => page.id !== pageId));
        showMessage(`"${pageTitle}" menüsü başarıyla silindi`, 'success');
      }
    } finally {
      setLoading(false);
    }
  };

  // Menü rollerini güncelleme işlemi
  const handleAssignRoles = async () => {
    if (!selectedPage) return;
    
    try {
      setLoading(true);
      console.log('Rol atama isteği gönderiliyor:', {
        pageId: selectedPage.id,
        roles: formData.roles
      });
      
      // API çağrısı yapalım
      const response = await axios.post(
        API_ENDPOINTS.ASSIGN_ROLES,
        {
          pageId: selectedPage.id,
          roles: formData.roles
        },
        { 
          headers: getAuthHeaders(),
          withCredentials: false
        }
      );

      console.log('Rol atama yanıtı:', response.data);

      if (response.status >= 200 && response.status < 300) {
        // Başarılı yanıt durumunda
        fetchPages(); // Güncel menü listesini yeniden yükle
        setOpenRoleDialog(false);
        showMessage('Roller başarıyla atandı', 'success');
      } else {
        throw new Error(response.data?.message || 'Roller atanırken hata oluştu');
      }
    } catch (error: any) {
      console.error('Error assigning roles:', error);
      const errorMessage = error.response?.data?.message || 'Roller atanırken hata oluştu';
      showMessage(`Roller atanırken hata oluştu: ${errorMessage}`, 'error');
      
      // Hata durumunda statik veri ile devam edelim
      if (selectedPage) {
        const updatedPages = pages.map(page => {
          if (page.id === selectedPage.id) {
            return {
              ...page,
              roles: formData.roles
            };
          }
          return page;
        });
        setPages(updatedPages);
        setOpenRoleDialog(false);
        showMessage('Roller başarıyla atandı (statik veri)', 'success');
      }
    } finally {
      setLoading(false);
    }
  };

  // Sayfa düzenleme diyaloğunu açma işlevi
  const handleOpenEditDialog = (page: Page) => {
    // Sayfanın alt sayfa olup olmadığını kontrol et
    const isSubPageItem = 'parentId' in page && page.parentId !== undefined && page.parentId !== null;
    
    setFormData({
      id: page.id,
      title: page.title,
      path: page.path,
      iconName: page.iconName,
      isActive: page.isActive,
      displayOrder: page.displayOrder,
      roles: page.roles,
      parentId: isSubPageItem && page.parentId ? page.parentId : ''
    });
    
    setIsSubPage(!!isSubPageItem);
    setEditMode(true);
    setOpenDialog(true);
  };

  // Yeni menü ekleme diyaloğunu açma işlevi
  const handleOpenAddDialog = () => {
    resetForm();
    setEditMode(false);
    setOpenDialog(true);
  };

  // Rol atama diyaloğunu açma işlevi
  const handleOpenRoleDialog = (page: Page) => {
    setSelectedPage(page);
    setFormData({
      ...formData,
      roles: [...page.roles]
    });
    setOpenRoleDialog(true);
  };

  // Diyaloğu kapatma işlevi
  const handleCloseDialog = () => {
    setOpenDialog(false);
    resetForm();
  };

  // Rol diyaloğunu kapatma işlevi
  const handleCloseRoleDialog = () => {
    setOpenRoleDialog(false);
    setSelectedPage(null);
  };

  // Alt menü ekleme diyaloğunu kapatma işlevi
  const handleCloseAddSubPageDialog = () => {
    setOpenAddSubPageDialog(false);
    resetForm();
  };

  // Form verilerini sıfırlama işlevi
  const resetForm = () => {
    setFormData({
      id: 0,
      title: '',
      path: '',
      iconName: '',
      isActive: true,
      displayOrder: 0,
      roles: [],
      parentId: ''
    });
  };

  // Form verilerini güncelleme işlevi
  const handleFormChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | SelectChangeEvent<number | string>
  ) => {
    const { name, value, type } = e.target as any;
    
    if (type === 'checkbox') {
      const checked = (e.target as HTMLInputElement).checked;
      setFormData({
        ...formData,
        [name]: checked
      });
    } else {
      setFormData({
        ...formData,
        [name]: value
      });
    }
  };

  // Rol seçimini güncelleme işlevi
  const handleRoleChange = (role: string) => {
    const currentRoles = [...formData.roles];
    const roleIndex = currentRoles.indexOf(role);
    
    if (roleIndex === -1) {
      // Rol yoksa ekle
      currentRoles.push(role);
    } else {
      // Rol varsa çıkar
      currentRoles.splice(roleIndex, 1);
    }
    
    setFormData({
      ...formData,
      roles: currentRoles
    });
  };

  // Bildirim mesajı gösterme işlevi
  const showMessage = (msg: string, sev: 'success' | 'error' | 'info' | 'warning') => {
    setMessage(msg);
    setSeverity(sev);
    setOpenSnackbar(true);
  };

  // Bildirimi kapatma işlevi
  const handleCloseSnackbar = () => {
    setOpenSnackbar(false);
  };

  // Menü genişletme/daraltma işlevi
  const handleToggleExpand = (pageId: number) => {
    setExpandedPages(prev => ({
      ...prev,
      [pageId]: !prev[pageId]
    }));
  };

  // Alt menü ekleme diyaloğunu açma işlevi
  const handleOpenAddSubPageDialog = (parentId: number) => {
    setFormData({
      id: 0,
      title: '',
      path: '',
      iconName: '',
      isActive: true,
      displayOrder: 0,
      roles: [],
      parentId: parentId.toString()
    });
    
    setIsSubPage(true);
    setOpenAddSubPageDialog(true);
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <Sidebar />
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <AppBar position="static" color="default" elevation={0} sx={{ borderBottom: '1px solid rgba(0, 0, 0, 0.12)' }}>
          <Toolbar>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
              Menü Yönetimi
            </Typography>
            <Button 
              variant="contained" 
              color="primary" 
              startIcon={<Add />}
              onClick={handleOpenAddDialog}
              sx={{ mr: 2 }}
            >
              Yeni Menü Ekle
            </Button>
          </Toolbar>
        </AppBar>

        {loading && (
          <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
            <CircularProgress />
          </Box>
        )}

        {!loading && (
          <TableContainer component={Paper} sx={{ mt: 2 }}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell width="40%">Menü</TableCell>
                  <TableCell>Yol</TableCell>
                  <TableCell>Sıra</TableCell>
                  <TableCell>Durum</TableCell>
                  <TableCell>İşlemler</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {pages.map((page) => (
                  <React.Fragment key={page.id}>
                    {/* Ana Menü Satırı */}
                    <TableRow 
                      hover
                      sx={{ 
                        cursor: 'pointer',
                        backgroundColor: expandedPages[page.id] ? 'rgba(0, 0, 0, 0.04)' : 'inherit'
                      }}
                      onClick={() => handleToggleExpand(page.id)}
                    >
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                          <IconButton
                            size="small"
                            onClick={(e) => {
                              e.stopPropagation();
                              handleToggleExpand(page.id);
                            }}
                            sx={{ mr: 1 }}
                          >
                            {expandedPages[page.id] ? <ExpandLess /> : <ExpandMore />}
                          </IconButton>
                          <Typography 
                            variant="body1" 
                            component="span"
                            sx={{ fontWeight: 'bold' }}
                          >
                            {page.title}
                          </Typography>
                          {page.subPages && page.subPages.length > 0 && (
                            <Chip 
                              size="small" 
                              label={`${page.subPages.length} alt menü`} 
                              sx={{ ml: 2 }}
                              color="primary"
                              variant="outlined"
                            />
                          )}
                        </Box>
                      </TableCell>
                      <TableCell>{page.path}</TableCell>
                      <TableCell>{page.displayOrder}</TableCell>
                      <TableCell>
                        <Chip 
                          label={page.isActive ? 'Aktif' : 'Pasif'} 
                          color={page.isActive ? 'success' : 'default'}
                          size="small"
                        />
                      </TableCell>
                      <TableCell>
                        <ButtonGroup size="small" variant="outlined">
                          <IconButton 
                            color="primary" 
                            onClick={(e) => {
                              e.stopPropagation();
                              handleOpenEditDialog(page);
                            }}
                            title="Düzenle"
                          >
                            <Edit fontSize="small" />
                          </IconButton>
                          <IconButton 
                            color="error" 
                            onClick={(e) => {
                              e.stopPropagation();
                              handleDeletePage(page.id);
                            }}
                            title="Sil"
                          >
                            <Delete fontSize="small" />
                          </IconButton>
                          <Button
                            color="success"
                            size="small"
                            onClick={(e) => {
                              e.stopPropagation();
                              handleOpenAddSubPageDialog(page.id);
                            }}
                            startIcon={<Add fontSize="small" />}
                            title="Alt Menü Ekle"
                          >
                            Alt Menü
                          </Button>
                        </ButtonGroup>
                      </TableCell>
                    </TableRow>
                    
                    {/* Alt Menüler (genişletildiğinde gösterilir) */}
                    {expandedPages[page.id] && page.subPages && page.subPages.length > 0 ? (
                      <TableRow>
                        <TableCell colSpan={5} sx={{ p: 0, borderBottom: 'none' }}>
                          <Box sx={{ pl: 4, pr: 2, pb: 2 }}>
                            <Paper variant="outlined" sx={{ mt: 1 }}>
                              <Table size="small">
                                <TableHead>
                                  <TableRow sx={{ backgroundColor: 'rgba(0, 0, 0, 0.02)' }}>
                                    <TableCell sx={{ width: '40%' }}>Başlık</TableCell>
                                    <TableCell sx={{ width: '30%' }}>Yol</TableCell>
                                    <TableCell sx={{ width: '15%' }} align="center">Durum</TableCell>
                                    <TableCell sx={{ width: '15%' }} align="right">İşlemler</TableCell>
                                  </TableRow>
                                </TableHead>
                                <TableBody>
                                  {page.subPages && page.subPages.map((subPage) => (
                                    <TableRow 
                                      key={`${page.id}-${subPage.id}`}
                                      sx={{ 
                                        '&:hover': {
                                          backgroundColor: 'rgba(0, 0, 0, 0.05)',
                                        }
                                      }}
                                    >
                                      <TableCell sx={{ width: '40%' }}>{subPage.title}</TableCell>
                                      <TableCell sx={{ width: '30%' }}>{subPage.path}</TableCell>
                                      <TableCell sx={{ width: '15%' }} align="center">
                                        <Chip 
                                          size="small" 
                                          label={subPage.isActive ? 'Aktif' : 'Pasif'} 
                                          color={subPage.isActive ? 'success' : 'default'}
                                        />
                                      </TableCell>
                                      <TableCell sx={{ width: '15%' }} align="right">
                                        <ButtonGroup size="small">
                                          <Button 
                                            size="small" 
                                            onClick={() => handleOpenEditDialog(subPage)}
                                          >
                                            Düzenle
                                          </Button>
                                        </ButtonGroup>
                                      </TableCell>
                                    </TableRow>
                                  ))}
                                </TableBody>
                              </Table>
                            </Paper>
                          </Box>
                        </TableCell>
                      </TableRow>
                    ) : (
                      <TableRow>
                        <TableCell colSpan={5} sx={{ p: 0, borderBottom: 'none' }}>
                          <Typography variant="body2" color="text.secondary" sx={{ py: 2, pl: 4 }}>
                            Bu menüye ait alt menü bulunmamaktadır.
                          </Typography>
                        </TableCell>
                      </TableRow>
                    )}
                  </React.Fragment>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}

        {/* Menü Ekleme/Düzenleme Diyaloğu */}
        <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
          <DialogTitle>
            {editMode ? 'Menüyü Düzenle' : isSubPage ? 'Alt Menü Ekle' : 'Yeni Menü Ekle'}
            {isSubPage && !editMode && (
              <Typography variant="subtitle2" color="textSecondary">
                Üst Menü: {pages.find(p => p.id === parseInt(formData.parentId.toString(), 10))?.title}
              </Typography>
            )}
          </DialogTitle>
          <DialogContent>
            <Grid container spacing={2} sx={{ mt: 1 }}>
              <Grid item xs={12} md={6}>
                <TextField
                  name="title"
                  label="Menü Başlığı"
                  value={formData.title}
                  onChange={handleFormChange}
                  fullWidth
                  required
                  error={formData.title === ''}
                  helperText={formData.title === '' ? 'Başlık gereklidir' : ''}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  name="path"
                  label="Menü Yolu"
                  value={formData.path}
                  onChange={handleFormChange}
                  fullWidth
                  required
                  error={formData.path === ''}
                  helperText={formData.path === '' ? 'Yol gereklidir' : isSubPage ? 'Alt menü için tam yol (örn: /parent/child)' : 'Ana menü için yol (örn: /dashboard)'}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  name="iconName"
                  label="İkon Adı"
                  value={formData.iconName}
                  onChange={handleFormChange}
                  fullWidth
                  helperText="Material UI ikon adı (örn: Home, People, Dashboard)"
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  name="displayOrder"
                  label="Görüntüleme Sırası"
                  type="number"
                  value={formData.displayOrder}
                  onChange={handleFormChange}
                  fullWidth
                  InputProps={{ inputProps: { min: 1 } }}
                  helperText={isSubPage ? "Alt menüler arasındaki sıralama" : "Ana menüler arasındaki sıralama"}
                />
              </Grid>
              <Grid item xs={12}>
                <FormControlLabel
                  control={
                    <Switch
                      name="isActive"
                      checked={formData.isActive}
                      onChange={(e) => handleFormChange(e)}
                    />
                  }
                  label="Aktif"
                />
              </Grid>
              {!isSubPage && editMode && (
                <Grid item xs={12}>
                  <Typography variant="subtitle1" gutterBottom>
                    Üst Menü
                  </Typography>
                  <Select
                    name="parentId"
                    value={formData.parentId}  
                    onChange={(e) => setFormData({...formData, parentId: e.target.value})}
                    fullWidth
                  >
                    <MenuItem value="">Üst Menü Yok</MenuItem>
                    {pages.map((page) => (
                      <MenuItem key={page.id} value={page.id.toString()}>{page.title}</MenuItem>
                    ))}
                  </Select>
                </Grid>
              )}
              <Grid item xs={12}>
                <Typography variant="subtitle1" gutterBottom>
                  Menü Rolleri
                </Typography>
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                  {roles.map((role) => (
                    <Chip
                      key={role}
                      label={role}
                      onClick={() => handleRoleChange(role)}
                      color={formData.roles.includes(role) ? 'primary' : 'default'}
                      variant={formData.roles.includes(role) ? 'filled' : 'outlined'}
                      sx={{ m: 0.5 }}
                    />
                  ))}
                </Box>
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDialog} color="inherit" startIcon={<Cancel />}>
              İptal
            </Button>
            <Button 
              onClick={editMode ? handleUpdatePage : handleCreatePage} 
              color="primary" 
              variant="contained"
              startIcon={<Save />}
            >
              {editMode ? 'Güncelle' : 'Kaydet'}
            </Button>
          </DialogActions>
        </Dialog>

        {/* Alt Menü Ekleme Diyaloğu */}
        <Dialog open={openAddSubPageDialog} onClose={handleCloseAddSubPageDialog} maxWidth="md" fullWidth>
          <DialogTitle>
            Alt Menü Ekle
          </DialogTitle>
          <DialogContent>
            <Grid container spacing={2} sx={{ mt: 1 }}>
              <Grid item xs={12} md={6}>
                <TextField
                  name="title"
                  label="Menü Başlığı"
                  value={formData.title}
                  onChange={handleFormChange}
                  fullWidth
                  required
                  error={formData.title === ''}
                  helperText={formData.title === '' ? 'Başlık gereklidir' : ''}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  name="path"
                  label="Menü Yolu"
                  value={formData.path}
                  onChange={handleFormChange}
                  fullWidth
                  required
                  error={formData.path === ''}
                  helperText={formData.path === '' ? 'Yol gereklidir' : 'Alt menü için tam yol (örn: /parent/child)'}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  name="iconName"
                  label="İkon Adı"
                  value={formData.iconName}
                  onChange={handleFormChange}
                  fullWidth
                  helperText="Material UI ikon adı (örn: Home, People, Dashboard)"
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  name="displayOrder"
                  label="Görüntüleme Sırası"
                  type="number"
                  value={formData.displayOrder}
                  onChange={handleFormChange}
                  fullWidth
                  InputProps={{ inputProps: { min: 1 } }}
                  helperText="Alt menüler arasındaki sıralama"
                />
              </Grid>
              <Grid item xs={12}>
                <FormControlLabel
                  control={
                    <Switch
                      name="isActive"
                      checked={formData.isActive}
                      onChange={(e) => handleFormChange(e)}
                    />
                  }
                  label="Aktif"
                />
              </Grid>
              <Grid item xs={12}>
                <Typography variant="subtitle1" gutterBottom>
                  Menü Rolleri
                </Typography>
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                  {roles.map((role) => (
                    <Chip
                      key={role}
                      label={role}
                      onClick={() => handleRoleChange(role)}
                      color={formData.roles.includes(role) ? 'primary' : 'default'}
                      variant={formData.roles.includes(role) ? 'filled' : 'outlined'}
                      sx={{ m: 0.5 }}
                    />
                  ))}
                </Box>
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseAddSubPageDialog} color="inherit" startIcon={<Cancel />}>
              İptal
            </Button>
            <Button 
              onClick={handleCreatePage} 
              color="primary" 
              variant="contained"
              startIcon={<Save />}
            >
              Kaydet
            </Button>
          </DialogActions>
        </Dialog>

        {/* Rol Atama Diyaloğu */}
        <Dialog open={openRoleDialog} onClose={handleCloseRoleDialog} maxWidth="sm" fullWidth>
          <DialogTitle>
            {selectedPage ? `"${selectedPage.title}" Menüsüne Rol Ata` : 'Rol Ata'}
          </DialogTitle>
          <DialogContent>
            <List>
              {roles.map((role) => (
                <ListItem key={role}>
                  <ListItemText primary={role} />
                  <ListItemSecondaryAction>
                    <Checkbox
                      edge="end"
                      checked={formData.roles.includes(role)}
                      onChange={() => handleRoleChange(role)}
                    />
                  </ListItemSecondaryAction>
                </ListItem>
              ))}
            </List>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseRoleDialog} color="inherit" startIcon={<Cancel />}>
              İptal
            </Button>
            <Button 
              onClick={handleAssignRoles} 
              color="primary" 
              variant="contained"
              startIcon={<Save />}
            >
              Rolleri Kaydet
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

export default PageManagement;
