# Importador AGEEML

Este importador descarga los catálogos oficiales del INEGI (entidades, municipios y localidades), selecciona el CSV no-UTF, normaliza claves y genera:

- Scripts SQL para MySQL, PostgreSQL y SQLite.
- Base de datos SQLite lista para usar (`ageeml.db`).
- Archivos comprimidos `.sql.gz` para distribución.

## Flujo de trabajo

1. Descarga los ZIP desde INEGI y valida que sean ZIP reales.
2. Extrae el CSV cuyo nombre no contiene `utf`.
3. Lee los CSV con `CsvHelper` ignorando escapes problemáticos.
4. Normaliza claves con padding (`CVE_ENT` 2, `CVE_MUN` 3, `CVE_LOC` 4).
5. Genera IDs internos ordenados por clave para asegurar consistencia.
6. Escribe los DDL desde archivos en `Sqls/`.
7. Inserta datos en lotes para los scripts SQL.
8. Crea y llena `sqlite/ageeml.db` con transacciones.
9. Comprime los `.sql` a `.sql.gz`.

## Uso

Desde la raíz del repo:

```bash
# Build
DOTNET_ENVIRONMENT=Development dotnet build importer

# Run
DOTNET_ENVIRONMENT=Development dotnet run --project importer
```

### Sobrescribir opciones

Las opciones se cargan en este orden (última gana):

1. `importer/appsettings.json`
2. User secrets
3. Variables de entorno
4. Argumentos de línea de comandos

Ejemplos:

```bash
# Cambiar URL de localidades
DOTNET_ENVIRONMENT=Development dotnet run --project importer -- \
  Download:LocalidadesUrl=https://www.inegi.org.mx/contenidos/app/ageeml/may_acento.zip

# Cambiar ruta de salida base
DOTNET_ENVIRONMENT=Development dotnet run --project importer -- Output:RootPath=/ruta/al/repo
```

Variables de entorno equivalentes:

```bash
export Download__LocalidadesUrl="https://..."
export Output__RootPath="/ruta/al/repo"
```

## Salidas

Archivos generados (por defecto):

- `mysql/ageeml.sql.gz`
- `postgresql/ageeml.sql.gz`
- `sqlite/ageeml.sql.gz`
- `sqlite/ageeml.db`

Los `.sql` planos se generan y se comprimen; están ignorados por Git.

## Componentes

- `Program.cs`: orquestación del flujo completo.
- `Models/`: modelos `Estado`, `Municipio`, `Localidad`.
- `Extensions/`:
  - `CsvUtilities`: configuración de `CsvHelper`.
  - `TextUtilities`: parseo, padding, formateo y escaping.
  - `FileUtilities`: compresión `.gz` e inserts por lote.
  - `PathUtilities`: resolución de rutas.
- `Options/`:
  - `DownloadOptions`: URLs de descarga.
  - `OutputOptions`: rutas de salida.
  - `SqlOptions`: ubicación de DDL.
- `Sqls/`: DDL por proveedor (`{provider}_{table}.sql`).
- `appsettings.json`: valores por defecto.

## Opciones disponibles

### Download

- `EstadosUrl`
- `MunicipiosUrl`
- `LocalidadesUrl`

### Output

- `RootPath`: ruta base del repo (vacío = detectar automáticamente).
- `WorkDir`: carpeta temporal para descargas.
- `MysqlSqlPath`
- `PostgreSqlSqlPath`
- `SqliteSqlPath`
- `SqliteDbPath`

### Sql

- `SqlsDirectory`: carpeta donde están los DDL (`Sqls`).

## Detalles de implementación

- El CSV de localidades contiene comillas dentro de campos (`LATITUD`, `LONGITUD`), por eso se usa `CsvHelper` en modo `NoEscape`.
- Los campos numéricos ausentes se convierten a `0` para cumplir con DDL no nulo.
- Las claves se normalizan con padding para evitar inconsistencias al mapear relaciones.
- SQLite se carga con PRAGMAs para mejorar el rendimiento de inserción masiva.

## Soporte y mantenimiento

- Si INEGI cambia nombres o encabezados de columnas, actualizar los nombres de campos en `Program.cs`.
- Si cambia el formato del ZIP, revisar `DownloadAndExtractAsync`.
- Para agregar otro motor, crear DDL en `Sqls/` y un método de salida equivalente.
