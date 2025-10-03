
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/10/2025 10:17:21
-- Generated from EDMX file: D:\Proyectos\c#\vercom 3.0\vercom\Models\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [VERCOM];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__CierreSub__Cierr__5C6CB6D7]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CierreSubMayor] DROP CONSTRAINT [FK__CierreSub__Cierr__5C6CB6D7];
GO
IF OBJECT_ID(N'[dbo].[FK__CierreSub__SubMa__5D60DB10]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CierreSubMayor] DROP CONSTRAINT [FK__CierreSub__SubMa__5D60DB10];
GO
IF OBJECT_ID(N'[dbo].[FK__Movimient__SubMa__2CBDA3B5]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MovimientoCaja] DROP CONSTRAINT [FK__Movimient__SubMa__2CBDA3B5];
GO
IF OBJECT_ID(N'[dbo].[FK__RegistroD__CajaI__32767D0B]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RegistroDiarioCaja] DROP CONSTRAINT [FK__RegistroD__CajaI__32767D0B];
GO
IF OBJECT_ID(N'[dbo].[FK__SubMayor__CajaID__2704CA5F]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubMayor] DROP CONSTRAINT [FK__SubMayor__CajaID__2704CA5F];
GO
IF OBJECT_ID(N'[dbo].[FK_CierreCaja_CajaPrincipal]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CierreCaja] DROP CONSTRAINT [FK_CierreCaja_CajaPrincipal];
GO
IF OBJECT_ID(N'[dbo].[FK_FKcliente_cu504141]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[cliente_cuenta] DROP CONSTRAINT [FK_FKcliente_cu504141];
GO
IF OBJECT_ID(N'[dbo].[FK_FKcliente_cu555547]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[cliente_cuenta] DROP CONSTRAINT [FK_FKcliente_cu555547];
GO
IF OBJECT_ID(N'[dbo].[FK_FKcliente423992]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[cliente] DROP CONSTRAINT [FK_FKcliente423992];
GO
IF OBJECT_ID(N'[dbo].[FK_FKnegocio542366]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[negocio] DROP CONSTRAINT [FK_FKnegocio542366];
GO
IF OBJECT_ID(N'[dbo].[FK_FKnegocio549625]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[negocio] DROP CONSTRAINT [FK_FKnegocio549625];
GO
IF OBJECT_ID(N'[dbo].[FK_FKnegocio736135]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[negocio] DROP CONSTRAINT [FK_FKnegocio736135];
GO
IF OBJECT_ID(N'[dbo].[FK_FKnegocio873465]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[negocio] DROP CONSTRAINT [FK_FKnegocio873465];
GO
IF OBJECT_ID(N'[dbo].[FK_FKnegocio968398]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[negocio] DROP CONSTRAINT [FK_FKnegocio968398];
GO
IF OBJECT_ID(N'[dbo].[FK_FKproducto393903]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[producto] DROP CONSTRAINT [FK_FKproducto393903];
GO
IF OBJECT_ID(N'[dbo].[FK_FKproducto607651]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[producto] DROP CONSTRAINT [FK_FKproducto607651];
GO
IF OBJECT_ID(N'[dbo].[FK_FKproducto660940]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[producto] DROP CONSTRAINT [FK_FKproducto660940];
GO
IF OBJECT_ID(N'[dbo].[FK_FKRolePermis471885]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RolePermissions] DROP CONSTRAINT [FK_FKRolePermis471885];
GO
IF OBJECT_ID(N'[dbo].[FK_FKRolePermis48370]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RolePermissions] DROP CONSTRAINT [FK_FKRolePermis48370];
GO
IF OBJECT_ID(N'[dbo].[FK_FKUserRoles613991]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRoles] DROP CONSTRAINT [FK_FKUserRoles613991];
GO
IF OBJECT_ID(N'[dbo].[FK_FKUserRoles959420]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRoles] DROP CONSTRAINT [FK_FKUserRoles959420];
GO
IF OBJECT_ID(N'[dbo].[FK_FKoperacion168701]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[operacion] DROP CONSTRAINT [FK_FKoperacion168701];
GO
IF OBJECT_ID(N'[dbo].[FK_FKoperacion21]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[operacion] DROP CONSTRAINT [FK_FKoperacion21];
GO
IF OBJECT_ID(N'[dbo].[FK_FKoperacion273527]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[operacion] DROP CONSTRAINT [FK_FKoperacion273527];
GO
IF OBJECT_ID(N'[dbo].[FK_FKoperacion325014]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[operacion] DROP CONSTRAINT [FK_FKoperacion325014];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AlertasSistema]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AlertasSistema];
GO
IF OBJECT_ID(N'[dbo].[area]', 'U') IS NOT NULL
    DROP TABLE [dbo].[area];
GO
IF OBJECT_ID(N'[dbo].[AuditoriaMovimientosCaja]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AuditoriaMovimientosCaja];
GO
IF OBJECT_ID(N'[dbo].[CajaPrincipal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CajaPrincipal];
GO
IF OBJECT_ID(N'[dbo].[categoria]', 'U') IS NOT NULL
    DROP TABLE [dbo].[categoria];
GO
IF OBJECT_ID(N'[dbo].[CierreCaja]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CierreCaja];
GO
IF OBJECT_ID(N'[dbo].[CierreSubMayor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CierreSubMayor];
GO
IF OBJECT_ID(N'[dbo].[cliente]', 'U') IS NOT NULL
    DROP TABLE [dbo].[cliente];
GO
IF OBJECT_ID(N'[dbo].[cliente_cuenta]', 'U') IS NOT NULL
    DROP TABLE [dbo].[cliente_cuenta];
GO
IF OBJECT_ID(N'[dbo].[ConfiguracionHorarioCaja]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ConfiguracionHorarioCaja];
GO
IF OBJECT_ID(N'[dbo].[cuenta]', 'U') IS NOT NULL
    DROP TABLE [dbo].[cuenta];
GO
IF OBJECT_ID(N'[dbo].[forma_operacion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[forma_operacion];
GO
IF OBJECT_ID(N'[dbo].[medio_pago]', 'U') IS NOT NULL
    DROP TABLE [dbo].[medio_pago];
GO
IF OBJECT_ID(N'[dbo].[MovimientoCaja]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MovimientoCaja];
GO
IF OBJECT_ID(N'[dbo].[negocio]', 'U') IS NOT NULL
    DROP TABLE [dbo].[negocio];
GO
IF OBJECT_ID(N'[dbo].[Notifications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Notifications];
GO
IF OBJECT_ID(N'[dbo].[operacion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[operacion];
GO
IF OBJECT_ID(N'[dbo].[Permissions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Permissions];
GO
IF OBJECT_ID(N'[dbo].[producto]', 'U') IS NOT NULL
    DROP TABLE [dbo].[producto];
GO
IF OBJECT_ID(N'[dbo].[producto_servicio]', 'U') IS NOT NULL
    DROP TABLE [dbo].[producto_servicio];
GO
IF OBJECT_ID(N'[dbo].[punto_venta]', 'U') IS NOT NULL
    DROP TABLE [dbo].[punto_venta];
GO
IF OBJECT_ID(N'[dbo].[RegistroDiarioCaja]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegistroDiarioCaja];
GO
IF OBJECT_ID(N'[dbo].[RolePermissions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RolePermissions];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[SubMayor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SubMayor];
GO
IF OBJECT_ID(N'[dbo].[tipo_cliente]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tipo_cliente];
GO
IF OBJECT_ID(N'[dbo].[tipo_factura]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tipo_factura];
GO
IF OBJECT_ID(N'[dbo].[tipo_operacion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tipo_operacion];
GO
IF OBJECT_ID(N'[dbo].[tipo_pago]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tipo_pago];
GO
IF OBJECT_ID(N'[dbo].[trazas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[trazas];
GO
IF OBJECT_ID(N'[dbo].[unidad]', 'U') IS NOT NULL
    DROP TABLE [dbo].[unidad];
GO
IF OBJECT_ID(N'[dbo].[UserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRoles];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AlertasSistema'
CREATE TABLE [dbo].[AlertasSistema] (
    [AlertaID] int IDENTITY(1,1) NOT NULL,
    [TipoAlerta] nvarchar(50)  NULL,
    [Descripcion] nvarchar(255)  NULL,
    [FechaAlerta] datetime  NOT NULL,
    [Usuario] nvarchar(100)  NULL
);
GO

-- Creating table 'area'
CREATE TABLE [dbo].[area] (
    [id] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(250)  NULL
);
GO

-- Creating table 'AuditoriaMovimientosCaja'
CREATE TABLE [dbo].[AuditoriaMovimientosCaja] (
    [AuditoriaID] int IDENTITY(1,1) NOT NULL,
    [MovimientoID] int  NULL,
    [SubMayorID] int  NULL,
    [TipoOperacion] nvarchar(10)  NULL,
    [TipoMovimiento] nvarchar(10)  NULL,
    [Monto] decimal(18,2)  NULL,
    [Concepto] nvarchar(255)  NULL,
    [ReferenciaExterna] nvarchar(100)  NULL,
    [MetodoPago] nvarchar(50)  NULL,
    [FechaMovimiento] datetime  NULL,
    [UsuarioOperacion] nvarchar(100)  NULL,
    [FechaOperacion] datetime  NOT NULL
);
GO

-- Creating table 'CajaPrincipal'
CREATE TABLE [dbo].[CajaPrincipal] (
    [CajaID] int IDENTITY(1,1) NOT NULL,
    [Nombre] nvarchar(100)  NOT NULL,
    [Moneda] nvarchar(10)  NOT NULL,
    [SaldoActual] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'categoria'
CREATE TABLE [dbo].[categoria] (
    [id] int IDENTITY(1,1) NOT NULL,
    [clave] varchar(10)  NULL
);
GO

-- Creating table 'CierreCaja'
CREATE TABLE [dbo].[CierreCaja] (
    [CierreID] int IDENTITY(1,1) NOT NULL,
    [CajaID] int  NOT NULL,
    [FechaCierre] datetime  NOT NULL,
    [TotalIngresos] decimal(18,2)  NOT NULL,
    [TotalEgresos] decimal(18,2)  NOT NULL,
    [SaldoFinal] decimal(18,2)  NOT NULL,
    [Usuario] nvarchar(50)  NOT NULL,
    [Observaciones] nvarchar(255)  NULL,
    [Estado] nvarchar(20)  NOT NULL,
    [FechaRegistro] datetime  NOT NULL,
    [SaldoCajaPrincipal] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'cliente'
CREATE TABLE [dbo].[cliente] (
    [id] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(255)  NULL,
    [nacionalidad] varchar(50)  NULL,
    [direccion] varchar(255)  NULL,
    [provincia] varchar(25)  NULL,
    [municipio] varchar(25)  NULL,
    [localidad] varchar(25)  NULL,
    [nit] varchar(16)  NULL,
    [reeup] varchar(25)  NULL,
    [renae] varchar(25)  NULL,
    [tipoClienteID] int  NOT NULL
);
GO

-- Creating table 'cliente_cuenta'
CREATE TABLE [dbo].[cliente_cuenta] (
    [id] int IDENTITY(1,1) NOT NULL,
    [clienteid] int  NULL,
    [cuentaid] int  NULL
);
GO

-- Creating table 'ConfiguracionHorarioCaja'
CREATE TABLE [dbo].[ConfiguracionHorarioCaja] (
    [CajaID] int  NOT NULL,
    [HoraInicio] time  NOT NULL,
    [HoraFin] time  NOT NULL
);
GO

-- Creating table 'cuenta'
CREATE TABLE [dbo].[cuenta] (
    [id] int IDENTITY(1,1) NOT NULL,
    [no] varchar(16)  NULL,
    [tipo_cuenta] varchar(25)  NULL,
    [agencia] varchar(255)  NULL,
    [banco] varchar(50)  NULL,
    [titular] varchar(255)  NULL,
    [direccion] varchar(255)  NULL
);
GO

-- Creating table 'forma_operacion'
CREATE TABLE [dbo].[forma_operacion] (
    [id] int IDENTITY(1,1) NOT NULL,
    [forma] varchar(100)  NULL
);
GO

-- Creating table 'medio_pago'
CREATE TABLE [dbo].[medio_pago] (
    [id] int IDENTITY(1,1) NOT NULL,
    [medio] varchar(50)  NULL
);
GO

-- Creating table 'MovimientoCaja'
CREATE TABLE [dbo].[MovimientoCaja] (
    [MovimientoID] int IDENTITY(1,1) NOT NULL,
    [SubMayorID] int  NOT NULL,
    [TipoMovimiento] nvarchar(10)  NULL,
    [Monto] decimal(18,2)  NOT NULL,
    [Concepto] nvarchar(255)  NULL,
    [ReferenciaExterna] nvarchar(100)  NULL,
    [MetodoPago] nvarchar(50)  NULL,
    [Fecha] datetime  NOT NULL,
    [Usuario] nvarchar(100)  NULL
);
GO

-- Creating table 'negocio'
CREATE TABLE [dbo].[negocio] (
    [id] int IDENTITY(1,1) NOT NULL,
    [fecha] datetime  NULL,
    [productoID] int  NULL,
    [clienteID] int  NULL,
    [cantidad] real  NULL,
    [operacionID] int  NULL,
    [facturaID] int  NULL,
    [medioPagoID] int  NULL,
    [factura] varchar(10)  NULL
);
GO

-- Creating table 'Notifications'
CREATE TABLE [dbo].[Notifications] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Message] nvarchar(500)  NOT NULL,
    [Role] nvarchar(50)  NOT NULL,
    [UserId] int  NULL,
    [IsRead] bit  NULL,
    [CreatedAt] datetime  NULL
);
GO

-- Creating table 'operacion'
CREATE TABLE [dbo].[operacion] (
    [id] int IDENTITY(1,1) NOT NULL,
    [punto_ventaid] int  NULL,
    [productoid] int  NULL,
    [fecha] datetime  NULL,
    [cantidad] real  NULL,
    [importe] real  NULL,
    [tipo_pagoid] int  NULL,
    [tipo_operacionid] int  NULL
);
GO

-- Creating table 'Permissions'
CREATE TABLE [dbo].[Permissions] (
    [PermissionID] int IDENTITY(1,1) NOT NULL,
    [PermissionName] varchar(75)  NULL,
    [PermissionDescription] varchar(255)  NULL
);
GO

-- Creating table 'producto'
CREATE TABLE [dbo].[producto] (
    [id] int IDENTITY(1,1) NOT NULL,
    [cod] varchar(50)  NULL,
    [nombre] varchar(250)  NULL,
    [precio] float  NULL,
    [costo] float  NULL,
    [fecha] datetime  NULL,
    [categoriaid] int  NULL,
    [unidadid] int  NULL,
    [areaid] int  NULL,
    [activo] bit  NULL
);
GO

-- Creating table 'producto_servicio'
CREATE TABLE [dbo].[producto_servicio] (
    [id] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(250)  NULL,
    [precio] real  NULL,
    [costo] real  NULL
);
GO

-- Creating table 'punto_venta'
CREATE TABLE [dbo].[punto_venta] (
    [id] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(50)  NULL
);
GO

-- Creating table 'RegistroDiarioCaja'
CREATE TABLE [dbo].[RegistroDiarioCaja] (
    [RegistroID] int IDENTITY(1,1) NOT NULL,
    [CajaID] int  NOT NULL,
    [Fecha] datetime  NOT NULL,
    [TotalIngresos] decimal(18,2)  NOT NULL,
    [TotalEgresos] decimal(18,2)  NOT NULL,
    [SaldoFinal] decimal(18,2)  NOT NULL,
    [UsuarioCierre] nvarchar(100)  NULL,
    [Observaciones] nvarchar(255)  NULL
);
GO

-- Creating table 'RolePermissions'
CREATE TABLE [dbo].[RolePermissions] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [RoleID] int  NULL,
    [PermissionID] int  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [RoleID] int IDENTITY(1,1) NOT NULL,
    [RoleName] varchar(50)  NULL,
    [IsSysAdmin] bit  NULL,
    [RoleDescription] varchar(255)  NULL
);
GO

-- Creating table 'SubMayor'
CREATE TABLE [dbo].[SubMayor] (
    [SubMayorID] int IDENTITY(1,1) NOT NULL,
    [CajaID] int  NOT NULL,
    [Nombre] nvarchar(100)  NOT NULL,
    [Saldo] decimal(18,2)  NOT NULL,
    [Activo] bit  NULL
);
GO

-- Creating table 'tipo_cliente'
CREATE TABLE [dbo].[tipo_cliente] (
    [id] int IDENTITY(1,1) NOT NULL,
    [tipo] varchar(50)  NULL
);
GO

-- Creating table 'tipo_factura'
CREATE TABLE [dbo].[tipo_factura] (
    [id] int IDENTITY(1,1) NOT NULL,
    [tipo] varchar(50)  NULL
);
GO

-- Creating table 'tipo_operacion'
CREATE TABLE [dbo].[tipo_operacion] (
    [id] int IDENTITY(1,1) NOT NULL,
    [tipo] varchar(50)  NULL
);
GO

-- Creating table 'tipo_pago'
CREATE TABLE [dbo].[tipo_pago] (
    [id] int IDENTITY(1,1) NOT NULL,
    [tipo] varchar(50)  NULL
);
GO

-- Creating table 'trazas'
CREATE TABLE [dbo].[trazas] (
    [id] int IDENTITY(1,1) NOT NULL,
    [fecha] datetime  NULL,
    [usuario] varchar(100)  NULL,
    [accion] varchar(255)  NULL,
    [descripcion] varchar(max)  NULL,
    [ip] varchar(45)  NULL,
    [modulo] varchar(100)  NULL
);
GO

-- Creating table 'unidad'
CREATE TABLE [dbo].[unidad] (
    [id] int IDENTITY(1,1) NOT NULL,
    [unidad1] varchar(50)  NULL
);
GO

-- Creating table 'UserRoles'
CREATE TABLE [dbo].[UserRoles] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [UserID] int  NOT NULL,
    [RoleID] int  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [UserID] int IDENTITY(1,1) NOT NULL,
    [UserName] varchar(250)  NULL,
    [Password] varchar(255)  NULL,
    [Email] varchar(255)  NULL,
    [IsApproved] bit  NULL,
    [CreationDate] datetime  NULL,
    [LastModified] datetime  NULL,
    [Inactive] bit  NULL
);
GO

-- Creating table 'CierreSubMayor'
CREATE TABLE [dbo].[CierreSubMayor] (
    [CierreID] int  NULL,
    [SubMayorID] int  NULL,
    [SaldoFinal] decimal(18,2)  NULL,
    [FechaRegistro] datetime  NULL,
    [ID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [AlertaID] in table 'AlertasSistema'
ALTER TABLE [dbo].[AlertasSistema]
ADD CONSTRAINT [PK_AlertasSistema]
    PRIMARY KEY CLUSTERED ([AlertaID] ASC);
GO

-- Creating primary key on [id] in table 'area'
ALTER TABLE [dbo].[area]
ADD CONSTRAINT [PK_area]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [AuditoriaID] in table 'AuditoriaMovimientosCaja'
ALTER TABLE [dbo].[AuditoriaMovimientosCaja]
ADD CONSTRAINT [PK_AuditoriaMovimientosCaja]
    PRIMARY KEY CLUSTERED ([AuditoriaID] ASC);
GO

-- Creating primary key on [CajaID] in table 'CajaPrincipal'
ALTER TABLE [dbo].[CajaPrincipal]
ADD CONSTRAINT [PK_CajaPrincipal]
    PRIMARY KEY CLUSTERED ([CajaID] ASC);
GO

-- Creating primary key on [id] in table 'categoria'
ALTER TABLE [dbo].[categoria]
ADD CONSTRAINT [PK_categoria]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [CierreID] in table 'CierreCaja'
ALTER TABLE [dbo].[CierreCaja]
ADD CONSTRAINT [PK_CierreCaja]
    PRIMARY KEY CLUSTERED ([CierreID] ASC);
GO

-- Creating primary key on [id] in table 'cliente'
ALTER TABLE [dbo].[cliente]
ADD CONSTRAINT [PK_cliente]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'cliente_cuenta'
ALTER TABLE [dbo].[cliente_cuenta]
ADD CONSTRAINT [PK_cliente_cuenta]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [CajaID] in table 'ConfiguracionHorarioCaja'
ALTER TABLE [dbo].[ConfiguracionHorarioCaja]
ADD CONSTRAINT [PK_ConfiguracionHorarioCaja]
    PRIMARY KEY CLUSTERED ([CajaID] ASC);
GO

-- Creating primary key on [id] in table 'cuenta'
ALTER TABLE [dbo].[cuenta]
ADD CONSTRAINT [PK_cuenta]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'forma_operacion'
ALTER TABLE [dbo].[forma_operacion]
ADD CONSTRAINT [PK_forma_operacion]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'medio_pago'
ALTER TABLE [dbo].[medio_pago]
ADD CONSTRAINT [PK_medio_pago]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [MovimientoID] in table 'MovimientoCaja'
ALTER TABLE [dbo].[MovimientoCaja]
ADD CONSTRAINT [PK_MovimientoCaja]
    PRIMARY KEY CLUSTERED ([MovimientoID] ASC);
GO

-- Creating primary key on [id] in table 'negocio'
ALTER TABLE [dbo].[negocio]
ADD CONSTRAINT [PK_negocio]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id], [Message], [Role] in table 'Notifications'
ALTER TABLE [dbo].[Notifications]
ADD CONSTRAINT [PK_Notifications]
    PRIMARY KEY CLUSTERED ([Id], [Message], [Role] ASC);
GO

-- Creating primary key on [id] in table 'operacion'
ALTER TABLE [dbo].[operacion]
ADD CONSTRAINT [PK_operacion]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [PermissionID] in table 'Permissions'
ALTER TABLE [dbo].[Permissions]
ADD CONSTRAINT [PK_Permissions]
    PRIMARY KEY CLUSTERED ([PermissionID] ASC);
GO

-- Creating primary key on [id] in table 'producto'
ALTER TABLE [dbo].[producto]
ADD CONSTRAINT [PK_producto]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'producto_servicio'
ALTER TABLE [dbo].[producto_servicio]
ADD CONSTRAINT [PK_producto_servicio]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'punto_venta'
ALTER TABLE [dbo].[punto_venta]
ADD CONSTRAINT [PK_punto_venta]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [RegistroID] in table 'RegistroDiarioCaja'
ALTER TABLE [dbo].[RegistroDiarioCaja]
ADD CONSTRAINT [PK_RegistroDiarioCaja]
    PRIMARY KEY CLUSTERED ([RegistroID] ASC);
GO

-- Creating primary key on [ID] in table 'RolePermissions'
ALTER TABLE [dbo].[RolePermissions]
ADD CONSTRAINT [PK_RolePermissions]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [RoleID] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([RoleID] ASC);
GO

-- Creating primary key on [SubMayorID] in table 'SubMayor'
ALTER TABLE [dbo].[SubMayor]
ADD CONSTRAINT [PK_SubMayor]
    PRIMARY KEY CLUSTERED ([SubMayorID] ASC);
GO

-- Creating primary key on [id] in table 'tipo_cliente'
ALTER TABLE [dbo].[tipo_cliente]
ADD CONSTRAINT [PK_tipo_cliente]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'tipo_factura'
ALTER TABLE [dbo].[tipo_factura]
ADD CONSTRAINT [PK_tipo_factura]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'tipo_operacion'
ALTER TABLE [dbo].[tipo_operacion]
ADD CONSTRAINT [PK_tipo_operacion]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'tipo_pago'
ALTER TABLE [dbo].[tipo_pago]
ADD CONSTRAINT [PK_tipo_pago]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'trazas'
ALTER TABLE [dbo].[trazas]
ADD CONSTRAINT [PK_trazas]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'unidad'
ALTER TABLE [dbo].[unidad]
ADD CONSTRAINT [PK_unidad]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [ID] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [PK_UserRoles]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [UserID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([UserID] ASC);
GO

-- Creating primary key on [ID] in table 'CierreSubMayor'
ALTER TABLE [dbo].[CierreSubMayor]
ADD CONSTRAINT [PK_CierreSubMayor]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [areaid] in table 'producto'
ALTER TABLE [dbo].[producto]
ADD CONSTRAINT [FK_FKproducto660940]
    FOREIGN KEY ([areaid])
    REFERENCES [dbo].[area]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKproducto660940'
CREATE INDEX [IX_FK_FKproducto660940]
ON [dbo].[producto]
    ([areaid]);
GO

-- Creating foreign key on [CajaID] in table 'RegistroDiarioCaja'
ALTER TABLE [dbo].[RegistroDiarioCaja]
ADD CONSTRAINT [FK__RegistroD__CajaI__32767D0B]
    FOREIGN KEY ([CajaID])
    REFERENCES [dbo].[CajaPrincipal]
        ([CajaID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__RegistroD__CajaI__32767D0B'
CREATE INDEX [IX_FK__RegistroD__CajaI__32767D0B]
ON [dbo].[RegistroDiarioCaja]
    ([CajaID]);
GO

-- Creating foreign key on [CajaID] in table 'SubMayor'
ALTER TABLE [dbo].[SubMayor]
ADD CONSTRAINT [FK__SubMayor__CajaID__2704CA5F]
    FOREIGN KEY ([CajaID])
    REFERENCES [dbo].[CajaPrincipal]
        ([CajaID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__SubMayor__CajaID__2704CA5F'
CREATE INDEX [IX_FK__SubMayor__CajaID__2704CA5F]
ON [dbo].[SubMayor]
    ([CajaID]);
GO

-- Creating foreign key on [CajaID] in table 'CierreCaja'
ALTER TABLE [dbo].[CierreCaja]
ADD CONSTRAINT [FK_CierreCaja_CajaPrincipal]
    FOREIGN KEY ([CajaID])
    REFERENCES [dbo].[CajaPrincipal]
        ([CajaID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CierreCaja_CajaPrincipal'
CREATE INDEX [IX_FK_CierreCaja_CajaPrincipal]
ON [dbo].[CierreCaja]
    ([CajaID]);
GO

-- Creating foreign key on [categoriaid] in table 'producto'
ALTER TABLE [dbo].[producto]
ADD CONSTRAINT [FK_FKproducto393903]
    FOREIGN KEY ([categoriaid])
    REFERENCES [dbo].[categoria]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKproducto393903'
CREATE INDEX [IX_FK_FKproducto393903]
ON [dbo].[producto]
    ([categoriaid]);
GO

-- Creating foreign key on [clienteid] in table 'cliente_cuenta'
ALTER TABLE [dbo].[cliente_cuenta]
ADD CONSTRAINT [FK_FKcliente_cu555547]
    FOREIGN KEY ([clienteid])
    REFERENCES [dbo].[cliente]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKcliente_cu555547'
CREATE INDEX [IX_FK_FKcliente_cu555547]
ON [dbo].[cliente_cuenta]
    ([clienteid]);
GO

-- Creating foreign key on [tipoClienteID] in table 'cliente'
ALTER TABLE [dbo].[cliente]
ADD CONSTRAINT [FK_FKcliente423992]
    FOREIGN KEY ([tipoClienteID])
    REFERENCES [dbo].[tipo_cliente]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKcliente423992'
CREATE INDEX [IX_FK_FKcliente423992]
ON [dbo].[cliente]
    ([tipoClienteID]);
GO

-- Creating foreign key on [clienteID] in table 'negocio'
ALTER TABLE [dbo].[negocio]
ADD CONSTRAINT [FK_FKnegocio736135]
    FOREIGN KEY ([clienteID])
    REFERENCES [dbo].[cliente]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKnegocio736135'
CREATE INDEX [IX_FK_FKnegocio736135]
ON [dbo].[negocio]
    ([clienteID]);
GO

-- Creating foreign key on [cuentaid] in table 'cliente_cuenta'
ALTER TABLE [dbo].[cliente_cuenta]
ADD CONSTRAINT [FK_FKcliente_cu504141]
    FOREIGN KEY ([cuentaid])
    REFERENCES [dbo].[cuenta]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKcliente_cu504141'
CREATE INDEX [IX_FK_FKcliente_cu504141]
ON [dbo].[cliente_cuenta]
    ([cuentaid]);
GO

-- Creating foreign key on [operacionID] in table 'negocio'
ALTER TABLE [dbo].[negocio]
ADD CONSTRAINT [FK_FKnegocio873465]
    FOREIGN KEY ([operacionID])
    REFERENCES [dbo].[forma_operacion]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKnegocio873465'
CREATE INDEX [IX_FK_FKnegocio873465]
ON [dbo].[negocio]
    ([operacionID]);
GO

-- Creating foreign key on [medioPagoID] in table 'negocio'
ALTER TABLE [dbo].[negocio]
ADD CONSTRAINT [FK_FKnegocio968398]
    FOREIGN KEY ([medioPagoID])
    REFERENCES [dbo].[medio_pago]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKnegocio968398'
CREATE INDEX [IX_FK_FKnegocio968398]
ON [dbo].[negocio]
    ([medioPagoID]);
GO

-- Creating foreign key on [SubMayorID] in table 'MovimientoCaja'
ALTER TABLE [dbo].[MovimientoCaja]
ADD CONSTRAINT [FK__Movimient__SubMa__2CBDA3B5]
    FOREIGN KEY ([SubMayorID])
    REFERENCES [dbo].[SubMayor]
        ([SubMayorID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Movimient__SubMa__2CBDA3B5'
CREATE INDEX [IX_FK__Movimient__SubMa__2CBDA3B5]
ON [dbo].[MovimientoCaja]
    ([SubMayorID]);
GO

-- Creating foreign key on [facturaID] in table 'negocio'
ALTER TABLE [dbo].[negocio]
ADD CONSTRAINT [FK_FKnegocio542366]
    FOREIGN KEY ([facturaID])
    REFERENCES [dbo].[tipo_factura]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKnegocio542366'
CREATE INDEX [IX_FK_FKnegocio542366]
ON [dbo].[negocio]
    ([facturaID]);
GO

-- Creating foreign key on [productoID] in table 'negocio'
ALTER TABLE [dbo].[negocio]
ADD CONSTRAINT [FK_FKnegocio549625]
    FOREIGN KEY ([productoID])
    REFERENCES [dbo].[producto_servicio]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKnegocio549625'
CREATE INDEX [IX_FK_FKnegocio549625]
ON [dbo].[negocio]
    ([productoID]);
GO

-- Creating foreign key on [punto_ventaid] in table 'operacion'
ALTER TABLE [dbo].[operacion]
ADD CONSTRAINT [FK_FKoperacion168701]
    FOREIGN KEY ([punto_ventaid])
    REFERENCES [dbo].[punto_venta]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKoperacion168701'
CREATE INDEX [IX_FK_FKoperacion168701]
ON [dbo].[operacion]
    ([punto_ventaid]);
GO

-- Creating foreign key on [productoid] in table 'operacion'
ALTER TABLE [dbo].[operacion]
ADD CONSTRAINT [FK_FKoperacion21]
    FOREIGN KEY ([productoid])
    REFERENCES [dbo].[producto]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKoperacion21'
CREATE INDEX [IX_FK_FKoperacion21]
ON [dbo].[operacion]
    ([productoid]);
GO

-- Creating foreign key on [tipo_pagoid] in table 'operacion'
ALTER TABLE [dbo].[operacion]
ADD CONSTRAINT [FK_FKoperacion273527]
    FOREIGN KEY ([tipo_pagoid])
    REFERENCES [dbo].[tipo_pago]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKoperacion273527'
CREATE INDEX [IX_FK_FKoperacion273527]
ON [dbo].[operacion]
    ([tipo_pagoid]);
GO

-- Creating foreign key on [tipo_operacionid] in table 'operacion'
ALTER TABLE [dbo].[operacion]
ADD CONSTRAINT [FK_FKoperacion325014]
    FOREIGN KEY ([tipo_operacionid])
    REFERENCES [dbo].[tipo_operacion]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKoperacion325014'
CREATE INDEX [IX_FK_FKoperacion325014]
ON [dbo].[operacion]
    ([tipo_operacionid]);
GO

-- Creating foreign key on [PermissionID] in table 'RolePermissions'
ALTER TABLE [dbo].[RolePermissions]
ADD CONSTRAINT [FK_FKRolePermis471885]
    FOREIGN KEY ([PermissionID])
    REFERENCES [dbo].[Permissions]
        ([PermissionID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKRolePermis471885'
CREATE INDEX [IX_FK_FKRolePermis471885]
ON [dbo].[RolePermissions]
    ([PermissionID]);
GO

-- Creating foreign key on [unidadid] in table 'producto'
ALTER TABLE [dbo].[producto]
ADD CONSTRAINT [FK_FKproducto607651]
    FOREIGN KEY ([unidadid])
    REFERENCES [dbo].[unidad]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKproducto607651'
CREATE INDEX [IX_FK_FKproducto607651]
ON [dbo].[producto]
    ([unidadid]);
GO

-- Creating foreign key on [RoleID] in table 'RolePermissions'
ALTER TABLE [dbo].[RolePermissions]
ADD CONSTRAINT [FK_FKRolePermis48370]
    FOREIGN KEY ([RoleID])
    REFERENCES [dbo].[Roles]
        ([RoleID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKRolePermis48370'
CREATE INDEX [IX_FK_FKRolePermis48370]
ON [dbo].[RolePermissions]
    ([RoleID]);
GO

-- Creating foreign key on [RoleID] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [FK_FKUserRoles613991]
    FOREIGN KEY ([RoleID])
    REFERENCES [dbo].[Roles]
        ([RoleID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKUserRoles613991'
CREATE INDEX [IX_FK_FKUserRoles613991]
ON [dbo].[UserRoles]
    ([RoleID]);
GO

-- Creating foreign key on [UserID] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [FK_FKUserRoles959420]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[Users]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FKUserRoles959420'
CREATE INDEX [IX_FK_FKUserRoles959420]
ON [dbo].[UserRoles]
    ([UserID]);
GO

-- Creating foreign key on [CierreID] in table 'CierreSubMayor'
ALTER TABLE [dbo].[CierreSubMayor]
ADD CONSTRAINT [FK__CierreSub__Cierr__5C6CB6D7]
    FOREIGN KEY ([CierreID])
    REFERENCES [dbo].[CierreCaja]
        ([CierreID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__CierreSub__Cierr__5C6CB6D7'
CREATE INDEX [IX_FK__CierreSub__Cierr__5C6CB6D7]
ON [dbo].[CierreSubMayor]
    ([CierreID]);
GO

-- Creating foreign key on [SubMayorID] in table 'CierreSubMayor'
ALTER TABLE [dbo].[CierreSubMayor]
ADD CONSTRAINT [FK__CierreSub__SubMa__5D60DB10]
    FOREIGN KEY ([SubMayorID])
    REFERENCES [dbo].[SubMayor]
        ([SubMayorID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__CierreSub__SubMa__5D60DB10'
CREATE INDEX [IX_FK__CierreSub__SubMa__5D60DB10]
ON [dbo].[CierreSubMayor]
    ([SubMayorID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------