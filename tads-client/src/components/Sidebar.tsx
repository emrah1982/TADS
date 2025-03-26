import React, { useState, useEffect } from 'react';
import { 
  Drawer, 
  List, 
  ListItemIcon, 
  ListItemText, 
  Collapse, 
  Typography,
  ListItemButton,
  Divider,
  Button,
  CircularProgress
} from '@mui/material';
import {
  MonitorOutlined,
  BugReportOutlined,
  ThermostatOutlined,
  LocationOnOutlined,
  BarChartOutlined,
  WaterDropOutlined,
  CalendarTodayOutlined,
  HistoryOutlined,
  PeopleOutlined,
  TerrainOutlined,
  NotificationsOutlined,
  BuildOutlined,
  AssignmentOutlined,
  PestControlOutlined,
  AgricultureOutlined,
  ExpandLess,
  ExpandMore,
  LogoutOutlined,
  DashboardOutlined
} from '@mui/icons-material';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

interface MenuItem {
  title: string;
  icon: React.ReactNode;
  items: {
    title: string;
    icon: React.ReactNode;
    path?: string;
  }[];
}

interface ApiMenuItem {
  title: string;
  iconName: string;
  subPages: {
    title: string;
    iconName: string;
    path?: string;
  }[];
}

interface JwtPayload {
  sub: string;
  email: string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string | string[];
  exp: number;
}

// İkon adından React komponenti döndüren yardımcı fonksiyon
const getIconByName = (iconName: string): React.ReactNode => {
  const iconMap: {[key: string]: React.ReactNode} = {
    'MonitorOutlined': <MonitorOutlined />,
    'BugReportOutlined': <BugReportOutlined />,
    'ThermostatOutlined': <ThermostatOutlined />,
    'LocationOnOutlined': <LocationOnOutlined />,
    'BarChartOutlined': <BarChartOutlined />,
    'WaterDropOutlined': <WaterDropOutlined />,
    'CalendarTodayOutlined': <CalendarTodayOutlined />,
    'HistoryOutlined': <HistoryOutlined />,
    'PeopleOutlined': <PeopleOutlined />,
    'TerrainOutlined': <TerrainOutlined />,
    'NotificationsOutlined': <NotificationsOutlined />,
    'BuildOutlined': <BuildOutlined />,
    'AssignmentOutlined': <AssignmentOutlined />,
    'PestControlOutlined': <PestControlOutlined />,
    'AgricultureOutlined': <AgricultureOutlined />,
    'DashboardOutlined': <DashboardOutlined />
  };
  
  return iconMap[iconName] || <DashboardOutlined />;
};

const Sidebar: React.FC = () => {
  const [openMenus, setOpenMenus] = useState<{ [key: string]: boolean }>({});
  const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
  const [loading, setLoading] = useState<boolean>(false); 
  const [userName, setUserName] = useState<string>('Kullanıcı');
  const { logout, token } = useAuth();
  const navigate = useNavigate();

  // Token'dan kullanıcı adını al
  useEffect(() => {
    if (token) {
      try {
        const decoded = jwtDecode<JwtPayload>(token);
        const name = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
        if (name) {
          setUserName(name);
        }
      } catch (error) {
        console.error('Token decode error:', error);
      }
    }
  }, [token]);

  // Statik menü öğeleri
  const staticMenuItems: MenuItem[] = [
    {
      title: 'Tarla Yönetimi',
      icon: <TerrainOutlined />,
      items: [
        { title: 'Parsel Haritası', icon: <LocationOnOutlined />, path: '/field-map' },
        { title: 'Parsel Tanımlamaları', icon: <TerrainOutlined />, path: '/field-management' },
        { title: 'Ekim Planı', icon: <CalendarTodayOutlined />, path: '/planting-plan' },
        { title: 'Sulama ve Gübreleme', icon: <WaterDropOutlined />, path: '/irrigation-fertilization-analysis' }
      ]
    },
    {
      title: 'Sensör Verileri',
      icon: <ThermostatOutlined />,
      items: [
        { title: 'Toprak Analizi', icon: <WaterDropOutlined />, path: '/soil-analysis' },
        { title: 'Hava Durumu', icon: <ThermostatOutlined />, path: '/weather' }
      ]
    },
    {
      title: 'Bitki Sağlığı',
      icon: <PestControlOutlined />,
      items: [
        { title: 'Hastalık Tespiti', icon: <BugReportOutlined />, path: '/disease-detection' },
        { title: 'Gelişim Takibi', icon: <BarChartOutlined />, path: '/growth-tracking' }
      ]
    },
    {
      title: 'Yönetim',
      icon: <BuildOutlined />,
      items: [
        { title: 'Kullanıcı Yönetimi', icon: <PeopleOutlined />, path: '/user-management' },
        { title: 'Sayfa Yönetimi', icon: <AssignmentOutlined />, path: '/page-management' },
        { title: 'Raporlar', icon: <AssignmentOutlined />, path: '/reports' }
      ]
    }
  ];

  // API'den menü öğelerini çek
  useEffect(() => {
    const fetchMenuItems = async () => {
      try {
        setLoading(true);
        const response = await fetch('http://localhost:5000/api/menu', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        
        if (!response.ok) {
          throw new Error('Menü öğeleri alınamadı');
        }
        
        const data = await response.json();
        console.log('API Response:', data); // Debug için
        
        // API'den gelen verileri frontend formatına dönüştür
        const formattedMenuItems: MenuItem[] = data.map((item: ApiMenuItem) => ({
          title: item.title,
          icon: getIconByName(item.iconName),
          items: (item.subPages || []).map(subItem => ({
            title: subItem.title,
            icon: getIconByName(subItem.iconName),
            path: subItem.path
          }))
        }));
        
        setMenuItems(formattedMenuItems);
      } catch (error) {
        console.error('Menü öğeleri yüklenirken hata oluştu:', error);
        // Hata durumunda statik menüyü göster
        setMenuItems(staticMenuItems);
      } finally {
        setLoading(false);
      }
    };
    
    if (token) {
      fetchMenuItems();
    } else {
      setMenuItems(staticMenuItems);
    }
  }, [token]);

  const handleMenuClick = (title: string) => {
    setOpenMenus(prev => ({
      ...prev,
      [title]: !prev[title]
    }));
  };

  const handleItemClick = (path?: string) => {
    if (path) {
      navigate(path);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const handleDashboard = () => {
    navigate('/dashboard');
  };

  return (
    <Drawer
      variant="permanent"
      anchor="left"
      sx={{
        width: 300,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: 300,
          boxSizing: 'border-box',
          backgroundColor: '#f5f5f5',
          borderRight: '1px solid rgba(0, 0, 0, 0.12)'
        }
      }}
    >
      <List sx={{ pt: 2 }}>
        <ListItemButton onClick={handleDashboard} sx={{ mb: 1 }}>
          <ListItemIcon><DashboardOutlined /></ListItemIcon>
          <ListItemText 
            primary={
              <Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
                Ana Sayfa
              </Typography>
            }
          />
        </ListItemButton>
        
        {userName && (
          <Typography 
            variant="subtitle2" 
            sx={{ 
              px: 2, 
              py: 1, 
              fontWeight: 'medium',
              color: 'text.secondary'
            }}
          >
            Hoş geldiniz, {userName}
          </Typography>
        )}
        
        <Divider sx={{ my: 1 }} />
        
        {loading ? (
          <div style={{ display: 'flex', justifyContent: 'center', padding: '20px' }}>
            <CircularProgress size={30} />
          </div>
        ) : (
          menuItems.map((menu) => (
            <React.Fragment key={menu.title}>
              <ListItemButton onClick={() => handleMenuClick(menu.title)}>
                <ListItemIcon>{menu.icon}</ListItemIcon>
                <ListItemText 
                  primary={
                    <Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
                      {menu.title}
                    </Typography>
                  }
                />
                {openMenus[menu.title] ? <ExpandLess /> : <ExpandMore />}
              </ListItemButton>
              <Collapse in={openMenus[menu.title]} timeout="auto" unmountOnExit>
                <List component="div" disablePadding>
                  {menu.items.map((item) => (
                    <ListItemButton
                      key={item.title}
                      sx={{ pl: 4, '&:hover': { backgroundColor: '#e0e0e0' } }}
                      onClick={() => handleItemClick(item.path)}
                    >
                      <ListItemIcon>{item.icon}</ListItemIcon>
                      <ListItemText primary={item.title} />
                    </ListItemButton>
                  ))}
                </List>
              </Collapse>
            </React.Fragment>
          ))
        )}
        
        <Divider sx={{ my: 1 }} />
        
        <ListItemButton onClick={handleLogout} sx={{ mt: 2 }}>
          <ListItemIcon><LogoutOutlined /></ListItemIcon>
          <ListItemText primary="Çıkış Yap" />
        </ListItemButton>
      </List>
    </Drawer>
  );
};

export default Sidebar;
