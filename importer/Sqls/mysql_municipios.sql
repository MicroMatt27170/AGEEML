CREATE TABLE `municipios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `estado_id` int NOT NULL COMMENT 'Relación con estados.id',
  `clave` varchar(3) NOT NULL COMMENT 'Cve_Mun – Clave del municipio',
  `nombre` varchar(100) NOT NULL COMMENT 'Nom_Mun – Nombre del municipio',
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `estado_id` (`estado_id`),
  CONSTRAINT `fk_municipios_estados` FOREIGN KEY (`estado_id`) REFERENCES `estados` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Tabla de Municipios de la Republica Mexicana';
