CREATE TABLE estados (
  -- Llave primaria
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  -- Cve_Ent - Clave de la entidad
  clave TEXT NOT NULL UNIQUE,
  -- Nom_Ent - Nombre de la entidad
  nombre TEXT NOT NULL,
  -- Nom_Abr - Nombre abreviado de la entidad
  abrev TEXT NOT NULL,
  activo INTEGER NOT NULL DEFAULT 1
);
