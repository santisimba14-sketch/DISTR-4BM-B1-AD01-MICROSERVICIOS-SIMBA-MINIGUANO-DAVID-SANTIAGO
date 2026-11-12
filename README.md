# Arquitectura de Microservicios - Comercio Multibodega

Proyecto desarrollado en **.NET Core (Web API)** bajo una arquitectura distribuida basada en microservicios independientes con comunicación síncrona vía HTTP (`IHttpClientFactory`) y bases de datos desacopladas en **SQL Server**.

* **Autor:** David Santiago Simba Miniguano
* **Asignatura:** Aplicaciones Distribuidas
* **Actividad:** AD01 - Diseño de Arquitecturas Distribuidas

---

## 1. Estructura del Repositorio

El repositorio está organizado en una solución contenedora con los siguientes recursos:

```text
DISTR-4BM-B1-AD01-MICROSERVICIOS-SIMBA-MINIGUANO-DAVID-SANTIAGO/
│
├── Catalogo/                 # Microservicio 1: Gestión de Categorías, Marcas y Productos
├── Inventario4DB/            # Microservicio 2: Gestión de Bodegas, Existencias, Movimientos y Reservas
├── Pedidos/                  # Microservicio 3: Gestión de Clientes, Pedidos y Consulta Distribuida
│
├── CatalogoDB.sql            # Script DDL y datos iniciales de CatalogoDB
├── BASEINVENTARIO4DB.sql     # Script DDL y datos iniciales de Inventario4DB
├── BASEPEDIDOS.sql           # Script DDL y datos iniciales de PedidosDB
│
├── POSTMAN.json              # Colección de Postman con las peticiones configuradas
├── .gitignore                # Exclusión de archivos compilados (bin/, obj/, .vs/)
└── README.md                 # Documentación e instrucciones de despliegue
```

---

## 2. Arquitectura y Puertos Locales

Cada microservicio opera de forma autónoma sobre su propia base de datos en SQL Server y expone su API REST en un puerto local fijo:

| Microservicio | Carpeta | Base de Datos | Puerto Local HTTP | Swagger UI |
| :--- | :--- | :--- | :---: | :--- |
| **Catalogo.Api** | `Catalogo/` | `CatalogoDB` | `5204` | `http://localhost:5204/swagger` |
| **Inventario.Api** | `Inventario4DB/` | `Inventario4DB` | `5117` | `http://localhost:5117/swagger` |
| **Pedidos.Api** | `Pedidos/` | `PedidosDB` | `5279` | `http://localhost:5279/swagger` |

### Comunicación Inter-Servicio (`Pedidos.Api` $\rightarrow$ `Catalogo.Api` e `Inventario.Api`)
* **Desacoplamiento de datos:** No existen claves foráneas físicas entre bases de datos distintas; la integridad se maneja mediante identificadores lógicos (`IdProducto`).
* **Integración HTTP:** El microservicio `Pedidos.Api` consume mediante `IHttpClientFactory` los endpoints de `Catalogo.Api` y `Inventario.Api` para consolidar la información del producto y verificar disponibilidad de stock en tiempo real.

---

## 3. Guía de Instalación y Despliegue Local

### Paso 1: Configurar las Bases de Datos en SQL Server
1. Abre **SQL Server Management Studio (SSMS)** y conéctate a tu instancia local (`localhost`).
2. Ejecuta los tres scripts incluidos en la raíz del repositorio en el siguiente orden:
   * `CatalogoDB.sql` (Crea `CatalogoDB`, sus tablas y registros base).
   * `BASEINVENTARIO4DB.sql` (Crea `Inventario4DB` y las tablas de control de bodegas y stock).
   * `BASEPEDIDOS.sql` (Crea `PedidosDB`, clientes y pedidos iniciales).
3. Verifica que las cadenas de conexión (`ConnectionStrings`) en el archivo `appsettings.json` de cada proyecto coincidan con el nombre de tu servidor SQL Server.

### Paso 2: Levantar los Microservicios
1. Abre cada proyecto en **Visual Studio 2022** (o ejecútalos desde terminal con `dotnet run` dentro de la carpeta de cada proyecto `.csproj`).
2. Asegúrate de iniciar los **tres servicios simultáneamente** utilizando los perfiles `http` configurados en `launchSettings.json` para que respeten los puertos `5204`, `5117` y `5279`.

---

## 4. Pruebas de Integración con Postman

1. Abre **Postman** y haz clic en el botón **Import**.
2. Selecciona el archivo **`POSTMAN.json`** ubicado en la raíz de este repositorio (si lo descargaste sin extensión, asegúrate de que conserve el formato JSON).
3. Ejecuta las peticiones en orden para validar cada servicio:
   * **Catálogo (`5204`):** `GET http://localhost:5204/api/Productos`
   * **Inventario (`5117`):** `GET http://localhost:5117/api/Existencias/por-producto/1`
   * **Pedidos (`5279`):** `GET http://localhost:5279/api/Pedidos`
4. **Prueba de Comunicación Distribuida (Endpoint Cruzado):**
   * Ejecuta la petición:
     ```http
     GET http://localhost:5279/api/Pedidos/1/con-catalogo-inventario
     ```
   * **Resultado esperado:** Código `200 OK` retornando un objeto JSON compuesto que integra los datos del pedido desde `PedidosDB`, los detalles del producto obtenidos vía HTTP desde `Catalogo.Api` y las existencias actuales consultadas en `Inventario.Api`.