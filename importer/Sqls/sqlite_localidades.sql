CREATE TABLE localidades (
  -- Llave primaria
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  -- Relación con municipios.id
  municipio_id INTEGER NOT NULL REFERENCES municipios(id) ON DELETE CASCADE ON UPDATE CASCADE,
  -- Cve_Loc – Clave de la localidad
  clave TEXT NOT NULL,
  -- Nom_Loc – Nombre de la localidad
  nombre TEXT NOT NULL,
  -- Mapa - Identificador del INEGI
  mapa INTEGER NOT NULL,
  -- Ámbito - Clasificación
  ambito TEXT NOT NULL,
  -- Latitud – Latitud en formato DMS
  latitud TEXT NOT NULL,
  -- Longitud – Longitud en formato DMS
  longitud TEXT NOT NULL,
  -- Lat_Decimal - Latitud en formato DD
  lat REAL NOT NULL,
  -- Lon_Decimal - Longitud en formato DD
  lng REAL NOT NULL,
  -- Altitud – Altitud
  altitud TEXT NOT NULL,
  -- Cve_Carta - Clave de carta topográfica
  carta TEXT NOT NULL,
  -- Pob_Total – Población Total
  poblacion INTEGER NOT NULL,
  -- Pob_Masculina – Población Masculina
  masculino INTEGER NOT NULL,
  -- Pob_Femenina – Población Femenina
  femenino INTEGER NOT NULL,
  -- Total De Viviendas Habitadas
  viviendas INTEGER NOT NULL,
  activo INTEGER NOT NULL DEFAULT 1
);
CREATE INDEX idx_localidades_municipio_id ON localidades(municipio_id);
