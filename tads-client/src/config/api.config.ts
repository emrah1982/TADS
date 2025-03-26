// API yapılandırma dosyası
// API port değişikliği gerektiğinde sadece bu dosyayı güncelleyin

const API_PORT = 5000;
const API_BASE_URL = `http://localhost:${API_PORT}/api`;

export const API_CONFIG = {
  BASE_URL: API_BASE_URL,
  ENDPOINTS: {
    // Auth endpoints
    AUTH: {
      LOGIN: `${API_BASE_URL}/Auth/login`,
      REGISTER: `${API_BASE_URL}/Auth/register`,
      CONFIRM_EMAIL: `${API_BASE_URL}/Auth/confirm-email`,
      RESET_PASSWORD: `${API_BASE_URL}/Auth/reset-password`,
      CHANGE_PASSWORD: `${API_BASE_URL}/Auth/change-password`,
      CHECK_ACCESS: `${API_BASE_URL}/Auth/check-access`
    },
    // User Management endpoints
    USER_MANAGEMENT: {
      USERS: `${API_BASE_URL}/UserManagement/users`,
      ROLES: `${API_BASE_URL}/UserManagement/roles`,
      USER_ROLES: `${API_BASE_URL}/UserManagement/user-roles`,
      ASSIGN_ROLE: `${API_BASE_URL}/UserManagement/assign-role`,
      REMOVE_ROLE: `${API_BASE_URL}/UserManagement/remove-role`,
      CONFIRM_EMAIL: `${API_BASE_URL}/UserManagement/confirm-email`,
      CREATE_ROLE: `${API_BASE_URL}/UserManagement/create-role`,
      CONFIRM_ALL_EMAILS: `${API_BASE_URL}/UserManagement/confirm-all-emails`
    },
    // Menu Management endpoints
    MENU: {
      ITEMS: `${API_BASE_URL}/user/menu-items`,
      CREATE: `${API_BASE_URL}/menu`,
      UPDATE: `${API_BASE_URL}/menu`,
      DELETE: `${API_BASE_URL}/menu`,
      ASSIGN_ROLES: `${API_BASE_URL}/menu/assign-roles`,
      ROLES: `${API_BASE_URL}/roles`
    },
    // Field Management endpoints
    FIELD_MANAGEMENT: {
      FIELDS: `${API_BASE_URL}/FieldManagement/fields`,
      FIELD: `${API_BASE_URL}/FieldManagement/field`,
      SOIL_TYPES: `${API_BASE_URL}/FieldManagement/soil-types`,
      SOIL_TYPE: `${API_BASE_URL}/FieldManagement/soil-type`,
      CROP_TYPES: `${API_BASE_URL}/FieldManagement/crop-types`
    },
    // Irrigation and Fertilization endpoints
    IRRIGATION_FERTILIZATION: {
      IRRIGATION: `${API_BASE_URL}/irrigation-fertilization/irrigation`,
      FERTILIZATION: `${API_BASE_URL}/irrigation-fertilization/fertilization`,
      FERTILIZER_TYPES: `${API_BASE_URL}/irrigation-fertilization/fertilizertypes`
    },
    // Yield endpoints
    YIELD: {
      DATA: `${API_BASE_URL}/yield`
    }
  }
};

export default API_CONFIG;
