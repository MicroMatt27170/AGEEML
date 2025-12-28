# Áreas Geoestadísticas Estatales, Municipales y Localidades

Base de Datos del INEGI en MySQL, PostgreSQL y SQLite

![INEGI AGEEML](https://developarts.com/bl-content/uploads/banner_ageeml.png)

El [INEGI](http://www.inegi.org.mx/) cuenta con **“Catálogo Único de Claves de Áreas Geoestadísticas Estatales, Municipales y Localidades”**  de la república mexicana que actualiza cada mes. El archivo fuente se encuentra en varios formatos y se puede descargar desde la sección [Catálogos Predefinidos](https://www.inegi.org.mx/app/ageeml/) y consultar el documento de [descripción](https://www.inegi.org.mx/contenidos/app/ageeml/Ayuda/Ayuda_Gral_Cat_Unico.pdf) de los campos.

En este proyecto, extraigo toda la información de un archivo _CSV_ versión _UTF-8_ y lo convierto a bases de datos MySQL, PostgreSQL y SQLite.

<!-- pagebreak -->

## Índice

1. [Diseño](#link1)
2. [Coordenadas Geográficas](#link2)
3. [Descarga](#link3)
4. [Diccionario de Datos](#link4)
5. [Contribuciones](#link6)
6. [Actualizaciones](#link5)

<!-- toc -->

## Diseño <a name="link1"></a>

El archivo contiene 3 tablas: `estados`, `municipios` y `localidades`. El diseño de la base de datos se muestra en la siguiente imagen:

He importado todos los campos que vienen en la base de datos del [INEGI](http://www.inegi.org.mx/), se pueden consultar en la sección _"Diccionario de Datos"_. Los campos importados están marcados en **negrita**.

La versión actual es: **2025.11**

La fecha de corte corresponde a **NOV 2025**. **Este dato es importante porque garantiza que el catálogo está actualizado a noviembre de 2025.**

Conteo de registros:

* **32** Estados
* **2,469** Municipios
* **300,690** Localidades

El peso de la base de datos es de: **37.3 MB**



## Coordenadas Geográficas <a name="link2"></a>

Los campos `latitud` y `longitud` vienen originalmente en un sistema de coordenadas **DMS** (Grados/Minutos/Segundos) . Actualmente el archivo también cuenta con dos campos con un sistema de coordenadas **DD** (Grados Decimales) en los campos `lat` y `lng` para ser ocupados en sistemas de mapas tipo _Google Maps_


## Descarga <a name="link3"></a>

Archivos de descarga por motor y formato:

| Formato | MySQL | PostgreSQL | SQLite |
| --- | --- | --- | --- |
| sql.gz | [![Descarga](http://developarts.com/bl-content/uploads/github.png)](mysql/ageeml.sql.gz) | [![Descarga](http://developarts.com/bl-content/uploads/github.png)](postgresql/ageeml.sql.gz) | [![Descarga](http://developarts.com/bl-content/uploads/github.png)](sqlite/ageeml.sql.gz) |
| db | — | — | [![Descarga](http://developarts.com/bl-content/uploads/github.png)](sqlite/ageeml.db) |

## Estructura del repositorio

- `mysql/`, `postgresql/`, `sqlite/`: scripts SQL (`.sql`) y comprimidos (`.sql.gz`) listos para importar.
- `sqlite/`: incluye además `ageeml.db` generado automáticamente.
- `importer/`: generador de SQL y base SQLite a partir del CSV del INEGI.
- `ageeml-odata-service/`: servicio OData para consumir el catálogo (Estados, Municipios y Localidades).
- `docker-compose.yml`: entorno de ejemplo con MySQL, PostgreSQL y el servicio OData.

Los datos están **actualizados a noviembre de 2025**. Este punto es crítico para asegurar que las claves y catálogos reflejan el último corte del INEGI.

### Regenerar SQL y base de datos (importer)

Puedes ejecutar el importador para sobrescribir los `.sql` y `.sql.gz` de MySQL, PostgreSQL y SQLite. En el caso de SQLite, también actualiza `sqlite/ageeml.db`. Esto descarga el archivo oficial del INEGI AGEEML y garantiza que los datos quedan actualizados.

```bash
DOTNET_ENVIRONMENT=Development dotnet run --project importer
```

## Servicio OData

El servicio `ageeml-odata-service` expone un API OData con las entidades **Estados**, **Municipios** y **Localidades**. Sirve una base de datos lista para usarse y permite consultas con filtros y proyecciones.

Ejemplos de consultas:

- `/api/v1/Estados?$select=id,nombre`
- `/api/v1/Municipios?$filter=estadoId eq 1`
- `/api/v1/Localidades?$filter=poblacion gt 1000000&$count=true`
- `/api/v1/Localidades?$top=10&$skip=20`
- `/api/v1/Localidades?$expand=municipio($select=id,nombre)`

### Levantar con Docker Compose

Puedes iniciar MySQL y PostgreSQL como ejemplo, además del servicio OData:

```bash
docker compose up
```

### Imagen pública del servicio OData

Si no quieres construir la imagen localmente, puedes usar la imagen pública:

```
micromatt27170/ageeml-odata-service
```



## Diccionario de Datos <a name="link4"></a>

Descripción de los campos de cada tabla del proyecto


### estados
| Columna | tipo | Comentarios |
| --- | --- | --- |
| `id` | _int(11)_ | 🔑 |
| `clave` | _varchar(2)_ | **Cve_Ent** - Clave de la entidad |
| `nombre` | _varchar(40)_ | **Nom_Ent** - Nombre de la entidad |
| `abrev` | _varchar(10)_ | **Nom_Abr** - Nombre abreviado de la entidad |
| `activo` | _tinyint(1)_ |  |


### municipios
| Columna | tipo | Comentarios |
| --- | --- | --- |
| `id` | _int(11)_ | 🔑 |
| `estado_id` | _int(11)_ | Relación: _`estados -> id`_ |
| `clave` | _varchar(3)_ | **Cve_Mun** - Clave del municipio |
| `nombre` | _varchar(100)_ | **Nom_Mun** - Nombre del municipio |
| `activo` | _tinyint(1)_ |  |


### localidades
| Columna | tipo | Comentarios |
| --- | --- | --- |
| `id` | _int(11)_ | 🔑 |
| `municipio_id` | _int(11)_ | Relación: _`municipios -> id`_ |
| `clave` | _varchar(4)_ | **Cve_Loc** – Clave de la localidad |
| `nombre` | _varchar(100)_ | **Nom_Loc** - Nombre de la localidad |
| `mapa` | _int(10)_ | **Mapa** - Identificador del INEGI |
| `ambito` | _varchar(1)_ | **Ámbito** - Clasificación |
| `latitud` | _varchar(15)_ | **Latitud** - Latitud en formato DMS |
| `longitud` | _varchar(15)_ | **Longitud** - Longitud en formato DMS |
| `lat` | _decimal(10,7)_ | **Lat_Decimal** Latitud en formato DD |
| `lng` | _decimal(10,7)_ | **Lon_Decimal** Longitud en formato DD |
| `altitud` | _varchar(15)_ | **Altitud** - Altitud |
| `carta` | _varchar(10)_ | **Cve_Carta** - Clave de carta topográfica |
| `poblacion` | _int(11)_ | **Pob_Total** - Población Total |
| `masculino` | _int(11)_ | **Pob_Masculina** - Población Masculina |
| `femenino` | _int(11)_ | **Pob_Femenina** - Población Femenina |
| `viviendas` | _int(11)_ | **Total De Viviendas Habitadas** - Total De Viviendas Habitadas |
| `activo` | tinyint(1) |  |

## Contribuciones <a name="link6"></a>

Este repositorio es un fork de https://github.com/developarts/AGEEML. El objetivo es complementar el proyecto con más proveedores de bases de datos más allá de MySQL, además de servicios para importar y actualizar las bases desde INEGI y un servicio out-of-the-box para microservicios (OData/WebAPI).



## Actualizaciones <a name="link5"></a>

* `[2025-12-28]` Se agregó soporte a bases de datos para PostgreSQL y SQLite, se creó un servicio para importar las bases desde el INEGI y un microservicio WebAPI con .NET.
* `[2020-11-20]` Se actualizó la información del INEGI a **OCT2020**.
*  `[2020-11-19]` El nombre del proyecto de renombró a **AGEEML**.
* `[2018-10-18]` Se actualizó la información del INEGI a **SEP2018**.
* `[2018-10-11]` Se creó el proyecto en GitHub para la distribución de los releases.
* `[2016-02-01]` Se actualizó la información del INEGI a **ENE2016**.
