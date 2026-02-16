-- ============================================================================
-- Wind Turbine Monitor - Complete MySQL Database Setup Script
-- ============================================================================
-- Usage:
-- 1. Open MySQL Command Line or MySQL Workbench
-- 2. Run this script
-- ============================================================================

-- Drop database if exists
DROP DATABASE IF EXISTS WindTurbineMonitor_Dev;

-- Create database
CREATE DATABASE WindTurbineMonitor_Dev
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE WindTurbineMonitor_Dev;

-- ============================================================================
-- CREATE TABLES
-- ============================================================================

-- Table: Turbines
CREATE TABLE Turbines (
    Id CHAR(36) PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Location VARCHAR(500) NOT NULL,
    Model VARCHAR(100),
    Capacity DECIMAL(10, 2) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- Table: TurbineReadings
CREATE TABLE TurbineReadings (
    Id CHAR(36) PRIMARY KEY,
    TurbineId CHAR(36) NOT NULL,
    Timestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RotorSpeedRpm DOUBLE NOT NULL,
    WindSpeedMs DOUBLE NOT NULL,
    PowerKw DOUBLE NOT NULL,
    EfficiencyPercent DOUBLE NOT NULL,
    WindDirectionDegrees DOUBLE NOT NULL,
    Status VARCHAR(50) NOT NULL,
    JsonData TEXT,
    GearboxTemperature DOUBLE,
    GeneratorTemperature DOUBLE,
    HydraulicPressure DOUBLE,
    VibrationLevel DOUBLE,
    CONSTRAINT FK_TurbineReadings_Turbines FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Table: TurbineEvents
CREATE TABLE TurbineEvents (
    Id CHAR(36) PRIMARY KEY,
    TurbineId CHAR(36) NOT NULL,
    EventType VARCHAR(100) NOT NULL,
    Message TEXT NOT NULL,
    Severity VARCHAR(50) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UserId VARCHAR(255),
    AdditionalData TEXT,
    CONSTRAINT FK_TurbineEvents_Turbines FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Table: ComponentStatuses
CREATE TABLE ComponentStatuses (
    Id CHAR(36) PRIMARY KEY,
    TurbineId CHAR(36) NOT NULL,
    ComponentType VARCHAR(50) NOT NULL,
    ComponentName VARCHAR(255) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    HealthPercentage DOUBLE NOT NULL,
    LastUpdated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT,
    AdditionalData TEXT,
    CONSTRAINT FK_ComponentStatuses_Turbines FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Table: MaintenanceSchedules
CREATE TABLE MaintenanceSchedules (
    Id CHAR(36) PRIMARY KEY,
    TurbineId CHAR(36) NOT NULL,
    ComponentType VARCHAR(50) NOT NULL,
    ScheduledDate TIMESTAMP NOT NULL,
    IsCompleted BOOLEAN NOT NULL DEFAULT FALSE,
    CompletedDate TIMESTAMP NULL,
    CompletedBy VARCHAR(255),
    Notes TEXT,
    Priority INT NOT NULL DEFAULT 1,
    Title VARCHAR(255),
    CONSTRAINT FK_MaintenanceSchedules_Turbines FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Table: AuditLogs
CREATE TABLE AuditLogs (
    Id CHAR(36) PRIMARY KEY,
    TurbineId CHAR(36) NULL,
    UserId VARCHAR(100),
    Username VARCHAR(100),
    Action VARCHAR(50) NOT NULL,
    EntityName VARCHAR(100) NOT NULL,
    EntityId VARCHAR(50),
    OldValue TEXT,
    NewValue TEXT,
    Timestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IpAddress VARCHAR(50),
    UserAgent TEXT,
    AdditionalData TEXT
);

-- ============================================================================
-- CREATE INDEXES
-- ============================================================================

CREATE INDEX IX_TurbineReadings_TurbineId ON TurbineReadings(TurbineId);
CREATE INDEX IX_TurbineReadings_Timestamp ON TurbineReadings(Timestamp DESC);
CREATE INDEX IX_TurbineEvents_TurbineId ON TurbineEvents(TurbineId);
CREATE INDEX IX_TurbineEvents_CreatedAt ON TurbineEvents(CreatedAt DESC);
CREATE INDEX IX_TurbineEvents_Severity ON TurbineEvents(Severity);
CREATE INDEX IX_ComponentStatuses_TurbineId ON ComponentStatuses(TurbineId);
CREATE INDEX IX_ComponentStatuses_ComponentType ON ComponentStatuses(ComponentType);
CREATE INDEX IX_MaintenanceSchedules_TurbineId ON MaintenanceSchedules(TurbineId);
CREATE INDEX IX_MaintenanceSchedules_ScheduledDate ON MaintenanceSchedules(ScheduledDate);
CREATE INDEX IX_MaintenanceSchedules_IsCompleted ON MaintenanceSchedules(IsCompleted);
CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_Timestamp ON AuditLogs(Timestamp);
CREATE INDEX IX_AuditLogs_Action ON AuditLogs(Action);
CREATE INDEX IX_AuditLogs_EntityName ON AuditLogs(EntityName);

-- ============================================================================
-- INSERT SEED DATA
-- ============================================================================

-- Insert 3 Sample Turbines
INSERT INTO Turbines (Id, Name, Location, Model, Capacity, Status, CreatedAt, IsActive) VALUES
('11111111-1111-1111-1111-111111111111', 'Turbine Alpha-01', 'Jakarta Coastal Wind Farm', 'WTG-2000', 2.5, 'Offline', NOW(), TRUE),
('22222222-2222-2222-2222-222222222222', 'Turbine Beta-02', 'Jakarta Coastal Wind Farm', 'WTG-2000', 2.5, 'Running', NOW(), TRUE),
('33333333-3333-3333-3333-333333333333', 'Turbine Gamma-03', 'Jakarta Coastal Wind Farm', 'WTG-3000', 3.5, 'Running', NOW(), TRUE);

-- Insert Initial Events for each turbine
INSERT INTO TurbineEvents (Id, TurbineId, EventType, Message, Severity, CreatedAt) VALUES
('aaaaaaaa-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'System', 'Turbine registered and initialized', 'Success', NOW()),
('bbbbbbbb-2222-2222-2222-222222222222', '22222222-2222-2222-2222-222222222222', 'System', 'Turbine started successfully', 'Success', NOW()),
('cccccccc-3333-3333-3333-333333333333', '33333333-3333-3333-3333-333333333333', 'System', 'Turbine started successfully', 'Success', NOW());

-- Insert Component Statuses for each turbine
-- Turbine Alpha-01
INSERT INTO ComponentStatuses (Id, TurbineId, ComponentType, ComponentName, Status, HealthPercentage, LastUpdated, Description) VALUES
('10000000-0001-0001-0001-000000000001', '11111111-1111-1111-1111-111111111111', 'Tower', 'Main Tower', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0001-0001-0001-000000000002', '11111111-1111-1111-1111-111111111111', 'Nacelle', 'Nacelle', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0001-0001-0001-000000000003', '11111111-1111-1111-1111-111111111111', 'RotorBlade', 'Rotor Blades', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0001-0001-0001-000000000004', '11111111-1111-1111-1111-111111111111', 'Gearbox', 'Gearbox', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0001-0001-0001-000000000005', '11111111-1111-1111-1111-111111111111', 'Generator', 'Generator', 'Healthy', 100.0, NOW(), 'Component in excellent condition');

-- Turbine Beta-02
INSERT INTO ComponentStatuses (Id, TurbineId, ComponentType, ComponentName, Status, HealthPercentage, LastUpdated, Description) VALUES
('10000000-0002-0002-0002-000000000001', '22222222-2222-2222-2222-222222222222', 'Tower', 'Main Tower', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0002-0002-0002-000000000002', '22222222-2222-2222-2222-222222222222', 'Nacelle', 'Nacelle', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0002-0002-0002-000000000003', '22222222-2222-2222-2222-222222222222', 'RotorBlade', 'Rotor Blades', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0002-0002-0002-000000000004', '22222222-2222-2222-2222-222222222222', 'Gearbox', 'Gearbox', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0002-0002-0002-000000000005', '22222222-2222-2222-2222-222222222222', 'Generator', 'Generator', 'Healthy', 100.0, NOW(), 'Component in excellent condition');

-- Turbine Gamma-03
INSERT INTO ComponentStatuses (Id, TurbineId, ComponentType, ComponentName, Status, HealthPercentage, LastUpdated, Description) VALUES
('10000000-0003-0003-0003-000000000001', '33333333-3333-3333-3333-333333333333', 'Tower', 'Main Tower', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0003-0003-0003-0003-000000000002', '33333333-3333-3333-3333-333333333333', 'Nacelle', 'Nacelle', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0003-0003-0003-0003-000000000003', '33333333-3333-3333-3333-333333333333', 'RotorBlade', 'Rotor Blades', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0003-0003-0003-0003-000000000004', '33333333-3333-3333-3333-333333333333', 'Gearbox', 'Gearbox', 'Healthy', 100.0, NOW(), 'Component in excellent condition'),
('10000000-0003-0003-0003-0003-000000000005', '33333333-3333-3333-3333-333333333333', 'Generator', 'Generator', 'Healthy', 100.0, NOW(), 'Component in excellent condition');

-- ============================================================================
-- VERIFICATION QUERIES
-- ============================================================================

-- Display all tables
SELECT '=== ALL TABLES ===' AS info;
SHOW TABLES;

-- Count records in each table
SELECT '=== RECORD COUNTS ===' AS info;
SELECT 'Turbines' AS Table_Name, COUNT(*) AS Record_Count FROM Turbines
UNION ALL
SELECT 'TurbineReadings', COUNT(*) FROM TurbineReadings
UNION ALL
SELECT 'TurbineEvents', COUNT(*) FROM TurbineEvents
UNION ALL
SELECT 'ComponentStatuses', COUNT(*) FROM ComponentStatuses
UNION ALL
SELECT 'MaintenanceSchedules', COUNT(*) FROM MaintenanceSchedules
UNION ALL
SELECT 'AuditLogs', COUNT(*) FROM AuditLogs;

-- Display all turbines
SELECT '=== ALL TURBINES ===' AS info;
SELECT
    Id,
    Name,
    Location,
    Model,
    Capacity,
    Status,
    IsActive
FROM Turbines
ORDER BY Name;

-- ============================================================================
-- SETUP COMPLETE!
-- ============================================================================
SELECT '========================================' AS info;
SELECT 'DATABASE SETUP COMPLETED SUCCESSFULLY!' AS info;
SELECT 'Database: WindTurbineMonitor_Dev' AS info;
SELECT 'Tables: 6 (Turbines, TurbineReadings, TurbineEvents, ComponentStatuses, MaintenanceSchedules, AuditLogs)' AS info;
SELECT 'Seed Data: 3 Turbines, 3 Events, 15 Component Statuses' AS info;
SELECT '========================================' AS info;
