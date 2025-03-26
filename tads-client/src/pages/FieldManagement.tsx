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
  CircularProgress,
  Grid,
  IconButton,
  FormControlLabel,
  Switch,
  Tab,
  Tabs,
  Autocomplete,
  DialogContentText
} from '@mui/material';
import { Add, Edit, Delete, Map, Agriculture, TerrainOutlined, LocationOn, Close } from '@mui/icons-material';
import Sidebar from '../components/Sidebar';
import { useAuth } from '../contexts/AuthContext';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import API_CONFIG from '../config/api.config';

// Parsel/Alan için arayüz tanımı
interface Field {
  id: number;
  name: string;
  area: number; // Dönüm cinsinden alan
  location: string;
  latitude?: number;
  longitude?: number;
  soilTypeId: number;
  soilType: string;
  isActive: boolean;
  cropType?: string;
  cropTypeId?: number;
  plantingDate?: string;
  harvestDate?: string;
  notes?: string;
}

// Toprak türleri için arayüz tanımı
interface SoilType {
  id: number;
  name: string;
  description: string;
}

// Ekin türleri için arayüz tanımı
interface CropType {
  id: number;
  name: string;
  description: string;
  growingSeason?: string;
  growingDays?: number;
}

// Ekin türleri için sabit değerler ve ID'leri
const CROP_TYPES = [
  'Buğday',
  'Arpa',
  'Mısır',
  'Pamuk',
  'Ayçiçeği',
  'Domates',
  'Patates',
  'Soğan',
  'Çeltik (Pirinç)',
  'Nohut',
  'Mercimek',
  'Fasulye'
];

// Ekin türü adından ID'ye dönüşüm için yardımcı fonksiyon
const getCropTypeIdByName = (name: string | undefined): number | null => {
  if (!name) return null;
  const index = CROP_TYPES.findIndex(crop => crop === name);
  return index !== -1 ? index + 1 : null;
};

interface JwtPayload {
  sub: string;
  email: string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string | string[];
  exp: number;
}

const FieldManagement: React.FC = () => {
  const [fields, setFields] = useState<Field[]>([]);
  const [openDialog, setOpenDialog] = useState(false);
  const [selectedField, setSelectedField] = useState<Field | null>(null);
  const [soilTypes, setSoilTypes] = useState<SoilType[]>([]);
  const [cropTypes, setCropTypes] = useState<CropType[]>([]);
  const [openAddSoilTypeDialog, setOpenAddSoilTypeDialog] = useState(false);
  const [newSoilType, setNewSoilType] = useState({ name: '', description: '' });
  const [openMapModal, setOpenMapModal] = useState(false);
  const [mapField, setMapField] = useState<Field | null>(null);
  const [formData, setFormData] = useState<Field>({
    id: 0,
    name: '',
    area: 0,
    location: '',
    latitude: 0,
    longitude: 0,
    soilTypeId: 0,
    soilType: '',
    isActive: true,
    cropType: '',
    cropTypeId: 0,
    plantingDate: '',
    harvestDate: '',
    notes: ''
  });
  const [message, setMessage] = useState('');
  const [severity, setSeverity] = useState<'success' | 'error'>('success');
  const [openSnackbar, setOpenSnackbar] = useState(false);
  const [loading, setLoading] = useState(false);
  const [userRoles, setUserRoles] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<'table' | 'map'>('table');
  const { token } = useAuth();

  // API URL'lerini sabit olarak tanımlayalım
  const API_ENDPOINTS = {
    FIELDS: API_CONFIG.ENDPOINTS.FIELD_MANAGEMENT.FIELDS,
    FIELD: API_CONFIG.ENDPOINTS.FIELD_MANAGEMENT.FIELD,
    SOIL_TYPES: API_CONFIG.ENDPOINTS.FIELD_MANAGEMENT.SOIL_TYPES,
    SOIL_TYPE: API_CONFIG.ENDPOINTS.FIELD_MANAGEMENT.SOIL_TYPE,
    CROP_TYPES: API_CONFIG.ENDPOINTS.FIELD_MANAGEMENT.CROP_TYPES,
  };

  // Token'dan kullanıcı rollerini çıkaran fonksiyon
  useEffect(() => {
    if (token) {
      try {
        const decoded = jwtDecode<JwtPayload>(token);
        const roles = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        
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
    return {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    };
  }, [token]);

  // Mesaj gösterme fonksiyonu
  const showMessage = (msg: string, sev: 'success' | 'error') => {
    setMessage(msg);
    setSeverity(sev);
    setOpenSnackbar(true);
  };

  // Parselleri getiren fonksiyon
  const fetchFields = useCallback(async () => {
    try {
      setLoading(true);
      // API çağrısını deneyelim
      const response = await axios.get(API_ENDPOINTS.FIELDS, {
        headers: getAuthHeaders()
      });
      
      if (response.data) {
        setFields(response.data);
        showMessage('Parseller başarıyla yüklendi', 'success');
      }
    } catch (error: any) {
      console.error('Error fetching fields:', error);
      const errorMessage = error.response?.data?.message || 'Parseller veritabanından yüklenemedi';
      showMessage(`${errorMessage}. Örnek veriler gösteriliyor.`, 'error');
      
      // Hata durumunda örnek verileri göster
      setFields([
        {
          id: 1,
          name: 'Kuzey Tarla',
          area: 45.5,
          location: 'Ankara, Polatlı',
          latitude: 39.5833,
          longitude: 32.1167,
          soilTypeId: 1,
          soilType: 'Killi Toprak',
          isActive: true,
          cropType: 'Buğday',
          plantingDate: '2024-10-15',
          harvestDate: '2025-06-20',
          notes: 'Sulama sistemi yenilendi'
        },
        {
          id: 2,
          name: 'Güney Bahçe',
          area: 12.8,
          location: 'Ankara, Polatlı',
          latitude: 39.5667,
          longitude: 32.1333,
          soilTypeId: 2,
          soilType: 'Humuslu Toprak',
          isActive: true,
          cropType: 'Domates',
          plantingDate: '2024-04-10',
          harvestDate: '2024-08-15',
          notes: 'Organik tarım yapılıyor'
        },
        {
          id: 3,
          name: 'Batı Parsel',
          area: 28.3,
          location: 'Ankara, Haymana',
          latitude: 39.4333,
          longitude: 32.6333,
          soilTypeId: 3,
          soilType: 'Tınlı Toprak',
          isActive: false,
          cropType: 'Mısır',
          plantingDate: '2024-05-01',
          harvestDate: '2024-09-30',
          notes: 'Nadasa bırakıldı'
        },
        {
          id: 4,
          name: 'Doğu Bağ',
          area: 8.2,
          location: 'Ankara, Beypazarı',
          latitude: 40.1667,
          longitude: 31.9167,
          soilTypeId: 4,
          soilType: 'Kireçli Toprak',
          isActive: true,
          cropType: 'Üzüm',
          plantingDate: '2023-03-15',
          harvestDate: '2024-09-10',
          notes: 'Yeni asma dikimi yapıldı'
        }
      ]);
    } finally {
      setLoading(false);
    }
  }, [API_ENDPOINTS.FIELDS, getAuthHeaders]);

  // Toprak türlerini ve ekin türlerini getiren fonksiyon
  const fetchSoilAndCropTypes = useCallback(async () => {
    try {
      // Toprak türlerini getir
      const soilResponse = await axios.get(API_ENDPOINTS.SOIL_TYPES, {
        headers: getAuthHeaders()
      });
      setSoilTypes(soilResponse.data);

      // Ekin türlerini getir
      const cropResponse = await axios.get(API_ENDPOINTS.CROP_TYPES, {
        headers: getAuthHeaders()
      });
      setCropTypes(cropResponse.data);
    } catch (error) {
      console.error('Error fetching soil or crop types:', error);
      showMessage('Toprak veya ekin türleri yüklenirken bir hata oluştu', 'error');
    }
  }, [getAuthHeaders]);

  // Sayfa yüklendiğinde parselleri ve toprak türlerini getir
  useEffect(() => {
    if (token) {
      fetchFields();
      fetchSoilAndCropTypes();
    }
  }, [token, fetchFields, fetchSoilAndCropTypes]);

  // Toprak türü adından ID'sini getiren fonksiyon
  const getSoilTypeIdByName = (name: string | undefined): number | null => {
    if (!name) return null;
    const soilType = soilTypes.find(st => st.name.toLowerCase() === name.toLowerCase());
    return soilType ? soilType.id : null;
  };

  // Toprak türü ID'sinden adını getiren fonksiyon
  const getSoilTypeNameById = (id: number | undefined): string => {
    if (!id) return '';
    const soilType = soilTypes.find(st => st.id === id);
    return soilType ? soilType.name : '';
  };

  // Ekin türü ID'sinden adını getiren fonksiyon
  const getCropTypeNameById = (id: number | undefined): string => {
    if (!id) return '';
    const cropType = cropTypes.find(ct => ct.id === id);
    return cropType ? cropType.name : '';
  };

  // Ekin türü adından ID'sini getiren fonksiyon
  const getCropTypeIdByName = (name: string | undefined): number | null => {
    if (!name) return null;
    const cropType = cropTypes.find(ct => ct.name.toLowerCase() === name.toLowerCase());
    return cropType ? cropType.id : null;
  };

  // Yeni toprak türü ekleme fonksiyonu
  const handleAddSoilType = async () => {
    if (!newSoilType.name || !newSoilType.description) {
      showMessage('Lütfen tüm alanları doldurun', 'error');
      return;
    }

    try {
      setLoading(true);
      const response = await axios.post(API_ENDPOINTS.SOIL_TYPE, newSoilType, {
        headers: getAuthHeaders()
      });
      
      if (response.data) {
        showMessage('Yeni toprak türü başarıyla eklendi', 'success');
        setSoilTypes([...soilTypes, response.data]);
        setOpenAddSoilTypeDialog(false);
        setNewSoilType({ name: '', description: '' });
      }
    } catch (error: any) {
      console.error('Error adding soil type:', error);
      const errorMessage = error.response?.data?.message || 'Toprak türü eklenirken bir hata oluştu';
      showMessage(errorMessage, 'error');
    } finally {
      setLoading(false);
    }
  };

  // Toprak türü dialog'unu açma fonksiyonu
  const handleOpenAddSoilTypeDialog = () => {
    setOpenAddSoilTypeDialog(true);
  };

  // Toprak türü dialog'unu kapatma fonksiyonu
  const handleCloseAddSoilTypeDialog = () => {
    setOpenAddSoilTypeDialog(false);
    setNewSoilType({ name: '', description: '' });
  };

  // Harita modalını açma fonksiyonu
  const handleOpenMapModal = (field: Field) => {
    if (field.latitude && field.longitude) {
      setMapField(field);
      setOpenMapModal(true);
    } else {
      showMessage('Bu parsel için GPS koordinatları tanımlanmamış', 'error');
    }
  };

  // Harita modalını kapatma fonksiyonu
  const handleCloseMapModal = () => {
    setOpenMapModal(false);
    setMapField(null);
  };

  // Görünüm modunu değiştirme fonksiyonu
  const handleViewModeChange = (event: React.SyntheticEvent, newValue: 'table' | 'map') => {
    setViewMode(newValue);
  };

  // Dialog'u açma fonksiyonu
  const handleOpenDialog = (field: Field | null = null) => {
    if (field) {
      setSelectedField(field);
      // Tarihleri ISO formatına dönüştür ve toprak/ekin türlerini doğru şekilde ayarla
      const formattedField = {
        ...field,
        // Tarih alanlarını doğru formata dönüştür
        plantingDate: field.plantingDate ? new Date(field.plantingDate).toISOString().split('T')[0] : '',
        harvestDate: field.harvestDate ? new Date(field.harvestDate).toISOString().split('T')[0] : '',
        // Toprak türü ID'sini doğru şekilde ayarla
        soilTypeId: field.soilTypeId || 0,
        soilType: getSoilTypeNameById(field.soilTypeId),
        // Ekin türü ID'sini doğru şekilde ayarla
        cropTypeId: field.cropTypeId || 0,
        cropType: field.cropTypeId ? getCropTypeNameById(field.cropTypeId) : ''
      };
      console.log('Editing field:', field);
      console.log('Formatted field for form:', formattedField);
      setFormData(formattedField);
    } else {
      setSelectedField(null);
      setFormData({
        id: 0,
        name: '',
        area: 0,
        location: '',
        latitude: 0,
        longitude: 0,
        soilTypeId: 0,
        soilType: '',
        isActive: true,
        cropType: '',
        cropTypeId: 0,
        plantingDate: '',
        harvestDate: '',
        notes: ''
      });
    }
    setOpenDialog(true);
  };

  // Dialog'u kapatma fonksiyonu
  const handleCloseDialog = () => {
    setOpenDialog(false);
    setSelectedField(null);
  };

  // Form verilerini güncelleme fonksiyonu
  const handleFormChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value === '' ? (name === 'latitude' || name === 'longitude' ? 0 : '') : value
    }));
  };

  // Select değişikliklerini işleme fonksiyonu
  const handleSelectChange = (e: any) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  // Switch değişikliklerini işleme fonksiyonu
  const handleSwitchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: checked
    }));
  };

  // Parsel kaydetme fonksiyonu
  const handleSaveField = async () => {
    try {
      setLoading(true);
      
      // Form doğrulama
      if (!formData.name || !formData.location || !formData.soilTypeId || formData.area <= 0) {
        showMessage('Lütfen tüm zorunlu alanları doldurun', 'error');
        setLoading(false);
        return;
      }

      // API için veri hazırlama
      const apiData = {
        ...formData,
        soilTypeId: formData.soilType ? getSoilTypeIdByName(formData.soilType) : Number(formData.soilTypeId),
        cropTypeId: formData.cropType ? getCropTypeIdByName(formData.cropType) : null,
        area: Number(formData.area),
        latitude: formData.latitude !== null && formData.latitude !== undefined ? Number(formData.latitude) : null,
        longitude: formData.longitude !== null && formData.longitude !== undefined ? Number(formData.longitude) : null
      };
      
      console.log('Sending data to API:', apiData);
      console.log('SoilType:', formData.soilType);
      console.log('SoilTypeId calculated:', formData.soilType ? getSoilTypeIdByName(formData.soilType) : Number(formData.soilTypeId));
      console.log('CropType:', formData.cropType);
      console.log('CropTypeId calculated:', getCropTypeIdByName(formData.cropType));
      console.log('Available soil types:', soilTypes);
      console.log('Available crop types:', cropTypes);
      
      // API çağrısını deneyelim
      const url = selectedField 
        ? `${API_ENDPOINTS.FIELD}/${formData.id}` 
        : API_ENDPOINTS.FIELD;
      
      const method = selectedField ? 'put' : 'post';
      
      try {
        const response = await axios[method](url, apiData, {
          headers: getAuthHeaders()
        });
        
        if (response.data) {
          showMessage(
            selectedField 
              ? 'Parsel başarıyla güncellendi' 
              : 'Yeni parsel başarıyla oluşturuldu', 
            'success'
          );
          
          // Parselleri yeniden yükle
          fetchFields();
          
          // Dialog'u kapat
          handleCloseDialog();
        }
      } catch (error: any) {
        console.error('Error saving field:', error);
        console.error('Error response:', error.response?.data);
        const errorMessage = error.response?.data?.message || 'Parsel kaydedilemedi';
        showMessage(errorMessage, 'error');
        
        // Hata durumunda manuel olarak güncelle (gerçek API olmadığı için)
        if (selectedField) {
          // Mevcut parseli güncelle
          setFields(prevFields => 
            prevFields.map(field => 
              field.id === formData.id ? formData : field
            )
          );
        } else {
          // Yeni parsel ekle
          const newField = {
            ...formData,
            id: Math.max(0, ...fields.map(f => f.id)) + 1
          };
          setFields(prevFields => [...prevFields, newField]);
        }
        
        // Dialog'u kapat
        handleCloseDialog();
      }
    } finally {
      setLoading(false);
    }
  };

  // Parsel silme fonksiyonu
  const handleDeleteField = async (id: number) => {
    if (!window.confirm('Bu parseli silmek istediğinizden emin misiniz?')) {
      return;
    }
    
    try {
      setLoading(true);
      
      // API çağrısını deneyelim
      const response = await axios.delete(`${API_ENDPOINTS.FIELD}/${id}`, {
        headers: getAuthHeaders()
      });
      
      if (response.data) {
        showMessage('Parsel başarıyla silindi', 'success');
        
        // Parselleri yeniden yükle
        fetchFields();
      }
    } catch (error: any) {
      console.error('Error deleting field:', error);
      const errorMessage = error.response?.data?.message || 'Parsel silinemedi';
      showMessage(errorMessage, 'error');
      
      // Hata durumunda manuel olarak güncelle (gerçek API olmadığı için)
      setFields(prevFields => prevFields.filter(field => field.id !== id));
    } finally {
      setLoading(false);
    }
  };

  // Leaflet için varsayılan ikon sorunu çözümü
  useEffect(() => {
    // Leaflet ikonlarını ayarla
    const defaultIcon = L.icon({
      iconRetinaUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon-2x.png',
      iconUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon.png',
      shadowUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-shadow.png',
      iconSize: [25, 41],
      iconAnchor: [12, 41],
      popupAnchor: [1, -34],
      shadowSize: [41, 41]
    });
    
    L.Marker.prototype.options.icon = defaultIcon;
  }, []);

  return (
    <Box sx={{ display: 'flex' }}>
      <Sidebar />
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <AppBar position="static" color="default" elevation={0} sx={{ borderBottom: '1px solid rgba(0, 0, 0, 0.12)' }}>
          <Toolbar>
            <Typography variant="h6" color="inherit" sx={{ flexGrow: 1 }}>
              <Agriculture sx={{ mr: 1, verticalAlign: 'middle' }} />
              Parsel / Alan Tanımlamaları
            </Typography>
            {isSuperAdmin() && (
              <Button 
                variant="contained" 
                color="primary" 
                startIcon={<Add />}
                onClick={() => handleOpenDialog()}
              >
                Yeni Parsel Ekle
              </Button>
            )}
          </Toolbar>
        </AppBar>

        {loading && (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
            <CircularProgress />
          </Box>
        )}

        {!loading && (
          <Paper sx={{ p: 2, mb: 3 }}>
            <Tabs
              value={viewMode}
              onChange={handleViewModeChange}
              indicatorColor="primary"
              textColor="primary"
              sx={{ mb: 2 }}
            >
              <Tab 
                value="table" 
                label="Tablo Görünümü" 
                icon={<TerrainOutlined />} 
                iconPosition="start" 
              />
              <Tab 
                value="map" 
                label="Harita Görünümü" 
                icon={<Map />} 
                iconPosition="start" 
              />
            </Tabs>
            
            {viewMode === 'table' ? (
              <TableContainer>
                <Table>
                  <TableHead>
                    <TableRow sx={{ backgroundColor: 'rgba(0, 0, 0, 0.04)' }}>
                      <TableCell>Parsel Adı</TableCell>
                      <TableCell>Alan (Dönüm)</TableCell>
                      <TableCell>Konum</TableCell>
                      <TableCell>GPS Koordinatları</TableCell>
                      <TableCell>Toprak Türü</TableCell>
                      <TableCell>Ekin Türü</TableCell>
                      <TableCell align="center">Durum</TableCell>
                      <TableCell align="right">İşlemler</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {fields.map((field) => (
                      <TableRow key={field.id} sx={{ '&:hover': { backgroundColor: 'rgba(0, 0, 0, 0.04)' } }}>
                        <TableCell>{field.name}</TableCell>
                        <TableCell>{typeof field.area === 'number' ? field.area.toFixed(1) : Number(field.area).toFixed(1)}</TableCell>
                        <TableCell>{field.location}</TableCell>
                        <TableCell>
                          {field.latitude && field.longitude 
                            ? `${Number(field.latitude).toFixed(6)}, ${Number(field.longitude).toFixed(6)}` 
                            : 'Belirtilmemiş'}
                        </TableCell>
                        <TableCell>{field.soilType}</TableCell>
                        <TableCell>{field.cropType || '-'}</TableCell>
                        <TableCell align="center">
                          <Chip 
                            size="small" 
                            label={field.isActive ? 'Aktif' : 'Pasif'} 
                            color={field.isActive ? 'success' : 'default'}
                          />
                        </TableCell>
                        <TableCell align="right">
                          <IconButton 
                            size="small" 
                            color="primary" 
                            onClick={() => handleOpenDialog(field)}
                            title="Düzenle"
                          >
                            <Edit fontSize="small" />
                          </IconButton>
                          <IconButton 
                            size="small" 
                            color="error" 
                            onClick={() => handleDeleteField(field.id)}
                            title="Sil"
                          >
                            <Delete fontSize="small" />
                          </IconButton>
                          <IconButton 
                            size="small" 
                            color="secondary" 
                            onClick={() => handleOpenMapModal(field)}
                            title="Haritada Göster"
                          >
                            <Map fontSize="small" />
                          </IconButton>
                        </TableCell>
                      </TableRow>
                    ))}
                    {fields.length === 0 && (
                      <TableRow>
                        <TableCell colSpan={8} align="center" sx={{ py: 3 }}>
                          <Typography variant="body1" color="textSecondary">
                            Henüz parsel tanımlanmamış
                          </Typography>
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            ) : (
              <Box sx={{ height: 500, width: '100%', mt: 2 }}>
                {fields.some(field => field.latitude && field.longitude) ? (
                  <MapContainer 
                    center={[39.9334, 32.8597]} // Ankara merkezi
                    zoom={8} 
                    style={{ height: '100%', width: '100%' }}
                  >
                    <TileLayer
                      attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                      url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    />
                    {fields.map(field => 
                      field.latitude && field.longitude ? (
                        <Marker 
                          key={field.id} 
                          position={[Number(field.latitude), Number(field.longitude)]}
                        >
                          <Popup>
                            <Typography variant="subtitle1">{field.name}</Typography>
                            <Typography variant="body2">Alan: {field.area} dönüm</Typography>
                            <Typography variant="body2">Konum: {field.location}</Typography>
                            <Typography variant="body2">Toprak: {field.soilType}</Typography>
                            {field.cropType && (
                              <Typography variant="body2">Ekin: {field.cropType}</Typography>
                            )}
                            {isSuperAdmin() && (
                              <Box sx={{ mt: 1 }}>
                                <Button 
                                  size="small" 
                                  variant="outlined" 
                                  color="primary" 
                                  onClick={() => handleOpenDialog(field)}
                                  sx={{ mr: 1 }}
                                >
                                  Düzenle
                                </Button>
                                <Button 
                                  size="small" 
                                  variant="outlined" 
                                  color="error" 
                                  onClick={() => handleDeleteField(field.id)}
                                >
                                  Sil
                                </Button>
                              </Box>
                            )}
                          </Popup>
                        </Marker>
                      ) : null
                    )}
                  </MapContainer>
                ) : (
                  <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
                    <Typography variant="body1" color="textSecondary">
                      <LocationOn sx={{ mr: 1, verticalAlign: 'middle' }} />
                      GPS koordinatları olan parsel bulunmamaktadır
                    </Typography>
                  </Box>
                )}
              </Box>
            )}
          </Paper>
        )}

        {/* Parsel Ekleme/Düzenleme Dialog'u */}
        <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
          <DialogTitle>
            {selectedField ? 'Parsel Düzenle' : 'Yeni Parsel Ekle'}
          </DialogTitle>
          <DialogContent dividers>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Parsel Adı"
                  name="name"
                  value={formData.name}
                  onChange={handleFormChange}
                  margin="normal"
                  required
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Alan (Dönüm)"
                  name="area"
                  type="number"
                  value={formData.area}
                  onChange={handleFormChange}
                  margin="normal"
                  required
                  inputProps={{ min: 0, step: 0.1 }}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Konum"
                  name="location"
                  value={formData.location}
                  onChange={handleFormChange}
                  margin="normal"
                  required
                  placeholder="İl, İlçe, Köy veya koordinat"
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Enlem (Latitude)"
                  name="latitude"
                  type="number"
                  value={formData.latitude === 0 && !selectedField ? '' : formData.latitude}
                  onChange={handleFormChange}
                  margin="normal"
                  inputProps={{ step: 0.0001 }}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Boylam (Longitude)"
                  name="longitude"
                  type="number"
                  value={formData.longitude === 0 && !selectedField ? '' : formData.longitude}
                  onChange={handleFormChange}
                  margin="normal"
                  inputProps={{ step: 0.0001 }}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <FormControl fullWidth margin="normal" required>
                  <Autocomplete
                    id="soilType"
                    options={soilTypes}
                    getOptionLabel={(option) => option.name}
                    value={soilTypes.find(st => st.id === Number(formData.soilTypeId)) || null}
                    onChange={(event, newValue) => {
                      if (newValue) {
                        setFormData({
                          ...formData,
                          soilTypeId: newValue.id,
                          soilType: newValue.name
                        });
                      }
                    }}
                    renderInput={(params) => (
                      <TextField 
                        {...params} 
                        label="Toprak Türü" 
                        margin="normal"
                        required
                      />
                    )}
                    noOptionsText={
                      <Box sx={{ textAlign: 'center', py: 1 }}>
                        <Typography variant="body2" color="textSecondary" gutterBottom>
                          Aradığınız toprak türü bulunamadı
                        </Typography>
                        {isSuperAdmin() && (
                          <Button 
                            size="small" 
                            variant="outlined" 
                            onClick={handleOpenAddSoilTypeDialog}
                            startIcon={<Add />}
                          >
                            Yeni Toprak Türü Ekle
                          </Button>
                        )}
                      </Box>
                    }
                  />
                </FormControl>
              </Grid>
              <Grid item xs={12} sm={6}>
                <FormControl fullWidth margin="normal">
                  <InputLabel>Ekin Türü</InputLabel>
                  <Select
                    name="cropType"
                    value={formData.cropType || ''}
                    onChange={handleSelectChange}
                    label="Ekin Türü"
                  >
                    <MenuItem value="">Seçiniz</MenuItem>
                    {cropTypes.map(type => (
                      <MenuItem key={type.id} value={type.name}>{type.name}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Ekim Tarihi"
                  name="plantingDate"
                  type="date"
                  value={formData.plantingDate || ''}
                  onChange={handleFormChange}
                  margin="normal"
                  InputLabelProps={{ shrink: true }}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Hasat Tarihi"
                  name="harvestDate"
                  type="date"
                  value={formData.harvestDate || ''}
                  onChange={handleFormChange}
                  margin="normal"
                  InputLabelProps={{ shrink: true }}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Notlar"
                  name="notes"
                  value={formData.notes || ''}
                  onChange={handleFormChange}
                  margin="normal"
                  multiline
                  rows={3}
                />
              </Grid>
              <Grid item xs={12}>
                <FormControlLabel
                  control={
                    <Switch
                      checked={formData.isActive}
                      onChange={handleSwitchChange}
                      name="isActive"
                      color="primary"
                    />
                  }
                  label="Aktif"
                />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDialog} color="inherit">
              İptal
            </Button>
            <Button 
              onClick={handleSaveField} 
              color="primary" 
              variant="contained"
              disabled={loading}
              startIcon={loading ? <CircularProgress size={20} /> : null}
            >
              {selectedField ? 'Güncelle' : 'Kaydet'}
            </Button>
          </DialogActions>
        </Dialog>

        {/* Harita Modalı */}
        <Dialog open={openMapModal} onClose={handleCloseMapModal} maxWidth="lg" fullWidth>
          <DialogTitle>
            {mapField?.name} Parsel Haritası
          </DialogTitle>
          <DialogContent dividers>
            <Box sx={{ height: 500, width: '100%' }}>
              {mapField?.latitude && mapField?.longitude ? (
                <MapContainer 
                  center={[Number(mapField.latitude), Number(mapField.longitude)]} 
                  zoom={15} 
                  style={{ height: '100%', width: '100%' }}
                >
                  <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                  />
                  <Marker 
                    position={[Number(mapField.latitude), Number(mapField.longitude)]}
                  >
                    <Popup>
                      <Typography variant="subtitle1">{mapField.name}</Typography>
                      <Typography variant="body2">Alan: {mapField.area} dönüm</Typography>
                      <Typography variant="body2">Konum: {mapField.location}</Typography>
                      <Typography variant="body2">Toprak: {mapField.soilType}</Typography>
                      {mapField.cropType && (
                        <Typography variant="body2">Ekin: {mapField.cropType}</Typography>
                      )}
                    </Popup>
                  </Marker>
                </MapContainer>
              ) : (
                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
                  <Typography variant="body1" color="textSecondary">
                    <LocationOn sx={{ mr: 1, verticalAlign: 'middle' }} />
                    GPS koordinatları bulunmamaktadır
                  </Typography>
                </Box>
              )}
            </Box>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseMapModal} color="inherit">
              Kapat
            </Button>
          </DialogActions>
        </Dialog>

        {/* Yeni Toprak Türü Ekleme Dialog'u */}
        <Dialog open={openAddSoilTypeDialog} onClose={handleCloseAddSoilTypeDialog}>
          <DialogTitle>Yeni Toprak Türü Ekle</DialogTitle>
          <DialogContent>
            <DialogContentText>
              Veritabanına yeni bir toprak türü eklemek için aşağıdaki bilgileri doldurun.
            </DialogContentText>
            <TextField
              autoFocus
              margin="dense"
              label="Toprak Türü Adı"
              type="text"
              fullWidth
              variant="outlined"
              value={newSoilType.name}
              onChange={(e) => setNewSoilType({ ...newSoilType, name: e.target.value })}
              required
            />
            <TextField
              margin="dense"
              label="Açıklama"
              type="text"
              fullWidth
              variant="outlined"
              value={newSoilType.description}
              onChange={(e) => setNewSoilType({ ...newSoilType, description: e.target.value })}
              required
              multiline
              rows={3}
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseAddSoilTypeDialog} color="inherit">
              İptal
            </Button>
            <Button onClick={handleAddSoilType} color="primary" variant="contained" disabled={loading}>
              {loading ? <CircularProgress size={24} /> : 'Ekle'}
            </Button>
          </DialogActions>
        </Dialog>

        {/* Bildirim Snackbar'ı */}
        <Snackbar 
          open={openSnackbar} 
          autoHideDuration={6000} 
          onClose={() => setOpenSnackbar(false)}
          anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        >
          <Alert 
            onClose={() => setOpenSnackbar(false)} 
            severity={severity} 
            sx={{ width: '100%' }}
          >
            {message}
          </Alert>
        </Snackbar>
      </Box>
    </Box>
  );
};

export default FieldManagement;
