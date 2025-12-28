CREATE TABLE localidades (
  id SERIAL PRIMARY KEY,
  municipio_id INTEGER NOT NULL REFERENCES municipios(id) ON DELETE CASCADE ON UPDATE CASCADE,
  clave VARCHAR(4) NOT NULL,
  nombre VARCHAR(100) NOT NULL,
  mapa INTEGER NOT NULL,
  ambito VARCHAR(1) NOT NULL,
  latitud VARCHAR(20) NOT NULL,
  longitud VARCHAR(20) NOT NULL,
  lat DECIMAL(10,7) NOT NULL,
  lng DECIMAL(10,7) NOT NULL,
  altitud VARCHAR(15) NOT NULL,
  carta VARCHAR(10) NOT NULL,
  poblacion INTEGER NOT NULL,
  masculino INTEGER NOT NULL,
  femenino INTEGER NOT NULL,
  viviendas INTEGER NOT NULL,
  activo SMALLINT NOT NULL DEFAULT 1
);
CREATE INDEX idx_localidades_municipio_id ON localidades(municipio_id);
COMMENT ON TABLE localidades IS 'Tabla de Localidades de la Republica Mexicana';
COMMENT ON COLUMN localidades.municipio_id IS 'Relación con municipios.id';
COMMENT ON COLUMN localidades.clave IS 'Cve_Loc – Clave de la localidad';
COMMENT ON COLUMN localidades.nombre IS 'Nom_Loc – Nombre de la localidad';
COMMENT ON COLUMN localidades.mapa IS 'Mapa - Identificador del INEGI';
COMMENT ON COLUMN localidades.ambito IS 'Ámbito - Clasificación';
COMMENT ON COLUMN localidades.latitud IS 'Latitud – Latitud en formato DMS';
COMMENT ON COLUMN localidades.longitud IS 'Longitud – Longitud en formato DMS';
COMMENT ON COLUMN localidades.lat IS 'Lat_Decimal - Latitud en formato DD';
COMMENT ON COLUMN localidades.lng IS 'Lon_Decimal - Longitud en formato DD';
COMMENT ON COLUMN localidades.altitud IS 'Altitud – Altitud';
COMMENT ON COLUMN localidades.carta IS 'Cve_Carta - Clave de carta topográfica';
COMMENT ON COLUMN localidades.poblacion IS 'Pob_Total – Población Total';
COMMENT ON COLUMN localidades.masculino IS 'Pob_Masculina – Población Masculina';
COMMENT ON COLUMN localidades.femenino IS 'Pob_Femenina – Población Femenina';
COMMENT ON COLUMN localidades.viviendas IS 'Total De Viviendas Habitadas';
