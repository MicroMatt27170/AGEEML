CREATE TABLE municipios (
  -- Llave primaria
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  -- Relación con estados.id
  estado_id INTEGER NOT NULL REFERENCES estados(id) ON DELETE CASCADE ON UPDATE CASCADE,
  -- Cve_Mun – Clave del municipio
  clave TEXT NOT NULL,
  -- Nom_Mun – Nombre del municipio
  nombre TEXT NOT NULL,
  activo INTEGER NOT NULL DEFAULT 1
);
CREATE INDEX idx_municipios_estado_id ON municipios(estado_id);
