select * from [dbo].[CAT_APP]
select * from [dbo].[CAT_ESTATUS_USR]
SELECT * FROM [dbo].[CAT_ROL]
select * from [dbo].[GRUPO]
select * from [dbo].[GRUPO_OPCION_ACCION]
select * from [dbo].[GRUPO_OPCION_SECCION]
SELECT * FROM [dbo].[GRUPO_PERMISO]
SELECT * FROM [dbo].[GRUPO_ROL]
SELECT * FROM [dbo].[GRUPO_URL_POST_APP]
select * from [dbo].[GRUPO_USUARIO]
select * from [dbo].[MENSAJE_APP]
select * from [dbo].[MODULO_APP]
select * from [dbo].[OPCION_ACCION]
select * from [dbo].[OPCION_MODULO]
select * from [dbo].[OPCION_SECCION]
select * from [dbo].[ROL_OPCION]
select * from [dbo].[URL_POST_APP]
select * from [dbo].[USUARIO]




--DBCC CHECKIDENT (USUARIO, RESEED, 0)

--INSERT INTO  CAT_APP (nombre_app,logo,url_home,activo,UsuAlta,FechaAlta,UsuCambio,FechaCambio)
--values('Sistema Nomina','LogoNomina.jpg','https://localhost:44347/',1,'admin@gmail.com',GETDATE(),'admin@gmail.com',GETDATE())
--INSERT INTO CAT_ESTATUS_USR (id,estatus,activo,UsuAlta,FechaAlta,UsuCambio,FechaCambio)
--VALUES (1,'Activado',1,'admin@gmail.com',GETDATE(),'admin@gmail.com',GETDATE())
--INSERT INTO CAT_ROL (rol,descripcion,activo,id_app,UsuAlta,FechaAlta,UsuCambio,FechaCambio)
--values('Admin','Permisos de administrador',1,1,'admin@gmail.com',GETDATE(),'admin@gmail.com',GETDATE())
--INSERT INTO MODULO_APP (id_padre,nombre_modulo,url_icono,url_destino,id_app,activo,[version],UsuAlta,FechaAlta,UsuCambio,FechaCambio)
--values(1,'Usuarios','menu.ico','https://localhost:44347/Home',1,1,'1.1','admin@gmail.com',GETDATE(),'admin@gmail.com',GETDATE())
--INSERT INTO OPCION_MODULO(descripcion_item,url_destino,url_icono,id_item_padre,id_modulo,activo,orden,version,UsuAlta,FechaAlta,UsuCambio,FechaCambio)
--values('Modulo para la administración de usuarios','https://localhost:44347/Usuarios/index','user.icon',
--1,1,1,1,'1.1','admin@gmail.com',GETDATE(),'admin@gmail.com',GETDATE())
--INSERT INTO ROL_OPCION (id_rol,id_item_modulo,UsuAlta,FechaAlta,UsuCambio,FechaCambio)
--VALUES(1,1,'admin@gmail.com',GETDATE(),'admin@gmail.com',GETDATE())
--INSERT INTO GRUPO (id_padre,grupo,activo,UsuAlta,FechaAlta,UsuCambio,FechaCambio)
--VALUES(1,'Nomina',1,'admin@gmail.com',GETDATE(),'admin@gmail.com',GETDATE())

--INSERT INTO GRUPO_USUARIO(id_grupo,id_usuario,otorgar,denegar,UsuAlta,FechaAlta,UsuCambio,FechaCambio)
--values(1,1,1,0,'admin@gmail.com',GETDATE(),'admin@gmail.com',GETDATE())

--insert into USUARIO (usuario,llave_usuario,id_estatus_usr,fecha_vencimiento_llave,id_persona,fecha_registro
--,UsuAlta,FechaAlta,UsuCambio,FechaCambio)
--values('admin@gmail.com','12345',1,'2022-12-31',1,GETDATE(),'admin@gmail.com',GETDATE(),'admin@gmail.com',GETDATE())
insert into GRUPO_ROL (id_rol,id_grupo,UsuAlta,FechaAlta,UsuCambio,FechaCambio)
values(2,1,'admin@gmail.com',GETDATE(),'admin@gmail.com',GETDATE())