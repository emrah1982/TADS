import React, { useState, useEffect } from 'react';
import { Box, Typography, AppBar, Toolbar, Paper, Container, Grid, CardHeader, Divider, IconButton } from '@mui/material';
import { 
  Map as MapIcon, 
  Image as ImageIcon, 
  Sensors as SensorsIcon, 
  BugReport as BugReportIcon,
  Grass as GrassIcon,
  Event as EventIcon,
  History as HistoryIcon,
  Assessment as AssessmentIcon,
  SystemUpdate as SystemUpdateIcon,
  Menu as MenuIcon
} from '@mui/icons-material';
import Sidebar from './Sidebar';
import { useAuth } from '../contexts/AuthContext';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  sub: string;
  email: string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string | string[];
  exp: number;
}

const Dashboard: React.FC = () => {
  const { token } = useAuth();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [userName, setUserName] = useState<string>('Kullanıcı');

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

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };
  
  return (
    <Box sx={{ display: 'flex', height: '100vh' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={handleDrawerToggle}
            sx={{ mr: 2, display: { sm: 'none' } }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            TADS - Tarım Analiz ve Denetleme Sistemi
          </Typography>
        </Toolbar>
      </AppBar>
      
      {/* Sidebar bileşeni */}
      <Sidebar />
      
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          mt: 8,
          backgroundColor: '#f5f5f5',
          width: { sm: `calc(100% - 300px)` }
        }}
      >
        <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
          <Typography variant="h4" gutterBottom sx={{ mb: 4 }}>
            Hoş geldiniz, {userName}!
          </Typography>
          
          {/* Ana Dashboard Grid */}
          <Grid container spacing={3}>
            {/* Üst Satır */}
            <Grid item xs={12} md={6}>
              <Paper 
                elevation={3} 
                sx={{ 
                  p: 2, 
                  display: 'flex', 
                  flexDirection: 'column', 
                  height: 300,
                  position: 'relative'
                }}
              >
                <CardHeader 
                  title="Parsel Haritası" 
                  avatar={<MapIcon color="primary" />} 
                />
                <Divider />
                <Box sx={{ 
                  display: 'flex', 
                  justifyContent: 'center', 
                  alignItems: 'center',
                  height: '100%',
                  backgroundColor: '#e8f5e9',
                  borderRadius: 1
                }}>
                  <Typography variant="body2" color="text.secondary">
                    Parsel haritası burada görüntülenecek
                  </Typography>
                </Box>
              </Paper>
            </Grid>
            
            <Grid item xs={12} md={6}>
              <Paper 
                elevation={3} 
                sx={{ 
                  p: 2, 
                  display: 'flex', 
                  flexDirection: 'column', 
                  height: 300,
                  position: 'relative'
                }}
              >
                <CardHeader 
                  title="Anlık Görüntü İşleme" 
                  avatar={<ImageIcon color="primary" />} 
                  subheader="✔️ ⚠️ ❌ Sayılar + Görsel"
                />
                <Divider />
                <Box sx={{ 
                  display: 'flex', 
                  justifyContent: 'center', 
                  alignItems: 'center',
                  height: '100%',
                  backgroundColor: '#e3f2fd',
                  borderRadius: 1
                }}>
                  <Typography variant="body2" color="text.secondary">
                    Anlık görüntü işleme sonuçları burada görüntülenecek
                  </Typography>
                </Box>
              </Paper>
            </Grid>
            
            {/* Orta Satır */}
            <Grid item xs={12} md={6}>
              <Paper 
                elevation={3} 
                sx={{ 
                  p: 2, 
                  display: 'flex', 
                  flexDirection: 'column', 
                  height: 300,
                  position: 'relative'
                }}
              >
                <CardHeader 
                  title="Sensör Verileri" 
                  avatar={<SensorsIcon color="primary" />} 
                  subheader="Toprak, Hava, pH..."
                />
                <Divider />
                <Box sx={{ 
                  display: 'flex', 
                  justifyContent: 'center', 
                  alignItems: 'center',
                  height: '100%',
                  backgroundColor: '#fff3e0',
                  borderRadius: 1
                }}>
                  <Typography variant="body2" color="text.secondary">
                    Sensör verileri burada görüntülenecek
                  </Typography>
                </Box>
              </Paper>
            </Grid>
            
            <Grid item xs={12} md={6}>
              <Paper 
                elevation={3} 
                sx={{ 
                  p: 2, 
                  display: 'flex', 
                  flexDirection: 'column', 
                  height: 300,
                  position: 'relative'
                }}
              >
                <CardHeader 
                  title="Son 24 Saatlik Hastalık" 
                  avatar={<BugReportIcon color="primary" />} 
                  subheader="Grafik: Pasta/Çubuk"
                />
                <Divider />
                <Box sx={{ 
                  display: 'flex', 
                  justifyContent: 'center', 
                  alignItems: 'center',
                  height: '100%',
                  backgroundColor: '#fce4ec',
                  borderRadius: 1
                }}>
                  <Typography variant="body2" color="text.secondary">
                    Son 24 saatlik hastalık verileri burada görüntülenecek
                  </Typography>
                </Box>
              </Paper>
            </Grid>
            
            {/* Alt Satır */}
            <Grid item xs={12} md={6}>
              <Paper 
                elevation={3} 
                sx={{ 
                  p: 2, 
                  display: 'flex', 
                  flexDirection: 'column', 
                  height: 300,
                  position: 'relative'
                }}
              >
                <CardHeader 
                  title="Bitki Gelişim Grafiği" 
                  avatar={<GrassIcon color="primary" />} 
                  subheader="Trend"
                />
                <Divider />
                <Box sx={{ 
                  display: 'flex', 
                  justifyContent: 'center', 
                  alignItems: 'center',
                  height: '100%',
                  backgroundColor: '#e0f7fa',
                  borderRadius: 1
                }}>
                  <Typography variant="body2" color="text.secondary">
                    Bitki gelişim grafiği burada görüntülenecek
                  </Typography>
                </Box>
              </Paper>
            </Grid>
            
            <Grid item xs={12} md={6}>
              <Paper 
                elevation={3} 
                sx={{ 
                  p: 2, 
                  display: 'flex', 
                  flexDirection: 'column', 
                  height: 300,
                  position: 'relative'
                }}
              >
                <CardHeader 
                  title="Yaklaşan Görevler" 
                  avatar={<EventIcon color="primary" />} 
                  subheader="Takvim + Görev Kartları"
                />
                <Divider />
                <Box sx={{ 
                  display: 'flex', 
                  justifyContent: 'center', 
                  alignItems: 'center',
                  height: '100%',
                  backgroundColor: '#f3e5f5',
                  borderRadius: 1
                }}>
                  <Typography variant="body2" color="text.secondary">
                    Yaklaşan görevler burada görüntülenecek
                  </Typography>
                </Box>
              </Paper>
            </Grid>
            
            {/* Alt Bilgi Satırı */}
            <Grid item xs={12} md={4}>
              <Paper 
                elevation={3} 
                sx={{ 
                  p: 2, 
                  display: 'flex', 
                  flexDirection: 'column', 
                  height: 200,
                  position: 'relative'
                }}
              >
                <CardHeader 
                  title="Son İşlemler" 
                  avatar={<HistoryIcon color="primary" />} 
                  subheader="Zaman Çizgisi"
                />
                <Divider />
                <Box sx={{ 
                  display: 'flex', 
                  justifyContent: 'center', 
                  alignItems: 'center',
                  height: '100%',
                  backgroundColor: '#fffde7',
                  borderRadius: 1
                }}>
                  <Typography variant="body2" color="text.secondary">
                    Son işlemler burada görüntülenecek
                  </Typography>
                </Box>
              </Paper>
            </Grid>
            
            <Grid item xs={12} md={4}>
              <Paper 
                elevation={3} 
                sx={{ 
                  p: 2, 
                  display: 'flex', 
                  flexDirection: 'column', 
                  height: 200,
                  position: 'relative'
                }}
              >
                <CardHeader 
                  title="Rapor Özetleri" 
                  avatar={<AssessmentIcon color="primary" />} 
                  subheader="7-30 gün verisi"
                />
                <Divider />
                <Box sx={{ 
                  display: 'flex', 
                  justifyContent: 'center', 
                  alignItems: 'center',
                  height: '100%',
                  backgroundColor: '#f1f8e9',
                  borderRadius: 1
                }}>
                  <Typography variant="body2" color="text.secondary">
                    Rapor özetleri burada görüntülenecek
                  </Typography>
                </Box>
              </Paper>
            </Grid>
            
            <Grid item xs={12} md={4}>
              <Paper 
                elevation={3} 
                sx={{ 
                  p: 2, 
                  display: 'flex', 
                  flexDirection: 'column', 
                  height: 200,
                  position: 'relative'
                }}
              >
                <CardHeader 
                  title="Sistem Durumu" 
                  avatar={<SystemUpdateIcon color="primary" />} 
                  subheader="Son güncelleme, AI"
                />
                <Divider />
                <Box sx={{ 
                  display: 'flex', 
                  justifyContent: 'center', 
                  alignItems: 'center',
                  height: '100%',
                  backgroundColor: '#e8eaf6',
                  borderRadius: 1
                }}>
                  <Typography variant="body2" color="text.secondary">
                    Sistem durumu burada görüntülenecek
                  </Typography>
                </Box>
              </Paper>
            </Grid>
          </Grid>
        </Container>
      </Box>
    </Box>
  );
};

export default Dashboard;
