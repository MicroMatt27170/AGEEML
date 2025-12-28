CREATE TABLE municipios (
  id SERIAL PRIMARY KEY,
  estado_id INTEGER NOT NULL REFERENCES estados(id) ON DELETE CASCADE ON UPDATE CASCADE,
  clave VARCHAR(3) NOT NULL,
  nombre VARCHAR(100) NOT NULL,
  activo SMALLINT NOT NULL DEFAULT 1
);
CREATE INDEX idx_municipios_estado_id ON municipios(estado_id);
COMMENT ON TABLE municipios IS 'Tabla de Municipios de la Republica Mexicana';
COMMENT ON COLUMN municipios.estado_id IS 'Relación con estados.id';
COMMENT ON COLUMN municipios.clave IS 'Cve_Mun – Clave del municipio';
COMMENT ON COLUMN municipios.nombre IS 'Nom_Mun – Nombre del municipio';
