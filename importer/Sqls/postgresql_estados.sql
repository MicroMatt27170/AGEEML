CREATE TABLE estados (
  id SERIAL PRIMARY KEY,
  clave VARCHAR(2) NOT NULL UNIQUE,
  nombre VARCHAR(40) NOT NULL,
  abrev VARCHAR(10) NOT NULL,
  activo SMALLINT NOT NULL DEFAULT 1
);
COMMENT ON TABLE estados IS 'Tabla de Estados de la República Mexicana';
COMMENT ON COLUMN estados.clave IS 'Cve_Ent - Clave de la entidad';
COMMENT ON COLUMN estados.nombre IS 'Nom_Ent - Nombre de la entidad';
COMMENT ON COLUMN estados.abrev IS 'Nom_Abr - Nombre abreviado de la entidad';
