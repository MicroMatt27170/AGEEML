CREATE TABLE `estados` (
  `id` int NOT NULL AUTO_INCREMENT,
  `clave` varchar(2) NOT NULL COMMENT 'Cve_Ent - Clave de la entidad',
  `nombre` varchar(40) NOT NULL COMMENT 'Nom_Ent  - Nombre de la entidad',
  `abrev` varchar(10) NOT NULL COMMENT 'Nom_Abr - Nombre abreviado de la entidad',
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_estados_clave` (`clave`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Tabla de Estados de la República Mexicana';
