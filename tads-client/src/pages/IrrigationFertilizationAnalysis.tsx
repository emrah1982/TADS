import React, { useState, useEffect } from 'react';
import { 
  Container, 
  Box, 
  Typography, 
  Paper, 
  Grid, 
  FormControl, 
  InputLabel, 
  Select, 
  MenuItem, 
  Card, 
  CardContent, 
  Divider,
  CircularProgress,
  Alert,
  Tabs,
  Tab,
  SelectChangeEvent,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  TextField,
  IconButton,
  Snackbar,
  FormHelperText,
  TableContainer,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody
} from '@mui/material';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { 
  WaterDrop as WaterIcon, 
  Grass as FertilizerIcon,
  Agriculture as YieldIcon,
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Close as CloseIcon
} from '@mui/icons-material';
import Sidebar from '../components/Sidebar';
import { useAuth } from '../contexts/AuthContext';
import axios from 'axios';
import API_CONFIG from '../config/api.config';

// API endpointleri
const API_ENDPOINTS = {
  FIELDS: API_CONFIG.ENDPOINTS.FIELD_MANAGEMENT.FIELDS,
  IRRIGATION: API_CONFIG.ENDPOINTS.IRRIGATION_FERTILIZATION.IRRIGATION,
  FERTILIZATION: API_CONFIG.ENDPOINTS.IRRIGATION_FERTILIZATION.FERTILIZATION,
  FERTILIZER_TYPES: API_CONFIG.ENDPOINTS.IRRIGATION_FERTILIZATION.FERTILIZER_TYPES,
  YIELD: API_CONFIG.ENDPOINTS.YIELD.DATA
};

// Arayüz tanımlamaları
interface Field {
  id: number;
  name: string;
  size: number;
  soilType: string;
  area: number;
}

interface Irrigation {
  id: number;
  fieldId: number;
  date: string;
  amount: number;
  unit: string;
  method: string;
  notes?: string;
  soilMoistureBeforeIrrigation?: number;
  soilMoistureAfterIrrigation?: number;
  season: number;
}

interface IrrigationFormData {
  fieldId: number;
  date: Date | null;
  amount: number;
  unit: string;
  method: string;
  notes?: string;
  soilMoistureBeforeIrrigation?: number;
  soilMoistureAfterIrrigation?: number;
  season: number;
}

interface Fertilization {
  id: number;
  fieldId: number;
  date: string;
  fertilizerTypeId: number;
  fertilizerTypeName: string;
  amount: number;
  unit: string;
  method: string;
  notes?: string;
  season: number;
}

interface FertilizationFormData {
  fieldId: number;
  date: Date | null;
  fertilizerTypeId: number;
  amount: number;
  unit: string;
  method: string;
  notes?: string;
  season: number;
}

interface FertilizerType {
  id: number;
  name: string;
  description: string;
  nutrientContent: string;
  category?: string;
  npk?: string;
}

interface Yield {
  id: number;
  fieldId: number;
  season: number;
  amount: number;
  unit: string;
}

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function a11yProps(index: number) {
  return {
    id: `tab-${index}`,
    'aria-controls': `tabpanel-${index}`,
  };
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`tabpanel-${index}`}
      aria-labelledby={`tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ p: 3 }}>
          {children}
        </Box>
      )}
    </div>
  );
}

const IrrigationFertilizationAnalysis: React.FC = () => {
  const [fields, setFields] = useState<Field[]>([]);
  const [selectedFieldId, setSelectedFieldId] = useState<number | ''>('');
  const [irrigationData, setIrrigationData] = useState<Irrigation[]>([]);
  const [fertilizationData, setFertilizationData] = useState<Fertilization[]>([]);
  const [fertilizerTypes, setFertilizerTypes] = useState<FertilizerType[]>([]);
  const [yieldData, setYieldData] = useState<Yield[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [tabValue, setTabValue] = useState(0);
  const { token } = useAuth();

  // CRUD Dialog State'leri
  const [openIrrigationDialog, setOpenIrrigationDialog] = useState(false);
  const [openFertilizationDialog, setOpenFertilizationDialog] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [currentIrrigation, setCurrentIrrigation] = useState<Irrigation | null>(null);
  const [currentFertilization, setCurrentFertilization] = useState<Fertilization | null>(null);
  const [deleteType, setDeleteType] = useState<'irrigation' | 'fertilization'>('irrigation');
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  const [formErrors, setFormErrors] = useState<{[key: string]: string}>({});

  // Form State'leri
  const [irrigationForm, setIrrigationForm] = useState<IrrigationFormData>({
    fieldId: selectedFieldId as number,
    date: new Date(),
    amount: 0,
    unit: 'Litre',
    method: 'Damla',
    notes: '',
    soilMoistureBeforeIrrigation: undefined,
    soilMoistureAfterIrrigation: undefined,
    season: new Date().getFullYear()
  });

  const [fertilizationForm, setFertilizationForm] = useState<FertilizationFormData>({
    fieldId: selectedFieldId as number,
    date: new Date(),
    fertilizerTypeId: 0,
    amount: 0,
    unit: 'kg/da',
    method: 'Serpme',
    notes: '',
    season: new Date().getFullYear()
  });

  // Parselleri yükle
  useEffect(() => {
    const fetchFields = async () => {
      try {
        setLoading(true);
        const response = await axios.get(API_ENDPOINTS.FIELDS, {
          headers: { Authorization: `Bearer ${token}` }
        });
        const fieldData = Array.isArray(response.data) ? response.data : 
                         response.data.value ? response.data.value : 
                         response.data.data ? response.data.data : [];
        setFields(fieldData);
      } catch (err) {
        console.error('Parseller yüklenirken hata oluştu:', err);
        setError('Parseller yüklenirken bir hata oluştu.');
        setSnackbarMessage('Parseller yüklenirken bir hata oluştu.');
        setSnackbarOpen(true);
      } finally {
        setLoading(false);
      }
    };

    fetchFields();
  }, [token]);

  // Gübre tiplerini yükle (sadece bir kez)
  useEffect(() => {
    let isMounted = true;
    
    const fetchFertilizerTypes = async () => {
      try {
        const response = await axios.get(API_ENDPOINTS.FERTILIZER_TYPES, {
          headers: { Authorization: `Bearer ${token}` }
        });
        
        if (isMounted) {
          if (response.status === 200 && response.data) {
            setFertilizerTypes(response.data);
            
            // Varsayılan gübre tipi ID'sini ayarla
            if (response.data.length > 0 && !fertilizationForm.fertilizerTypeId) {
              setFertilizationForm(prev => ({
                ...prev,
                fertilizerTypeId: response.data[0].id
              }));
            }
          } else {
            console.error('Gübre tipleri yüklenirken hata oluştu:', response);
            setSnackbarMessage('Gübre tipleri yüklenirken bir hata oluştu.');
            setSnackbarOpen(true);
          }
        }
      } catch (err) {
        console.error('Gübre tipleri yüklenirken hata oluştu:', err);
        setSnackbarMessage('Gübre tipleri yüklenirken bir hata oluştu.');
        setSnackbarOpen(true);
      }
    };

    fetchFertilizerTypes();
    
    return () => {
      isMounted = false;
    };
  }, [token]);

  // Parsel değiştiğinde verileri yükle
  useEffect(() => {
    if (selectedFieldId !== '') {
      fetchIrrigationData();
      fetchFertilizationData();
      fetchYieldData();
      
      // Formları güncelle
      setIrrigationForm(prev => ({ ...prev, fieldId: selectedFieldId as number }));
      setFertilizationForm(prev => ({ ...prev, fieldId: selectedFieldId as number }));
    }
  }, [selectedFieldId]);

  const fetchIrrigationData = async () => {
    try {
      setLoading(true);
      const response = await axios.get(`${API_ENDPOINTS.IRRIGATION}/field/${selectedFieldId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        timeout: 10000 // 10 saniye timeout
      });

      if (response.status === 200 && response.data) {
        setIrrigationData(response.data);
      } else {
        throw new Error('Geçersiz veri yapısı');
      }
    } catch (err) {
      console.error('Sulama verileri yüklenirken hata oluştu:', err);
      
      let errorMessage = 'Sulama verileri yüklenirken bir hata oluştu.';
      
      if (axios.isAxiosError(err)) {
        if (err.response) {
          // Sunucudan gelen özel hata mesajını kullan
          errorMessage = err.response.data?.message || errorMessage;
        } else if (err.request) {
          errorMessage = 'Sunucuya bağlanılamadı';
        }
      }

      setError(errorMessage);
      setSnackbarMessage(errorMessage);
      setSnackbarOpen(true);
      setIrrigationData([]);
    } finally {
      setLoading(false);
    }
  };

  const fetchFertilizationData = async () => {
    try {
      setLoading(true);
      const response = await axios.get(`${API_ENDPOINTS.FERTILIZATION}/field/${selectedFieldId}`, {
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      setFertilizationData(response.data);
    } catch (err) {
      console.error('Gübreleme verileri yüklenirken hata oluştu:', err);
      setError('Gübreleme verileri yüklenirken bir hata oluştu.');
      setSnackbarMessage('Gübreleme verileri yüklenirken bir hata oluştu.');
      setSnackbarOpen(true);
      setFertilizationData([]);
    } finally {
      setLoading(false);
    }
  };

  const fetchYieldData = async () => {
    try {
      setLoading(true);
      const response = await axios.get(`${API_ENDPOINTS.YIELD}/field/${selectedFieldId}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      setYieldData(response.data);
    } catch (err) {
      console.error('Verim verileri yüklenirken hata oluştu:', err);
      setError('Verim verileri yüklenirken bir hata oluştu.');
      setSnackbarMessage('Verim verileri yüklenirken bir hata oluştu.');
      setSnackbarOpen(true);
      setYieldData([]);
    } finally {
      setLoading(false);
    }
  };

  const handleFieldChange = (event: SelectChangeEvent<number | string>) => {
    setSelectedFieldId(event.target.value as number);
  };

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  // Form Handlers
  const handleIrrigationFormChange = (field: keyof IrrigationFormData, value: any) => {
    setIrrigationForm(prev => ({ ...prev, [field]: value }));
    if (formErrors[field]) {
      setFormErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const handleFertilizationFormChange = (field: keyof FertilizationFormData, value: any) => {
    setFertilizationForm(prev => ({ ...prev, [field]: value }));
    if (formErrors[field]) {
      setFormErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  // Dialog Handlers
  const handleOpenIrrigationDialog = (irrigation?: Irrigation) => {
    if (irrigation) {
      setCurrentIrrigation(irrigation);
      setIrrigationForm({
        fieldId: irrigation.fieldId,
        date: new Date(irrigation.date),
        amount: irrigation.amount,
        unit: irrigation.unit,
        method: irrigation.method,
        notes: irrigation.notes || '',
        soilMoistureBeforeIrrigation: irrigation.soilMoistureBeforeIrrigation,
        soilMoistureAfterIrrigation: irrigation.soilMoistureAfterIrrigation,
        season: irrigation.season
      });
    } else {
      setCurrentIrrigation(null);
      setIrrigationForm({
        fieldId: selectedFieldId as number,
        date: new Date(),
        amount: 0,
        unit: 'Litre',
        method: 'Damla',
        notes: '',
        soilMoistureBeforeIrrigation: undefined,
        soilMoistureAfterIrrigation: undefined,
        season: new Date().getFullYear()
      });
    }
    setOpenIrrigationDialog(true);
  };

  const handleOpenFertilizationDialog = (fertilization?: Fertilization) => {
    if (fertilization) {
      setCurrentFertilization(fertilization);
      setFertilizationForm({
        fieldId: fertilization.fieldId,
        date: new Date(fertilization.date),
        fertilizerTypeId: fertilization.fertilizerTypeId,
        amount: fertilization.amount,
        unit: fertilization.unit,
        method: fertilization.method,
        notes: fertilization.notes || '',
        season: fertilization.season
      });
    } else {
      setCurrentFertilization(null);
      setFertilizationForm({
        fieldId: selectedFieldId as number,
        date: new Date(),
        fertilizerTypeId: fertilizerTypes.length > 0 ? fertilizerTypes[0].id : 0,
        amount: 0,
        unit: 'kg/da',
        method: 'Serpme',
        notes: '',
        season: new Date().getFullYear()
      });
    }
    setOpenFertilizationDialog(true);
  };

  const handleCloseIrrigationDialog = () => {
    setOpenIrrigationDialog(false);
    setFormErrors({});
  };

  const handleCloseFertilizationDialog = () => {
    setOpenFertilizationDialog(false);
    setFormErrors({});
  };

  const handleOpenDeleteDialog = (type: 'irrigation' | 'fertilization', id: number) => {
    setDeleteType(type);
    setDeleteId(id);
    setOpenDeleteDialog(true);
  };

  const handleCloseDeleteDialog = () => {
    setOpenDeleteDialog(false);
    setDeleteId(null);
  };

  const handleCloseSnackbar = () => {
    setSnackbarOpen(false);
  };

  // Validation
  const validateIrrigationForm = (): boolean => {
    const errors: {[key: string]: string} = {};
    
    if (!irrigationForm.date || isNaN(irrigationForm.date.getTime())) {
      errors.date = 'Geçerli bir tarih giriniz';
    }
    
    if (!irrigationForm.fieldId || irrigationForm.fieldId <= 0) {
      errors.fieldId = 'Parsel seçilmelidir';
    }
    
    if (isNaN(Number(irrigationForm.amount)) || Number(irrigationForm.amount) <= 0) {
      errors.amount = 'Miktar pozitif bir sayısal değer olmalıdır';
    }
    
    if (!irrigationForm.unit) {
      errors.unit = 'Birim gereklidir';
    }
    
    if (!irrigationForm.method) {
      errors.method = 'Yöntem gereklidir';
    }
    
    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const validateFertilizationForm = (): boolean => {
    const errors: {[key: string]: string} = {};
    
    if (!fertilizationForm.date || isNaN(fertilizationForm.date.getTime())) {
      errors.date = 'Geçerli bir tarih giriniz';
    }
    
    if (!fertilizationForm.fieldId || fertilizationForm.fieldId <= 0) {
      errors.fieldId = 'Parsel seçilmelidir';
    }
    
    if (fertilizationForm.fertilizerTypeId <= 0) {
      errors.fertilizerTypeId = 'Gübre türü seçilmelidir';
    }
    
    if (isNaN(Number(fertilizationForm.amount)) || Number(fertilizationForm.amount) <= 0) {
      errors.amount = 'Miktar pozitif bir sayısal değer olmalıdır';
    }
    
    if (!fertilizationForm.unit) {
      errors.unit = 'Birim gereklidir';
    }
    
    if (!fertilizationForm.method) {
      errors.method = 'Yöntem gereklidir';
    }
    
    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  // CRUD Operations
  const handleSaveIrrigation = async () => {
    if (!validateIrrigationForm()) return;
    
    try {
      setLoading(true);
      
      const formData = {
        ...irrigationForm,
        fieldId: Number(irrigationForm.fieldId),
        amount: Number(irrigationForm.amount),
        date: irrigationForm.date?.toISOString().split('T')[0],
        season: irrigationForm.date?.getFullYear() || new Date().getFullYear(),
        soilMoistureBeforeIrrigation: irrigationForm.soilMoistureBeforeIrrigation === undefined ? null : irrigationForm.soilMoistureBeforeIrrigation,
        soilMoistureAfterIrrigation: irrigationForm.soilMoistureAfterIrrigation === undefined ? null : irrigationForm.soilMoistureAfterIrrigation,
        notes: irrigationForm.notes || ''
      };
      
      console.log('Gönderilecek sulama verisi:', formData);
      
      if (currentIrrigation) {
        await axios.put(`${API_ENDPOINTS.IRRIGATION}/${currentIrrigation.id}`, 
          { ...formData, id: currentIrrigation.id },
          { 
            headers: { 
              Authorization: `Bearer ${token}`,
              'Content-Type': 'application/json'
            } 
          }
        );
        setSnackbarMessage('Sulama kaydı başarıyla güncellendi');
      } else {
        await axios.post(API_ENDPOINTS.IRRIGATION, formData, {
          headers: { 
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        });
        setSnackbarMessage('Sulama kaydı başarıyla eklendi');
      }
      
      handleCloseIrrigationDialog();
      fetchIrrigationData();
      setSnackbarOpen(true);
    } catch (err) {
      console.error('Sulama kaydı kaydedilirken hata oluştu:', err);
      
      let errorMessage = 'Sulama kaydı kaydedilirken bir hata oluştu.';
      
      if (axios.isAxiosError(err)) {
        if (err.response) {
          // Sunucudan gelen özel hata mesajını kullan
          errorMessage = err.response.data?.message || errorMessage;
        } else if (err.request) {
          errorMessage = 'Sunucuya bağlanılamadı';
        }
      }

      setError(errorMessage);
      setSnackbarMessage(errorMessage);
      setSnackbarOpen(true);
    } finally {
      setLoading(false);
    }
  };

  const handleSaveFertilization = async () => {
    if (!validateFertilizationForm()) return;
    
    try {
      setLoading(true);
      
      const formData = {
        ...fertilizationForm,
        amount: Number(fertilizationForm.amount),
        date: fertilizationForm.date?.toISOString().split('T')[0],
        season: fertilizationForm.date?.getFullYear() || new Date().getFullYear(),
        notes: fertilizationForm.notes || '',
        fertilizerTypeId: fertilizationForm.fertilizerTypeId || 1
      };
      
      console.log('Gönderilecek gübreleme verisi:', formData);
      
      if (currentFertilization) {
        await axios.put(`${API_ENDPOINTS.FERTILIZATION}/${currentFertilization.id}`, 
          { ...formData, id: currentFertilization.id },
          { 
            headers: { 
              Authorization: `Bearer ${token}`,
              'Content-Type': 'application/json'
            } 
          }
        );
        setSnackbarMessage('Gübreleme kaydı başarıyla güncellendi');
      } else {
        await axios.post(API_ENDPOINTS.FERTILIZATION, formData, {
          headers: { 
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        });
        setSnackbarMessage('Gübreleme kaydı başarıyla eklendi');
      }
      
      handleCloseFertilizationDialog();
      fetchFertilizationData();
      setSnackbarOpen(true);
    } catch (err) {
      console.error('Gübreleme kaydı kaydedilirken hata oluştu:', err);
      
      let errorMessage = 'Gübreleme kaydı kaydedilirken bir hata oluştu.';
      
      if (axios.isAxiosError(err)) {
        if (err.response) {
          // Sunucudan gelen özel hata mesajını kullan
          errorMessage = err.response.data?.message || errorMessage;
        } else if (err.request) {
          errorMessage = 'Sunucuya bağlanılamadı';
        }
      }
      
      setError(errorMessage);
      setSnackbarMessage(errorMessage);
      setSnackbarOpen(true);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteId) return;
    
    try {
      setLoading(true);
      
      if (deleteType === 'irrigation') {
        await axios.delete(`${API_ENDPOINTS.IRRIGATION}/${deleteId}`, 
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSnackbarMessage('Sulama kaydı başarıyla silindi');
        fetchIrrigationData();
      } else {
        await axios.delete(`${API_ENDPOINTS.FERTILIZATION}/${deleteId}`, 
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSnackbarMessage('Gübreleme kaydı başarıyla silindi');
        fetchFertilizationData();
      }
      
      handleCloseDeleteDialog();
      setSnackbarOpen(true);
    } catch (err) {
      console.error('Kayıt silinirken hata oluştu:', err);
      setError('Kayıt silinirken bir hata oluştu.');
      setSnackbarMessage('Kayıt silinirken bir hata oluştu.');
      setSnackbarOpen(true);
    } finally {
      setLoading(false);
    }
  };

  // Hesaplama fonksiyonları
  const calculateTotalIrrigation = () => {
    return irrigationData.reduce((total, item) => total + item.amount, 0);
  };

  const calculateTotalFertilization = () => {
    return fertilizationData.reduce((total, item) => total + item.amount, 0);
  };

  const calculateAverageYield = () => {
    if (yieldData.length === 0) return 0;
    return yieldData.reduce((total, item) => total + item.amount, 0) / yieldData.length;
  };

  const calculateIrrigationEfficiency = () => {
    const totalIrrigation = calculateTotalIrrigation();
    const avgYield = calculateAverageYield();
    return totalIrrigation === 0 || avgYield === 0 ? 0 : avgYield / totalIrrigation;
  };

  const calculateFertilizationEfficiency = () => {
    const totalFertilization = calculateTotalFertilization();
    const avgYield = calculateAverageYield();
    return totalFertilization === 0 || avgYield === 0 ? 0 : avgYield / totalFertilization;
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <Sidebar />
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Container>
          <Box sx={{ my: 4 }}>
            <Typography variant="h4" component="h1" gutterBottom>
              Sulama ve Gübreleme Analizi
            </Typography>
            
            <FormControl fullWidth sx={{ mb: 3 }}>
              <InputLabel id="field-select-label">Parsel Seçin</InputLabel>
              <Select
                labelId="field-select-label"
                id="field-select"
                value={selectedFieldId}
                label="Parsel Seçin"
                onChange={handleFieldChange}
                disabled={loading}
              >
                {fields.map((field) => (
                  <MenuItem key={`field-${field.id}`} value={field.id}>
                    {field.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            {selectedFieldId && (
              <>
                <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 2 }}>
                  <Tabs value={tabValue} onChange={handleTabChange} aria-label="analysis tabs">
                    <Tab label="Sulama" {...a11yProps(0)} />
                    <Tab label="Gübreleme" {...a11yProps(1)} />
                    <Tab label="Verim Analizi" {...a11yProps(2)} />
                  </Tabs>
                </Box>

                <TabPanel value={tabValue} index={0}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                    <Typography variant="h6">Sulama Kayıtları</Typography>
                    <Button 
                      variant="contained" 
                      startIcon={<AddIcon />}
                      onClick={() => handleOpenIrrigationDialog()}
                      disabled={loading}
                    >
                      Yeni Sulama Ekle
                    </Button>
                  </Box>
                  
                  {loading ? (
                    <CircularProgress />
                  ) : irrigationData.length > 0 ? (
                    <TableContainer component={Paper}>
                      <Table>
                        <TableHead>
                          <TableRow>
                            <TableCell>Tarih</TableCell>
                            <TableCell>Miktar</TableCell>
                            <TableCell>Birim</TableCell>
                            <TableCell>Yöntem</TableCell>
                            <TableCell>Toprak Nemi (Öncesi)</TableCell>
                            <TableCell>Toprak Nemi (Sonrası)</TableCell>
                            <TableCell>Notlar</TableCell>
                            <TableCell>İşlemler</TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {irrigationData.map((irrigation) => (
                            <TableRow key={`irrigation-${irrigation.id}`}>
                              <TableCell>{new Date(irrigation.date).toLocaleDateString()}</TableCell>
                              <TableCell>{irrigation.amount}</TableCell>
                              <TableCell>{irrigation.unit}</TableCell>
                              <TableCell>{irrigation.method}</TableCell>
                              <TableCell>{irrigation.soilMoistureBeforeIrrigation || '-'}</TableCell>
                              <TableCell>{irrigation.soilMoistureAfterIrrigation || '-'}</TableCell>
                              <TableCell>{irrigation.notes || '-'}</TableCell>
                              <TableCell>
                                <IconButton 
                                  aria-label="edit" 
                                  onClick={() => handleOpenIrrigationDialog(irrigation)}
                                  disabled={loading}
                                >
                                  <EditIcon />
                                </IconButton>
                                <IconButton 
                                  aria-label="delete" 
                                  onClick={() => handleOpenDeleteDialog('irrigation', irrigation.id)}
                                  disabled={loading}
                                >
                                  <DeleteIcon />
                                </IconButton>
                              </TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  ) : (
                    <Alert severity="info">Bu parsel için henüz sulama kaydı bulunmamaktadır.</Alert>
                  )}
                </TabPanel>

                <TabPanel value={tabValue} index={1}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                    <Typography variant="h6">Gübreleme Kayıtları</Typography>
                    <Button 
                      variant="contained" 
                      startIcon={<AddIcon />}
                      onClick={() => handleOpenFertilizationDialog()}
                      disabled={loading}
                    >
                      Yeni Gübreleme Ekle
                    </Button>
                  </Box>
                  
                  {loading ? (
                    <CircularProgress />
                  ) : fertilizationData.length > 0 ? (
                    <TableContainer component={Paper}>
                      <Table>
                        <TableHead>
                          <TableRow>
                            <TableCell>Tarih</TableCell>
                            <TableCell>Gübre Tipi</TableCell>
                            <TableCell>Miktar</TableCell>
                            <TableCell>Birim</TableCell>
                            <TableCell>Yöntem</TableCell>
                            <TableCell>Notlar</TableCell>
                            <TableCell>İşlemler</TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {fertilizationData.map((fertilization) => (
                            <TableRow key={`fertilization-${fertilization.id}`}>
                              <TableCell>{new Date(fertilization.date).toLocaleDateString()}</TableCell>
                              <TableCell>{fertilization.fertilizerTypeName}</TableCell>
                              <TableCell>{fertilization.amount}</TableCell>
                              <TableCell>{fertilization.unit}</TableCell>
                              <TableCell>{fertilization.method}</TableCell>
                              <TableCell>{fertilization.notes || '-'}</TableCell>
                              <TableCell>
                                <IconButton 
                                  aria-label="edit" 
                                  onClick={() => handleOpenFertilizationDialog(fertilization)}
                                  disabled={loading}
                                >
                                  <EditIcon />
                                </IconButton>
                                <IconButton 
                                  aria-label="delete" 
                                  onClick={() => handleOpenDeleteDialog('fertilization', fertilization.id)}
                                  disabled={loading}
                                >
                                  <DeleteIcon />
                                </IconButton>
                              </TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  ) : (
                    <Alert severity="info">Bu parsel için henüz gübreleme kaydı bulunmamaktadır.</Alert>
                  )}
                </TabPanel>

                <TabPanel value={tabValue} index={2}>
                  <Typography variant="h6" gutterBottom>Verim Analizi</Typography>
                  
                  <Grid container spacing={3}>
                    <Grid item xs={12} md={4}>
                      <Paper sx={{ p: 2 }}>
                        <Typography variant="subtitle1">Toplam Sulama</Typography>
                        <Typography variant="h5">{calculateTotalIrrigation()} Litre</Typography>
                      </Paper>
                    </Grid>
                    <Grid item xs={12} md={4}>
                      <Paper sx={{ p: 2 }}>
                        <Typography variant="subtitle1">Toplam Gübreleme</Typography>
                        <Typography variant="h5">{calculateTotalFertilization()} kg/da</Typography>
                      </Paper>
                    </Grid>
                    <Grid item xs={12} md={4}>
                      <Paper sx={{ p: 2 }}>
                        <Typography variant="subtitle1">Ortalama Verim</Typography>
                        <Typography variant="h5">{calculateAverageYield().toFixed(2)} kg/da</Typography>
                      </Paper>
                    </Grid>
                    <Grid item xs={12} md={6}>
                      <Paper sx={{ p: 2 }}>
                        <Typography variant="subtitle1">Sulama Verimliliği</Typography>
                        <Typography variant="h5">{calculateIrrigationEfficiency().toFixed(2)} kg/L</Typography>
                      </Paper>
                    </Grid>
                    <Grid item xs={12} md={6}>
                      <Paper sx={{ p: 2 }}>
                        <Typography variant="subtitle1">Gübreleme Verimliliği</Typography>
                        <Typography variant="h5">{calculateFertilizationEfficiency().toFixed(2)} kg/kg</Typography>
                      </Paper>
                    </Grid>
                  </Grid>
                </TabPanel>
              </>
            )}

            {/* Sulama Dialog */}
            <Dialog open={openIrrigationDialog} onClose={handleCloseIrrigationDialog} maxWidth="md" fullWidth>
              <DialogTitle>
                {currentIrrigation ? 'Sulama Kaydını Düzenle' : 'Yeni Sulama Kaydı Ekle'}
              </DialogTitle>
              <DialogContent>
                <Grid container spacing={2} sx={{ mt: 1 }}>
                  <Grid item xs={12} md={6}>
                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                      <DatePicker
                        label="Tarih"
                        value={irrigationForm.date}
                        onChange={(newValue: Date | null) => handleIrrigationFormChange('date', newValue)}
                        slotProps={{
                          textField: {
                            fullWidth: true,
                            error: !!formErrors.date,
                            helperText: formErrors.date
                          }
                        }}
                      />
                    </LocalizationProvider>
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <TextField
                      fullWidth
                      label="Miktar"
                      type="number"
                      value={Number.isNaN(irrigationForm.amount) ? '' : irrigationForm.amount}
                      onChange={(e) => handleIrrigationFormChange('amount', e.target.value === '' ? 0 : Number(e.target.value))}
                      error={!!formErrors.amount}
                      helperText={formErrors.amount}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <TextField
                      fullWidth
                      label="Birim"
                      value={irrigationForm.unit}
                      onChange={(e) => handleIrrigationFormChange('unit', e.target.value)}
                      error={!!formErrors.unit}
                      helperText={formErrors.unit}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <TextField
                      fullWidth
                      label="Yöntem"
                      value={irrigationForm.method}
                      onChange={(e) => handleIrrigationFormChange('method', e.target.value)}
                      error={!!formErrors.method}
                      helperText={formErrors.method}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <TextField
                      fullWidth
                      label="Toprak Nemi (Öncesi)"
                      type="number"
                      value={Number.isNaN(irrigationForm.soilMoistureBeforeIrrigation) ? '' : irrigationForm.soilMoistureBeforeIrrigation || ''}
                      onChange={(e) => handleIrrigationFormChange('soilMoistureBeforeIrrigation', e.target.value === '' ? null : Number(e.target.value))}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <TextField
                      fullWidth
                      label="Toprak Nemi (Sonrası)"
                      type="number"
                      value={Number.isNaN(irrigationForm.soilMoistureAfterIrrigation) ? '' : irrigationForm.soilMoistureAfterIrrigation || ''}
                      onChange={(e) => handleIrrigationFormChange('soilMoistureAfterIrrigation', e.target.value === '' ? null : Number(e.target.value))}
                    />
                  </Grid>
                  <Grid item xs={12}>
                    <TextField
                      fullWidth
                      label="Notlar"
                      multiline
                      rows={3}
                      value={irrigationForm.notes}
                      onChange={(e) => handleIrrigationFormChange('notes', e.target.value)}
                    />
                  </Grid>
                </Grid>
              </DialogContent>
              <DialogActions>
                <Button onClick={handleCloseIrrigationDialog} disabled={loading}>İptal</Button>
                <Button onClick={handleSaveIrrigation} variant="contained" disabled={loading}>
                  {loading ? <CircularProgress size={24} /> : 'Kaydet'}
                </Button>
              </DialogActions>
            </Dialog>

            {/* Gübreleme Dialog */}
            <Dialog open={openFertilizationDialog} onClose={handleCloseFertilizationDialog} maxWidth="md" fullWidth>
              <DialogTitle>
                {currentFertilization ? 'Gübreleme Kaydını Düzenle' : 'Yeni Gübreleme Kaydı Ekle'}
              </DialogTitle>
              <DialogContent>
                <Grid container spacing={2} sx={{ mt: 1 }}>
                  <Grid item xs={12} md={6}>
                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                      <DatePicker
                        label="Tarih"
                        value={fertilizationForm.date}
                        onChange={(newValue: Date | null) => handleFertilizationFormChange('date', newValue)}
                        slotProps={{
                          textField: {
                            fullWidth: true,
                            error: !!formErrors.date,
                            helperText: formErrors.date
                          }
                        }}
                      />
                    </LocalizationProvider>
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <FormControl fullWidth error={!!formErrors.fertilizerTypeId}>
                      <InputLabel id="fertilizer-type-label">Gübre Türü</InputLabel>
                      <Select
                        labelId="fertilizer-type-label"
                        value={fertilizationForm.fertilizerTypeId}
                        label="Gübre Türü"
                        onChange={(e) => handleFertilizationFormChange('fertilizerTypeId', e.target.value)}
                      >
                        {Array.isArray(fertilizerTypes) ? fertilizerTypes.map((type) => (
                          <MenuItem key={`fertilizer-type-${type.id}`} value={type.id}>
                            {type.name}
                          </MenuItem>
                        )) : (
                          <MenuItem value="">Gübre türleri yüklenemedi</MenuItem>
                        )}
                      </Select>
                      {formErrors.fertilizerTypeId && (
                        <FormHelperText>{formErrors.fertilizerTypeId}</FormHelperText>
                      )}
                    </FormControl>
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <TextField
                      fullWidth
                      label="Miktar"
                      type="number"
                      value={Number.isNaN(fertilizationForm.amount) ? '' : fertilizationForm.amount}
                      onChange={(e) => handleFertilizationFormChange('amount', e.target.value === '' ? 0 : Number(e.target.value))}
                      error={!!formErrors.amount}
                      helperText={formErrors.amount}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <TextField
                      fullWidth
                      label="Birim"
                      value={fertilizationForm.unit}
                      onChange={(e) => handleFertilizationFormChange('unit', e.target.value)}
                      error={!!formErrors.unit}
                      helperText={formErrors.unit}
                    />
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <TextField
                      fullWidth
                      label="Yöntem"
                      value={fertilizationForm.method}
                      onChange={(e) => handleFertilizationFormChange('method', e.target.value)}
                      error={!!formErrors.method}
                      helperText={formErrors.method}
                    />
                  </Grid>
                  <Grid item xs={12}>
                    <TextField
                      fullWidth
                      label="Notlar"
                      multiline
                      rows={3}
                      value={fertilizationForm.notes}
                      onChange={(e) => handleFertilizationFormChange('notes', e.target.value)}
                    />
                  </Grid>
                </Grid>
              </DialogContent>
              <DialogActions>
                <Button onClick={handleCloseFertilizationDialog} disabled={loading}>İptal</Button>
                <Button onClick={handleSaveFertilization} variant="contained" disabled={loading}>
                  {loading ? <CircularProgress size={24} /> : 'Kaydet'}
                </Button>
              </DialogActions>
            </Dialog>

            {/* Silme Onay Dialog */}
            <Dialog open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
              <DialogTitle>Silme Onayı</DialogTitle>
              <DialogContent>
                <DialogContentText>
                  Bu {deleteType === 'irrigation' ? 'sulama' : 'gübreleme'} kaydını silmek istediğinizden emin misiniz?
                  Bu işlem geri alınamaz.
                </DialogContentText>
              </DialogContent>
              <DialogActions>
                <Button onClick={handleCloseDeleteDialog} disabled={loading}>İptal</Button>
                <Button onClick={handleDelete} color="error" disabled={loading}>
                  {loading ? <CircularProgress size={24} /> : 'Sil'}
                </Button>
              </DialogActions>
            </Dialog>

            {/* Snackbar */}
            <Snackbar
              open={snackbarOpen}
              autoHideDuration={6000}
              onClose={handleCloseSnackbar}
              message={snackbarMessage}
              action={
                <IconButton
                  size="small"
                  aria-label="close"
                  color="inherit"
                  onClick={handleCloseSnackbar}
                >
                  <CloseIcon fontSize="small" />
                </IconButton>
              }
            />
          </Box>
        </Container>
      </Box>
    </Box>
  );
};

export default IrrigationFertilizationAnalysis;