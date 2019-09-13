-- Script Date: 5/9/2019 10:36  - ErikEJ.SqlCeScripting version 3.5.2.81
-- Database information:
-- Locale Identifier: 11274
-- Encryption Mode: 
-- Case Sensitive: False
-- Database: C:\Users\fyanicelli\source\repos\Unir.TFG.MeterMAX\Unir.TFG.MeterMAX.MDM\Databases\db-template - copia.sdf
-- ServerVersion: 4.0.8876.1
-- DatabaseSize: 512 KB
-- SpaceAvailable: 3.999 GB
-- Created: 5/9/2019 10:25

-- User Table information:
-- Number of tables: 35
-- Addresses: 0 row(s)
-- Cities: 0 row(s)
-- CommunicationChannelSettings: 0 row(s)
-- Countries: 0 row(s)
-- Customers: 0 row(s)
-- DataSetComponents: 0 row(s)
-- DataSets: 0 row(s)
-- EnergySupplies: 0 row(s)
-- EnergySupplyGroups: 0 row(s)
-- GeoCoordinates: 0 row(s)
-- InsituMeterSessionSettings: 0 row(s)
-- MeterManufacturers: 0 row(s)
-- MeterProtocolSettings: 0 row(s)
-- Meters: 0 row(s)
-- MeterSession: 0 row(s)
-- MeterSessionItemTasks: 0 row(s)
-- MeterSessionReconnectionSchemas: 0 row(s)
-- MeterSessionSettings: 0 row(s)
-- MeterSessionStatuses: 0 row(s)
-- MeterSessionTasks: 0 row(s)
-- MeterSessionTypes: 0 row(s)
-- OnDemandMeterSessions: 0 row(s)
-- PhoneCompanies: 0 row(s)
-- PostalCodes: 0 row(s)
-- ReconnectionSchedules: 0 row(s)
-- RemoteDevices: 0 row(s)
-- RemoteMeterSessionSettings: 0 row(s)
-- Roles: 1 row(s)
-- SIMCards: 0 row(s)
-- SIMCardServices: 0 row(s)
-- States: 0 row(s)
-- Streets: 0 row(s)
-- Users: 1 row(s)
-- UserSessions: 0 row(s)
-- Vendors: 0 row(s)

SELECT 1;
PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE [Streets] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(255) NOT NULL COLLATE NOCASE
, [Number] int NOT NULL
, [Floor] nvarchar(100) NULL COLLATE NOCASE
);
CREATE TABLE [SIMCardServices] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(100) NOT NULL COLLATE NOCASE
, [Description] nvarchar(255) NULL COLLATE NOCASE
);
CREATE TABLE [Roles] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(100) NOT NULL COLLATE NOCASE
, [Description] nvarchar(255) NULL COLLATE NOCASE
);
CREATE TABLE [Users] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Identification] nvarchar(50) NOT NULL COLLATE NOCASE
, [UserName] nvarchar(50) NOT NULL COLLATE NOCASE
, [Password] nvarchar(20) NOT NULL COLLATE NOCASE
, [RoleId] int NOT NULL
, CONSTRAINT [Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
);
CREATE TABLE [UserSessions] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [StartDate] datetime NOT NULL
, [EndDate] datetime NULL
, [UserId] int NOT NULL
, CONSTRAINT [UserSessions_Users] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [MeterSessionTypes] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(100) NOT NULL COLLATE NOCASE
, [Description] nvarchar(255) NULL COLLATE NOCASE
);
CREATE TABLE [MeterSessionStatuses] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(100) NOT NULL COLLATE NOCASE
, [Description] nvarchar(255) NULL COLLATE NOCASE
);
CREATE TABLE [MeterSessionReconnectionSchemas] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [MaxReconnectionAttempts] int NOT NULL
);
CREATE TABLE [ReconnectionSchedules] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Schedule] bigint NOT NULL
, [MeterSessionReconnectionSchemaId] int NOT NULL
, CONSTRAINT [ReconnectionSchedules_MeterSessionReconnectionSchemas] FOREIGN KEY ([MeterSessionReconnectionSchemaId]) REFERENCES [MeterSessionReconnectionSchemas] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [MeterProtocolSettings] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [UserId] int NOT NULL
, [UserName] nvarchar(100) NOT NULL COLLATE NOCASE
, [Password] nvarchar(100) NOT NULL COLLATE NOCASE
);
CREATE TABLE [GeoCoordinates] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Altitude] float NULL
, [Latitude] float NOT NULL
, [Longitude] float NOT NULL
);
CREATE TABLE [EnergySupplyGroups] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(255) NOT NULL COLLATE NOCASE
, [Description] nvarchar(255) NULL COLLATE NOCASE
);
CREATE TABLE [DataSets] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(100) NOT NULL COLLATE NOCASE
, [Description] nvarchar(255) NULL COLLATE NOCASE
);
CREATE TABLE [DataSetComponents] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(100) NOT NULL COLLATE NOCASE
, [Description] nvarchar(255) NULL COLLATE NOCASE
);
CREATE TABLE [DataSets_DataSetComponents] (
  [DataSetId] int NOT NULL
, [DataSetComponentId] int NOT NULL
, CONSTRAINT [DataSets_DataSetComponents_DataSets] FOREIGN KEY ([DataSetId]) REFERENCES [DataSets] ([Id]) ON DELETE SET NULL ON UPDATE SET NULL
, CONSTRAINT [DataSets_DataSetComponents_DataSetComponents] FOREIGN KEY ([DataSetComponentId]) REFERENCES [DataSetComponents] ([Id]) ON DELETE SET NULL ON UPDATE SET NULL
);
CREATE TABLE [MeterSessionTasks] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(100) NOT NULL COLLATE NOCASE
, [StartDate] datetime NOT NULL
, [EndDate] datetime NOT NULL
, [Error] nvarchar(255) NULL COLLATE NOCASE
, [Completed] bit NULL
, [DataSetComponentId] int NOT NULL
, CONSTRAINT [MeterSessionTasks_DataSetComponents] FOREIGN KEY ([DataSetComponentId]) REFERENCES [DataSetComponents] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
);
CREATE TABLE [MeterSessionItemTasks] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(255) NOT NULL COLLATE NOCASE
, [StartDate] datetime NOT NULL
, [EndDate] datetime NOT NULL
, [Error] nvarchar(255) NULL COLLATE NOCASE
, [Success] bit NULL
, [TaskResult] image NULL
, [SessionTaskId] int NOT NULL
, CONSTRAINT [MeterSessionItemTasks_MeterSessionTasks] FOREIGN KEY ([SessionTaskId]) REFERENCES [MeterSessionTasks] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [Customers] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [IdentificationNumber] nvarchar(100) NOT NULL COLLATE NOCASE
, [Name] nvarchar(255) NOT NULL COLLATE NOCASE
);
CREATE TABLE [Countries] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(255) NOT NULL COLLATE NOCASE
);
CREATE TABLE [States] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(255) NOT NULL COLLATE NOCASE
, [CountryId] int NOT NULL
, CONSTRAINT [States_Countries] FOREIGN KEY ([CountryId]) REFERENCES [Countries] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [CommunicationChannelSettings] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [SendAckResponseThershold] int NULL
, [BaudRate] int NOT NULL
, [PacketSize] tinyint NOT NULL
, [NumberOfPackets] tinyint NOT NULL
, [ChannelTrafficTimeout] tinyint NOT NULL
, [InterCharacterTimeout] tinyint NOT NULL
, [ResponseTimeout] tinyint NOT NULL
, [NumberOfRetries] tinyint NOT NULL
);
CREATE TABLE [MeterSessionSettings] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(100) NOT NULL COLLATE NOCASE
, [InternalReconnectionAttempts] int NULL
, [ProtocolSettingId] int NOT NULL
, [ReconnectionSchemaId] int NULL
, [CommunicationChannelSettingId] int NULL
, CONSTRAINT [MeterSessionSettings_MeterProtocolSettings] FOREIGN KEY ([ProtocolSettingId]) REFERENCES [MeterProtocolSettings] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
, CONSTRAINT [MeterSessionSettings_MeterSessionReconnectionSchemas] FOREIGN KEY ([ReconnectionSchemaId]) REFERENCES [MeterSessionReconnectionSchemas] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [MeterSessionSettings_CommunicationChannelSettings] FOREIGN KEY ([CommunicationChannelSettingId]) REFERENCES [CommunicationChannelSettings] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
);
CREATE TABLE [RemoteMeterSessionSettings] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [UseMeterMAXProtocol] bit NOT NULL
, [MeterSessionSettingId] int NOT NULL
, CONSTRAINT [RemoteMeterSessionSettings_MeterSessionSettings] FOREIGN KEY ([MeterSessionSettingId]) REFERENCES [MeterSessionSettings] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [InsituMeterSessionSettings] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [PortName] nvarchar(20) NOT NULL COLLATE NOCASE
, [DtrEnabled] bit NULL
, [MeterSessionSettingId] int NOT NULL
, CONSTRAINT [InsituMeterSessionSettings_MeterSessionSettings] FOREIGN KEY ([MeterSessionSettingId]) REFERENCES [MeterSessionSettings] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [Cities] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(255) NOT NULL COLLATE NOCASE
, [StateId] int NOT NULL
, CONSTRAINT [Cities_States] FOREIGN KEY ([StateId]) REFERENCES [States] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [PostalCodes] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Code] nvarchar(100) NOT NULL COLLATE NOCASE
, [CityId] int NOT NULL
, CONSTRAINT [PostalCodes_Cities] FOREIGN KEY ([CityId]) REFERENCES [Cities] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [Addresses] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [StreetId] int NOT NULL
, [PostalCodeId] int NOT NULL
, CONSTRAINT [Addresses_PostalCodes] FOREIGN KEY ([PostalCodeId]) REFERENCES [PostalCodes] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
, CONSTRAINT [Addresses_Streets] FOREIGN KEY ([PostalCodeId]) REFERENCES [Streets] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [Vendors] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(255) NOT NULL COLLATE NOCASE
, [Description] nvarchar(255) NULL COLLATE NOCASE
, [AddressId] int NULL
, CONSTRAINT [Vendors_Addresses] FOREIGN KEY ([AddressId]) REFERENCES [Addresses] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
);
CREATE TABLE [PhoneCompanies] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(255) NOT NULL COLLATE NOCASE
, [AddressId] int NULL
, CONSTRAINT [PhoneCompanies_Addresses] FOREIGN KEY ([AddressId]) REFERENCES [Addresses] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
);
CREATE TABLE [SIMCards] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [SerialNumber] nvarchar(255) NOT NULL COLLATE NOCASE
, [PhoneNumber] nvarchar(100) NOT NULL COLLATE NOCASE
, [PhoneCompanyId] int NOT NULL
, [ServiceId] int NOT NULL
, CONSTRAINT [SIMCards_PhoneCompanies] FOREIGN KEY ([PhoneCompanyId]) REFERENCES [PhoneCompanies] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
, CONSTRAINT [SIMCards_SIMCardServices] FOREIGN KEY ([ServiceId]) REFERENCES [SIMCardServices] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [RemoteDevices] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Model] nvarchar(255) NOT NULL COLLATE NOCASE
, [SerialNumber] nvarchar(255) NOT NULL COLLATE NOCASE
, [Ip] nvarchar(100) NOT NULL COLLATE NOCASE
, [PortNumber] int NOT NULL
, [VendorId] int NULL
, [SIMCardId] int NOT NULL
, CONSTRAINT [RemoteDevices_SIMCards] FOREIGN KEY ([SIMCardId]) REFERENCES [SIMCards] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [RemoteDevices_Vendors] FOREIGN KEY ([VendorId]) REFERENCES [Vendors] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
CREATE TABLE [MeterManufacturers] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] nvarchar(255) NOT NULL COLLATE NOCASE
, [AddressId] int NULL
, CONSTRAINT [MeterManufacturers_Addresses] FOREIGN KEY ([AddressId]) REFERENCES [Addresses] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
);
CREATE TABLE [EnergySupplies] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Code] nvarchar(100) NOT NULL COLLATE NOCASE
, [AddressId] int NOT NULL
, [CustomerId] int NOT NULL
, [GeoCoordinateId] int NULL
, [GroupId] int NULL
, CONSTRAINT [EnergySupplies_Addresses] FOREIGN KEY ([AddressId]) REFERENCES [Addresses] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [EnergySupplies_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [EnergySupplies_EnergySupplyGroups] FOREIGN KEY ([GroupId]) REFERENCES [EnergySupplyGroups] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [EnergySupplies_GeoCoordinates] FOREIGN KEY ([GeoCoordinateId]) REFERENCES [GeoCoordinates] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
);
CREATE TABLE [Meters] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Model] nvarchar(150) NOT NULL COLLATE NOCASE
, [SerialNumber] bigint NOT NULL
, [EnergySupplyId] int NOT NULL
, [MeterManufacturerId] int NOT NULL
, [RemoteDeviceId] int NULL
, CONSTRAINT [Meters_EnergySupplies] FOREIGN KEY ([EnergySupplyId]) REFERENCES [EnergySupplies] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
, CONSTRAINT [Meters_MeterManufacturers] FOREIGN KEY ([MeterManufacturerId]) REFERENCES [MeterManufacturers] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [Meters_RemoteDevices] FOREIGN KEY ([RemoteDeviceId]) REFERENCES [RemoteDevices] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
);
CREATE TABLE [MeterSession] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Guid] nvarchar(100) NOT NULL COLLATE NOCASE
, [StartDate] datetime NOT NULL
, [EndDate] datetime NOT NULL
, [SessionTrace] ntext NULL
, [SessionStatusId] int NOT NULL
, [SessionSettingId] int NOT NULL
, [MeterId] int NOT NULL
, CONSTRAINT [MeterSessions_Meters] FOREIGN KEY ([MeterId]) REFERENCES [Meters] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [MeterSessions_MeterSessionSettings] FOREIGN KEY ([SessionSettingId]) REFERENCES [MeterSessionSettings] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [MeterSessions_MeterSessionStatuses] FOREIGN KEY ([SessionStatusId]) REFERENCES [MeterSessionStatuses] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [OnDemandMeterSessions] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Guid] nvarchar(100) NOT NULL COLLATE NOCASE
, [DataSetId] int NOT NULL
, [MeterId] int NOT NULL
, [SessionTypeId] int NOT NULL
, [MeterSessionId] int NOT NULL
, [UserSessionId] int NOT NULL
, CONSTRAINT [OnDemandMeterSessions_DataSets] FOREIGN KEY ([DataSetId]) REFERENCES [DataSets] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [OnDemandMeterSessions_MeterSessions] FOREIGN KEY ([MeterSessionId]) REFERENCES [MeterSession] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [OnDemandMeterSessions_MeterSessionTypes] FOREIGN KEY ([SessionTypeId]) REFERENCES [MeterSessionTypes] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [OnDemandMeterSessions_UserSessions] FOREIGN KEY ([UserSessionId]) REFERENCES [UserSessions] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
, CONSTRAINT [OnDemandsMeterSessions_Meters] FOREIGN KEY ([MeterId]) REFERENCES [Meters] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
INSERT INTO [Roles] ([Id],[Name],[Description]) VALUES (
1,'Administrator','Administrador del Sistema');
INSERT INTO [Users] ([Id],[Identification],[UserName],[Password],[RoleId]) VALUES (
1,'0000000','admin','admin',1);
CREATE UNIQUE INDEX [SIMCardServices_UQ__SIMCardServices__00000000000000D2] ON [SIMCardServices] ([Name] ASC);
CREATE UNIQUE INDEX [MeterSessionTypes_UQ__MeterSessionTypes__00000000000003DE] ON [MeterSessionTypes] ([Name] ASC);
CREATE UNIQUE INDEX [MeterSessionStatuses_UQ__MeterSessionStatuses__0000000000000260] ON [MeterSessionStatuses] ([Name] ASC);
CREATE UNIQUE INDEX [EnergySupplyGroups_UQ__EnergySupplyGroups__000000000000040E] ON [EnergySupplyGroups] ([Name] ASC);
CREATE UNIQUE INDEX [DataSets_UQ__DataSets__0000000000000053] ON [DataSets] ([Name] ASC);
CREATE UNIQUE INDEX [DataSetComponents_UQ__DataSetComponents__000000000000004E] ON [DataSetComponents] ([Name] ASC);
CREATE UNIQUE INDEX [Customers_UQ__Customers__0000000000000148] ON [Customers] ([IdentificationNumber] ASC);
CREATE UNIQUE INDEX [Countries_UQ__Countries__000000000000010C] ON [Countries] ([Name] ASC);
CREATE UNIQUE INDEX [States_UQ__States__0000000000000136] ON [States] ([Name] ASC);
CREATE UNIQUE INDEX [MeterSessionSettings_UQ__MeterSessionSettings__00000000000002B8] ON [MeterSessionSettings] ([Name] ASC);
CREATE UNIQUE INDEX [Cities_UQ__Cities__0000000000000122] ON [Cities] ([Name] ASC);
CREATE UNIQUE INDEX [PostalCodes_UQ__PostalCodes__00000000000003B2] ON [PostalCodes] ([Code] ASC);
CREATE UNIQUE INDEX [SIMCards_UQ__SIMCards__000000000000041A] ON [SIMCards] ([SerialNumber] ASC);
CREATE UNIQUE INDEX [SIMCards_UQ__SIMCards__0000000000000424] ON [SIMCards] ([PhoneNumber] ASC);
CREATE UNIQUE INDEX [MeterManufacturers_UQ__MeterManufacturers__00000000000001DC] ON [MeterManufacturers] ([Name] ASC);
CREATE UNIQUE INDEX [EnergySupplies_UQ__EnergySupplies__00000000000001BD] ON [EnergySupplies] ([Code] ASC);
CREATE UNIQUE INDEX [MeterSession_UQ__MeterSession__0000000000000214] ON [MeterSession] ([Guid] ASC);
CREATE UNIQUE INDEX [OnDemandMeterSessions_UQ__OnDemandMeterSessions__0000000000000384] ON [OnDemandMeterSessions] ([Guid] ASC);
CREATE TRIGGER [fki_Users_RoleId_Roles_Id] BEFORE Insert ON [Users] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table Users violates foreign key constraint Users_Roles') WHERE (SELECT Id FROM Roles WHERE  Id = NEW.RoleId) IS NULL; END;
CREATE TRIGGER [fku_Users_RoleId_Roles_Id] BEFORE Update ON [Users] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table Users violates foreign key constraint Users_Roles') WHERE (SELECT Id FROM Roles WHERE  Id = NEW.RoleId) IS NULL; END;
CREATE TRIGGER [fki_UserSessions_UserId_Users_Id] BEFORE Insert ON [UserSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table UserSessions violates foreign key constraint UserSessions_Users') WHERE (SELECT Id FROM Users WHERE  Id = NEW.UserId) IS NULL; END;
CREATE TRIGGER [fku_UserSessions_UserId_Users_Id] BEFORE Update ON [UserSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table UserSessions violates foreign key constraint UserSessions_Users') WHERE (SELECT Id FROM Users WHERE  Id = NEW.UserId) IS NULL; END;
CREATE TRIGGER [fki_ReconnectionSchedules_MeterSessionReconnectionSchemaId_MeterSessionReconnectionSchemas_Id] BEFORE Insert ON [ReconnectionSchedules] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table ReconnectionSchedules violates foreign key constraint ReconnectionSchedules_MeterSessionReconnectionSchemas') WHERE (SELECT Id FROM MeterSessionReconnectionSchemas WHERE  Id = NEW.MeterSessionReconnectionSchemaId) IS NULL; END;
CREATE TRIGGER [fku_ReconnectionSchedules_MeterSessionReconnectionSchemaId_MeterSessionReconnectionSchemas_Id] BEFORE Update ON [ReconnectionSchedules] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table ReconnectionSchedules violates foreign key constraint ReconnectionSchedules_MeterSessionReconnectionSchemas') WHERE (SELECT Id FROM MeterSessionReconnectionSchemas WHERE  Id = NEW.MeterSessionReconnectionSchemaId) IS NULL; END;
CREATE TRIGGER [fki_DataSets_DataSetComponents_DataSetId_DataSets_Id] BEFORE Insert ON [DataSets_DataSetComponents] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table DataSets_DataComponents violates foreign key constraint DataSets_DataSetComponents_DataSets') WHERE (SELECT Id FROM DataSets WHERE  Id = NEW.DataSetId) IS NULL; END;
CREATE TRIGGER [fku_DataSets_DataSetComponents_DataSetId_DataSets_Id] BEFORE Update ON [DataSets_DataSetComponents] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table DataSets_DataComponents violates foreign key constraint DataSets_DataSetComponents_DataSets') WHERE (SELECT Id FROM DataSets WHERE  Id = NEW.DataSetId) IS NULL; END;
CREATE TRIGGER [fki_DataSets_DataSetComponents_DataSetComponentId_DataSetComponents_Id] BEFORE Insert ON [DataSets_DataSetComponents] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table DataSets_DataComponents violates foreign key constraint DataSets_DataSetComponents_DataSetComponents') WHERE (SELECT Id FROM DataSetComponents WHERE  Id = NEW.DataSetComponentId) IS NULL; END;
CREATE TRIGGER [fku_DataSets_DataSetComponents_DataSetComponentId_DataSetComponents_Id] BEFORE Update ON [DataSets_DataSetComponents] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table DataSets_DataComponents violates foreign key constraint DataSets_DataSetComponents_DataSetComponents') WHERE (SELECT Id FROM DataSetComponents WHERE  Id = NEW.DataSetComponentId) IS NULL; END;
CREATE TRIGGER [fki_MeterSessionTasks_DataSetComponentId_DataSetComponents_Id] BEFORE Insert ON [MeterSessionTasks] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table MeterSessionTasks violates foreign key constraint MeterSessionTasks_DataSetComponents') WHERE (SELECT Id FROM DataSetComponents WHERE  Id = NEW.DataSetComponentId) IS NULL; END;
CREATE TRIGGER [fku_MeterSessionTasks_DataSetComponentId_DataSetComponents_Id] BEFORE Update ON [MeterSessionTasks] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table MeterSessionTasks violates foreign key constraint MeterSessionTasks_DataSetComponents') WHERE (SELECT Id FROM DataSetComponents WHERE  Id = NEW.DataSetComponentId) IS NULL; END;
CREATE TRIGGER [fki_MeterSessionItemTasks_SessionTaskId_MeterSessionTasks_Id] BEFORE Insert ON [MeterSessionItemTasks] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table MeterSessionItemTasks violates foreign key constraint MeterSessionItemTasks_MeterSessionTasks') WHERE (SELECT Id FROM MeterSessionTasks WHERE  Id = NEW.SessionTaskId) IS NULL; END;
CREATE TRIGGER [fku_MeterSessionItemTasks_SessionTaskId_MeterSessionTasks_Id] BEFORE Update ON [MeterSessionItemTasks] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table MeterSessionItemTasks violates foreign key constraint MeterSessionItemTasks_MeterSessionTasks') WHERE (SELECT Id FROM MeterSessionTasks WHERE  Id = NEW.SessionTaskId) IS NULL; END;
CREATE TRIGGER [fki_States_CountryId_Countries_Id] BEFORE Insert ON [States] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table States violates foreign key constraint States_Countries') WHERE (SELECT Id FROM Countries WHERE  Id = NEW.CountryId) IS NULL; END;
CREATE TRIGGER [fku_States_CountryId_Countries_Id] BEFORE Update ON [States] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table States violates foreign key constraint States_Countries') WHERE (SELECT Id FROM Countries WHERE  Id = NEW.CountryId) IS NULL; END;
CREATE TRIGGER [fki_MeterSessionSettings_CommunicationChannelSettingId_CommunicationChannelSettings_Id] BEFORE Insert ON [MeterSessionSettings] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table MeterSessionSettings violates foreign key constraint MeterSessionSettings_CommunicationChannelSettings') WHERE NEW.CommunicationChannelSettingId IS NOT NULL AND(SELECT Id FROM CommunicationChannelSettings WHERE  Id = NEW.CommunicationChannelSettingId) IS NULL; END;
CREATE TRIGGER [fku_MeterSessionSettings_CommunicationChannelSettingId_CommunicationChannelSettings_Id] BEFORE Update ON [MeterSessionSettings] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table MeterSessionSettings violates foreign key constraint MeterSessionSettings_CommunicationChannelSettings') WHERE NEW.CommunicationChannelSettingId IS NOT NULL AND(SELECT Id FROM CommunicationChannelSettings WHERE  Id = NEW.CommunicationChannelSettingId) IS NULL; END;
CREATE TRIGGER [fki_MeterSessionSettings_ProtocolSettingId_MeterProtocolSettings_Id] BEFORE Insert ON [MeterSessionSettings] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table MeterSessionSettings violates foreign key constraint MeterSessionSettings_MeterProtocolSettings') WHERE (SELECT Id FROM MeterProtocolSettings WHERE  Id = NEW.ProtocolSettingId) IS NULL; END;
CREATE TRIGGER [fku_MeterSessionSettings_ProtocolSettingId_MeterProtocolSettings_Id] BEFORE Update ON [MeterSessionSettings] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table MeterSessionSettings violates foreign key constraint MeterSessionSettings_MeterProtocolSettings') WHERE (SELECT Id FROM MeterProtocolSettings WHERE  Id = NEW.ProtocolSettingId) IS NULL; END;
CREATE TRIGGER [fki_MeterSessionSettings_ReconnectionSchemaId_MeterSessionReconnectionSchemas_Id] BEFORE Insert ON [MeterSessionSettings] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table MeterSessionSettings violates foreign key constraint MeterSessionSettings_MeterSessionReconnectionSchemas') WHERE NEW.ReconnectionSchemaId IS NOT NULL AND(SELECT Id FROM MeterSessionReconnectionSchemas WHERE  Id = NEW.ReconnectionSchemaId) IS NULL; END;
CREATE TRIGGER [fku_MeterSessionSettings_ReconnectionSchemaId_MeterSessionReconnectionSchemas_Id] BEFORE Update ON [MeterSessionSettings] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table MeterSessionSettings violates foreign key constraint MeterSessionSettings_MeterSessionReconnectionSchemas') WHERE NEW.ReconnectionSchemaId IS NOT NULL AND(SELECT Id FROM MeterSessionReconnectionSchemas WHERE  Id = NEW.ReconnectionSchemaId) IS NULL; END;
CREATE TRIGGER [fki_RemoteMeterSessionSettings_MeterSessionSettingId_MeterSessionSettings_Id] BEFORE Insert ON [RemoteMeterSessionSettings] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table RemoteMeterSessionSettings violates foreign key constraint RemoteMeterSessionSettings_MeterSessionSettings') WHERE (SELECT Id FROM MeterSessionSettings WHERE  Id = NEW.MeterSessionSettingId) IS NULL; END;
CREATE TRIGGER [fku_RemoteMeterSessionSettings_MeterSessionSettingId_MeterSessionSettings_Id] BEFORE Update ON [RemoteMeterSessionSettings] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table RemoteMeterSessionSettings violates foreign key constraint RemoteMeterSessionSettings_MeterSessionSettings') WHERE (SELECT Id FROM MeterSessionSettings WHERE  Id = NEW.MeterSessionSettingId) IS NULL; END;
CREATE TRIGGER [fki_InsituMeterSessionSettings_MeterSessionSettingId_MeterSessionSettings_Id] BEFORE Insert ON [InsituMeterSessionSettings] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table InsituMeterSessionSettings violates foreign key constraint InsituMeterSessionSettings_MeterSessionSettings') WHERE (SELECT Id FROM MeterSessionSettings WHERE  Id = NEW.MeterSessionSettingId) IS NULL; END;
CREATE TRIGGER [fku_InsituMeterSessionSettings_MeterSessionSettingId_MeterSessionSettings_Id] BEFORE Update ON [InsituMeterSessionSettings] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table InsituMeterSessionSettings violates foreign key constraint InsituMeterSessionSettings_MeterSessionSettings') WHERE (SELECT Id FROM MeterSessionSettings WHERE  Id = NEW.MeterSessionSettingId) IS NULL; END;
CREATE TRIGGER [fki_Cities_StateId_States_Id] BEFORE Insert ON [Cities] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table Cities violates foreign key constraint Cities_States') WHERE (SELECT Id FROM States WHERE  Id = NEW.StateId) IS NULL; END;
CREATE TRIGGER [fku_Cities_StateId_States_Id] BEFORE Update ON [Cities] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table Cities violates foreign key constraint Cities_States') WHERE (SELECT Id FROM States WHERE  Id = NEW.StateId) IS NULL; END;
CREATE TRIGGER [fki_PostalCodes_CityId_Cities_Id] BEFORE Insert ON [PostalCodes] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table PostalCodes violates foreign key constraint PostalCodes_Cities') WHERE (SELECT Id FROM Cities WHERE  Id = NEW.CityId) IS NULL; END;
CREATE TRIGGER [fku_PostalCodes_CityId_Cities_Id] BEFORE Update ON [PostalCodes] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table PostalCodes violates foreign key constraint PostalCodes_Cities') WHERE (SELECT Id FROM Cities WHERE  Id = NEW.CityId) IS NULL; END;
CREATE TRIGGER [fki_Addresses_PostalCodeId_PostalCodes_Id] BEFORE Insert ON [Addresses] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table Addresses violates foreign key constraint Addresses_PostalCodes') WHERE (SELECT Id FROM PostalCodes WHERE  Id = NEW.PostalCodeId) IS NULL; END;
CREATE TRIGGER [fku_Addresses_PostalCodeId_PostalCodes_Id] BEFORE Update ON [Addresses] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table Addresses violates foreign key constraint Addresses_PostalCodes') WHERE (SELECT Id FROM PostalCodes WHERE  Id = NEW.PostalCodeId) IS NULL; END;
CREATE TRIGGER [fki_Addresses_PostalCodeId_Streets_Id] BEFORE Insert ON [Addresses] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table Addresses violates foreign key constraint Addresses_Streets') WHERE (SELECT Id FROM Streets WHERE  Id = NEW.PostalCodeId) IS NULL; END;
CREATE TRIGGER [fku_Addresses_PostalCodeId_Streets_Id] BEFORE Update ON [Addresses] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table Addresses violates foreign key constraint Addresses_Streets') WHERE (SELECT Id FROM Streets WHERE  Id = NEW.PostalCodeId) IS NULL; END;
CREATE TRIGGER [fki_Vendors_AddressId_Addresses_Id] BEFORE Insert ON [Vendors] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table Vendors violates foreign key constraint Vendors_Addresses') WHERE NEW.AddressId IS NOT NULL AND(SELECT Id FROM Addresses WHERE  Id = NEW.AddressId) IS NULL; END;
CREATE TRIGGER [fku_Vendors_AddressId_Addresses_Id] BEFORE Update ON [Vendors] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table Vendors violates foreign key constraint Vendors_Addresses') WHERE NEW.AddressId IS NOT NULL AND(SELECT Id FROM Addresses WHERE  Id = NEW.AddressId) IS NULL; END;
CREATE TRIGGER [fki_PhoneCompanies_AddressId_Addresses_Id] BEFORE Insert ON [PhoneCompanies] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table PhoneCompanies violates foreign key constraint PhoneCompanies_Addresses') WHERE NEW.AddressId IS NOT NULL AND(SELECT Id FROM Addresses WHERE  Id = NEW.AddressId) IS NULL; END;
CREATE TRIGGER [fku_PhoneCompanies_AddressId_Addresses_Id] BEFORE Update ON [PhoneCompanies] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table PhoneCompanies violates foreign key constraint PhoneCompanies_Addresses') WHERE NEW.AddressId IS NOT NULL AND(SELECT Id FROM Addresses WHERE  Id = NEW.AddressId) IS NULL; END;
CREATE TRIGGER [fki_SIMCards_PhoneCompanyId_PhoneCompanies_Id] BEFORE Insert ON [SIMCards] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table SIMCards violates foreign key constraint SIMCards_PhoneCompanies') WHERE (SELECT Id FROM PhoneCompanies WHERE  Id = NEW.PhoneCompanyId) IS NULL; END;
CREATE TRIGGER [fku_SIMCards_PhoneCompanyId_PhoneCompanies_Id] BEFORE Update ON [SIMCards] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table SIMCards violates foreign key constraint SIMCards_PhoneCompanies') WHERE (SELECT Id FROM PhoneCompanies WHERE  Id = NEW.PhoneCompanyId) IS NULL; END;
CREATE TRIGGER [fki_SIMCards_ServiceId_SIMCardServices_Id] BEFORE Insert ON [SIMCards] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table SIMCards violates foreign key constraint SIMCards_SIMCardServices') WHERE (SELECT Id FROM SIMCardServices WHERE  Id = NEW.ServiceId) IS NULL; END;
CREATE TRIGGER [fku_SIMCards_ServiceId_SIMCardServices_Id] BEFORE Update ON [SIMCards] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table SIMCards violates foreign key constraint SIMCards_SIMCardServices') WHERE (SELECT Id FROM SIMCardServices WHERE  Id = NEW.ServiceId) IS NULL; END;
CREATE TRIGGER [fki_RemoteDevices_SIMCardId_SIMCards_Id] BEFORE Insert ON [RemoteDevices] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table RemoteDevices violates foreign key constraint RemoteDevices_SIMCards') WHERE (SELECT Id FROM SIMCards WHERE  Id = NEW.SIMCardId) IS NULL; END;
CREATE TRIGGER [fku_RemoteDevices_SIMCardId_SIMCards_Id] BEFORE Update ON [RemoteDevices] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table RemoteDevices violates foreign key constraint RemoteDevices_SIMCards') WHERE (SELECT Id FROM SIMCards WHERE  Id = NEW.SIMCardId) IS NULL; END;
CREATE TRIGGER [fki_RemoteDevices_VendorId_Vendors_Id] BEFORE Insert ON [RemoteDevices] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table RemoteDevices violates foreign key constraint RemoteDevices_Vendors') WHERE NEW.VendorId IS NOT NULL AND(SELECT Id FROM Vendors WHERE  Id = NEW.VendorId) IS NULL; END;
CREATE TRIGGER [fku_RemoteDevices_VendorId_Vendors_Id] BEFORE Update ON [RemoteDevices] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table RemoteDevices violates foreign key constraint RemoteDevices_Vendors') WHERE NEW.VendorId IS NOT NULL AND(SELECT Id FROM Vendors WHERE  Id = NEW.VendorId) IS NULL; END;
CREATE TRIGGER [fki_MeterManufacturers_AddressId_Addresses_Id] BEFORE Insert ON [MeterManufacturers] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table MeterManufacturers violates foreign key constraint MeterManufacturers_Addresses') WHERE NEW.AddressId IS NOT NULL AND(SELECT Id FROM Addresses WHERE  Id = NEW.AddressId) IS NULL; END;
CREATE TRIGGER [fku_MeterManufacturers_AddressId_Addresses_Id] BEFORE Update ON [MeterManufacturers] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table MeterManufacturers violates foreign key constraint MeterManufacturers_Addresses') WHERE NEW.AddressId IS NOT NULL AND(SELECT Id FROM Addresses WHERE  Id = NEW.AddressId) IS NULL; END;
CREATE TRIGGER [fki_EnergySupplies_AddressId_Addresses_Id] BEFORE Insert ON [EnergySupplies] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table EnergySupplies violates foreign key constraint EnergySupplies_Addresses') WHERE (SELECT Id FROM Addresses WHERE  Id = NEW.AddressId) IS NULL; END;
CREATE TRIGGER [fku_EnergySupplies_AddressId_Addresses_Id] BEFORE Update ON [EnergySupplies] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table EnergySupplies violates foreign key constraint EnergySupplies_Addresses') WHERE (SELECT Id FROM Addresses WHERE  Id = NEW.AddressId) IS NULL; END;
CREATE TRIGGER [fki_EnergySupplies_CustomerId_Customers_Id] BEFORE Insert ON [EnergySupplies] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table EnergySupplies violates foreign key constraint EnergySupplies_Customers') WHERE (SELECT Id FROM Customers WHERE  Id = NEW.CustomerId) IS NULL; END;
CREATE TRIGGER [fku_EnergySupplies_CustomerId_Customers_Id] BEFORE Update ON [EnergySupplies] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table EnergySupplies violates foreign key constraint EnergySupplies_Customers') WHERE (SELECT Id FROM Customers WHERE  Id = NEW.CustomerId) IS NULL; END;
CREATE TRIGGER [fki_EnergySupplies_GroupId_EnergySupplyGroups_Id] BEFORE Insert ON [EnergySupplies] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table EnergySupplies violates foreign key constraint EnergySupplies_EnergySupplyGroups') WHERE NEW.GroupId IS NOT NULL AND(SELECT Id FROM EnergySupplyGroups WHERE  Id = NEW.GroupId) IS NULL; END;
CREATE TRIGGER [fku_EnergySupplies_GroupId_EnergySupplyGroups_Id] BEFORE Update ON [EnergySupplies] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table EnergySupplies violates foreign key constraint EnergySupplies_EnergySupplyGroups') WHERE NEW.GroupId IS NOT NULL AND(SELECT Id FROM EnergySupplyGroups WHERE  Id = NEW.GroupId) IS NULL; END;
CREATE TRIGGER [fki_EnergySupplies_GeoCoordinateId_GeoCoordinates_Id] BEFORE Insert ON [EnergySupplies] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table EnergySupplies violates foreign key constraint EnergySupplies_GeoCoordinates') WHERE NEW.GeoCoordinateId IS NOT NULL AND(SELECT Id FROM GeoCoordinates WHERE  Id = NEW.GeoCoordinateId) IS NULL; END;
CREATE TRIGGER [fku_EnergySupplies_GeoCoordinateId_GeoCoordinates_Id] BEFORE Update ON [EnergySupplies] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table EnergySupplies violates foreign key constraint EnergySupplies_GeoCoordinates') WHERE NEW.GeoCoordinateId IS NOT NULL AND(SELECT Id FROM GeoCoordinates WHERE  Id = NEW.GeoCoordinateId) IS NULL; END;
CREATE TRIGGER [fki_Meters_EnergySupplyId_EnergySupplies_Id] BEFORE Insert ON [Meters] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table Meters violates foreign key constraint Meters_EnergySupplies') WHERE (SELECT Id FROM EnergySupplies WHERE  Id = NEW.EnergySupplyId) IS NULL; END;
CREATE TRIGGER [fku_Meters_EnergySupplyId_EnergySupplies_Id] BEFORE Update ON [Meters] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table Meters violates foreign key constraint Meters_EnergySupplies') WHERE (SELECT Id FROM EnergySupplies WHERE  Id = NEW.EnergySupplyId) IS NULL; END;
CREATE TRIGGER [fki_Meters_MeterManufacturerId_MeterManufacturers_Id] BEFORE Insert ON [Meters] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table Meters violates foreign key constraint Meters_MeterManufacturers') WHERE (SELECT Id FROM MeterManufacturers WHERE  Id = NEW.MeterManufacturerId) IS NULL; END;
CREATE TRIGGER [fku_Meters_MeterManufacturerId_MeterManufacturers_Id] BEFORE Update ON [Meters] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table Meters violates foreign key constraint Meters_MeterManufacturers') WHERE (SELECT Id FROM MeterManufacturers WHERE  Id = NEW.MeterManufacturerId) IS NULL; END;
CREATE TRIGGER [fki_Meters_RemoteDeviceId_RemoteDevices_Id] BEFORE Insert ON [Meters] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table Meters violates foreign key constraint Meters_RemoteDevices') WHERE NEW.RemoteDeviceId IS NOT NULL AND(SELECT Id FROM RemoteDevices WHERE  Id = NEW.RemoteDeviceId) IS NULL; END;
CREATE TRIGGER [fku_Meters_RemoteDeviceId_RemoteDevices_Id] BEFORE Update ON [Meters] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table Meters violates foreign key constraint Meters_RemoteDevices') WHERE NEW.RemoteDeviceId IS NOT NULL AND(SELECT Id FROM RemoteDevices WHERE  Id = NEW.RemoteDeviceId) IS NULL; END;
CREATE TRIGGER [fki_MeterSession_MeterId_Meters_Id] BEFORE Insert ON [MeterSession] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table MeterSession violates foreign key constraint MeterSessions_Meters') WHERE (SELECT Id FROM Meters WHERE  Id = NEW.MeterId) IS NULL; END;
CREATE TRIGGER [fku_MeterSession_MeterId_Meters_Id] BEFORE Update ON [MeterSession] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table MeterSession violates foreign key constraint MeterSessions_Meters') WHERE (SELECT Id FROM Meters WHERE  Id = NEW.MeterId) IS NULL; END;
CREATE TRIGGER [fki_MeterSession_SessionSettingId_MeterSessionSettings_Id] BEFORE Insert ON [MeterSession] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table MeterSession violates foreign key constraint MeterSessions_MeterSessionSettings') WHERE (SELECT Id FROM MeterSessionSettings WHERE  Id = NEW.SessionSettingId) IS NULL; END;
CREATE TRIGGER [fku_MeterSession_SessionSettingId_MeterSessionSettings_Id] BEFORE Update ON [MeterSession] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table MeterSession violates foreign key constraint MeterSessions_MeterSessionSettings') WHERE (SELECT Id FROM MeterSessionSettings WHERE  Id = NEW.SessionSettingId) IS NULL; END;
CREATE TRIGGER [fki_MeterSession_SessionStatusId_MeterSessionStatuses_Id] BEFORE Insert ON [MeterSession] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table MeterSession violates foreign key constraint MeterSessions_MeterSessionStatuses') WHERE (SELECT Id FROM MeterSessionStatuses WHERE  Id = NEW.SessionStatusId) IS NULL; END;
CREATE TRIGGER [fku_MeterSession_SessionStatusId_MeterSessionStatuses_Id] BEFORE Update ON [MeterSession] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table MeterSession violates foreign key constraint MeterSessions_MeterSessionStatuses') WHERE (SELECT Id FROM MeterSessionStatuses WHERE  Id = NEW.SessionStatusId) IS NULL; END;
CREATE TRIGGER [fki_OnDemandMeterSessions_DataSetId_DataSets_Id] BEFORE Insert ON [OnDemandMeterSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table OnDemandMeterSessions violates foreign key constraint OnDemandMeterSessions_DataSets') WHERE (SELECT Id FROM DataSets WHERE  Id = NEW.DataSetId) IS NULL; END;
CREATE TRIGGER [fku_OnDemandMeterSessions_DataSetId_DataSets_Id] BEFORE Update ON [OnDemandMeterSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table OnDemandMeterSessions violates foreign key constraint OnDemandMeterSessions_DataSets') WHERE (SELECT Id FROM DataSets WHERE  Id = NEW.DataSetId) IS NULL; END;
CREATE TRIGGER [fki_OnDemandMeterSessions_MeterSessionId_MeterSession_Id] BEFORE Insert ON [OnDemandMeterSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table OnDemandMeterSessions violates foreign key constraint OnDemandMeterSessions_MeterSessions') WHERE (SELECT Id FROM MeterSession WHERE  Id = NEW.MeterSessionId) IS NULL; END;
CREATE TRIGGER [fku_OnDemandMeterSessions_MeterSessionId_MeterSession_Id] BEFORE Update ON [OnDemandMeterSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table OnDemandMeterSessions violates foreign key constraint OnDemandMeterSessions_MeterSessions') WHERE (SELECT Id FROM MeterSession WHERE  Id = NEW.MeterSessionId) IS NULL; END;
CREATE TRIGGER [fki_OnDemandMeterSessions_SessionTypeId_MeterSessionTypes_Id] BEFORE Insert ON [OnDemandMeterSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table OnDemandMeterSessions violates foreign key constraint OnDemandMeterSessions_MeterSessionTypes') WHERE (SELECT Id FROM MeterSessionTypes WHERE  Id = NEW.SessionTypeId) IS NULL; END;
CREATE TRIGGER [fku_OnDemandMeterSessions_SessionTypeId_MeterSessionTypes_Id] BEFORE Update ON [OnDemandMeterSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table OnDemandMeterSessions violates foreign key constraint OnDemandMeterSessions_MeterSessionTypes') WHERE (SELECT Id FROM MeterSessionTypes WHERE  Id = NEW.SessionTypeId) IS NULL; END;
CREATE TRIGGER [fki_OnDemandMeterSessions_UserSessionId_UserSessions_Id] BEFORE Insert ON [OnDemandMeterSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table OnDemandMeterSessions violates foreign key constraint OnDemandMeterSessions_UserSessions') WHERE (SELECT Id FROM UserSessions WHERE  Id = NEW.UserSessionId) IS NULL; END;
CREATE TRIGGER [fku_OnDemandMeterSessions_UserSessionId_UserSessions_Id] BEFORE Update ON [OnDemandMeterSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table OnDemandMeterSessions violates foreign key constraint OnDemandMeterSessions_UserSessions') WHERE (SELECT Id FROM UserSessions WHERE  Id = NEW.UserSessionId) IS NULL; END;
CREATE TRIGGER [fki_OnDemandMeterSessions_MeterId_Meters_Id] BEFORE Insert ON [OnDemandMeterSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table OnDemandMeterSessions violates foreign key constraint OnDemandsMeterSessions_Meters') WHERE (SELECT Id FROM Meters WHERE  Id = NEW.MeterId) IS NULL; END;
CREATE TRIGGER [fku_OnDemandMeterSessions_MeterId_Meters_Id] BEFORE Update ON [OnDemandMeterSessions] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table OnDemandMeterSessions violates foreign key constraint OnDemandsMeterSessions_Meters') WHERE (SELECT Id FROM Meters WHERE  Id = NEW.MeterId) IS NULL; END;
COMMIT;

