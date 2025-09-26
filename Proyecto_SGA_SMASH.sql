CREATE DATABASE Proyecto_SGA_SMASH;
USE Proyecto_SGA_SMASH;

-- Tabla Roles
CREATE TABLE Roles (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(50) NOT NULL,
    descripcion VARCHAR(255)
);

-- Tabla Empleado
CREATE TABLE Empleado (
    id INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL,
    Puesto VARCHAR(100),
    SalarioBase DECIMAL(10, 2),
    FechaIngreso DATE,
    Estado VARCHAR(50)
);

-- Tabla Vacacion
CREATE TABLE Vacacion (
    id INT PRIMARY KEY IDENTITY(1,1),
    empleado_id INT,
    fecha_inicio DATE,
    fecha_fin DATE,
    estado VARCHAR(50),
    dias_solicitados INT,
    fecha_solicitud DATE,
    aprobado_por INT,
    FOREIGN KEY (empleado_id) REFERENCES Empleado(id),
    FOREIGN KEY (aprobado_por) REFERENCES Empleado(id)
);

-- Tabla Planilla
CREATE TABLE Planilla (
    id INT PRIMARY KEY IDENTITY(1,1),
    empleado_id INT,
    mes INT,
    anio INT,
    salario_base DECIMAL(10, 2),
    bonificaciones DECIMAL(10, 2),
    deducciones DECIMAL(10, 2),
    FOREIGN KEY (empleado_id) REFERENCES Empleado(id)
);

-- Tabla Usuario
CREATE TABLE Usuario (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    correo VARCHAR(100) UNIQUE,
    rol_id INT,
    contrasena VARCHAR(255) NOT NULL,
    fecha_creacion DATETIME,
    ultimo_acceso DATETIME,
    FOREIGN KEY (rol_id) REFERENCES Roles(id)
);

-- Tabla Bitacora
CREATE TABLE Bitacora (
    id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT,
    accion VARCHAR(255),
    tabla_afectada VARCHAR(100),
    id_registro_afectado INT,
    descripcion TEXT,
    fecha_hora DATETIME,
    FOREIGN KEY (usuario_id) REFERENCES Usuario(id)
);

-- Tabla Cliente
CREATE TABLE Cliente (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    correo VARCHAR(100) UNIQUE,
    telefono VARCHAR(20),
    FechaRegistro DATETIME
);

-- Tabla Reservacion
CREATE TABLE Reservacion (
    id INT PRIMARY KEY IDENTITY(1,1),
    cliente_id INT,
    fecha_hora DATETIME,
    mesa VARCHAR(50),
    estado VARCHAR(50),
    registrado_por INT,
    FOREIGN KEY (cliente_id) REFERENCES Cliente(id),
    FOREIGN KEY (registrado_por) REFERENCES Usuario(id)
);

-- Tabla Gasto
CREATE TABLE Gasto (
    id INT PRIMARY KEY IDENTITY(1,1),
    monto DECIMAL(10, 2) NOT NULL,
    fecha DATE,
    tipo VARCHAR(50),
    descripcion TEXT,
    registrado_por INT,
    FOREIGN KEY (registrado_por) REFERENCES Usuario(id)
);

-- Tabla Proveedor
CREATE TABLE Proveedor (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    contacto VARCHAR(100),
    telefono VARCHAR(20),
    correo VARCHAR(100)
);

-- Tabla Producto
CREATE TABLE Producto (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    unidad_medida VARCHAR(50),
    precio_unitario DECIMAL(10, 2),
    stock_actual DECIMAL(10, 2),
    minimo_stock DECIMAL(10, 2)
);

-- Tabla Producto_Proveedor
CREATE TABLE Producto_Proveedor (
    id INT PRIMARY KEY IDENTITY(1,1),
    producto_id INT,
    proveedor_id INT,
    precio_unitario DECIMAL(10, 2),
    tiempo_entrega_dias INT,
    fecha_registro DATETIME,
    activo BIT,
    FOREIGN KEY (producto_id) REFERENCES Producto(id),
    FOREIGN KEY (proveedor_id) REFERENCES Proveedor(id)
);

-- Tabla Inventario
CREATE TABLE Inventario (
    id INT PRIMARY KEY IDENTITY(1,1),
    registrado_por INT,
    tipo_movimiento VARCHAR(50),
    cantidad DECIMAL(10, 2),
    fecha_movimiento DATETIME,
    motivo TEXT,
    producto_id INT,
    FOREIGN KEY (registrado_por) REFERENCES Usuario(id),
    FOREIGN KEY (producto_id) REFERENCES Producto(id)
);