-- ============================================================================
-- Wind Turbine Monitor - Complete Database Setup Script
-- ============================================================================
-- Usage:
-- 1. Open pgAdmin 4
-- 2. Connect to your PostgreSQL server
-- 3. Open Query Tool (press F6 or click the query tool icon)
-- 4. Copy and paste this entire script
-- 5. Execute (press F5 or click Execute)
-- ============================================================================

-- Drop existing database if it exists (WARNING: This will delete all data!)
DROP DATABASE IF EXISTS WindTurbineMonitor_Dev;

-- Create the database
CREATE DATABASE WindTurbineMonitor_Dev
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- Connect to the new database
\c WindTurbineMonitor_Dev;

-- ============================================================================
-- CREATE TABLES
-- ============================================================================

-- Table: Turbines
CREATE TABLE Turbines (
    Id UUID PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Location VARCHAR(500) NOT NULL,
    Model VARCHAR(100),
    Capacity DECIMAL(10, 2) NOT NULL,
    Status VARCHAR(50) NOT NULL,  -- Running, Idle, Offline, Error, Maintenance
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT true
);

-- Table: TurbineReadings
CREATE TABLE TurbineReadings (
    Id UUID PRIMARY KEY,
    TurbineId UUID NOT NULL,
    Timestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RotorSpeedRpm DOUBLE PRECISION NOT NULL,
    WindSpeedMs DOUBLE PRECISION NOT NULL,
    PowerKw DOUBLE PRECISION NOT NULL,
    EfficiencyPercent DOUBLE PRECISION NOT NULL,
    WindDirectionDegrees DOUBLE PRECISION NOT NULL,
    Status VARCHAR(50) NOT NULL,
    JsonData TEXT,
    GearboxTemperature DOUBLE PRECISION,
    GeneratorTemperature DOUBLE PRECISION,
    HydraulicPressure DOUBLE PRECISION,
    VibrationLevel DOUBLE PRECISION,
    CONSTRAINT FK_TurbineReadings_Turbines FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Table: TurbineEvents
CREATE TABLE TurbineEvents (
    Id UUID PRIMARY KEY,
    TurbineId UUID NOT NULL,
    EventType VARCHAR(100) NOT NULL,
    Message TEXT NOT NULL,
    Severity VARCHAR(50) NOT NULL,  -- Success, Info, Warning, Error
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UserId VARCHAR(255),
    AdditionalData TEXT,
    CONSTRAINT FK_TurbineEvents_Turbines FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Table: ComponentStatuses
CREATE TABLE ComponentStatuses (
    Id UUID PRIMARY KEY,
    TurbineId UUID NOT NULL,
    ComponentType VARCHAR(50) NOT NULL,  -- Tower, Nacelle, RotorBlade, Gearbox, Generator, etc.
    ComponentName VARCHAR(255) NOT NULL,
    Status VARCHAR(50) NOT NULL,  -- Healthy, Warning, Error, Maintenance
    HealthPercentage DOUBLE PRECISION NOT NULL,
    LastUpdated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT,
    AdditionalData TEXT,
    CONSTRAINT FK_ComponentStatuses_Turbines FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Table: MaintenanceSchedules
CREATE TABLE MaintenanceSchedules (
    Id UUID PRIMARY KEY,
    TurbineId UUID NOT NULL,
    ComponentType VARCHAR(50) NOT NULL,
    ScheduledDate TIMESTAMP NOT NULL,
    IsCompleted BOOLEAN NOT NULL DEFAULT false,
    CompletedDate TIMESTAMP,
    CompletedBy VARCHAR(255),
    Notes TEXT,
    Priority INTEGER NOT NULL DEFAULT 1,  -- 1 = Low, 5 = High
    Title VARCHAR(255),
    CONSTRAINT FK_MaintenanceSchedules_Turbines FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Create indexes for better query performance
CREATE INDEX IX_TurbineReadings_TurbineId ON TurbineReadings(TurbineId);
CREATE INDEX IX_TurbineReadings_Timestamp ON TurbineReadings(Timestamp DESC);
CREATE INDEX IX_TurbineEvents_TurbineId ON TurbineEvents(TurbineId);
CREATE INDEX IX_TurbineEvents_CreatedAt ON TurbineEvents(CreatedAt DESC);
CREATE INDEX IX_ComponentStatuses_TurbineId ON ComponentStatuses(TurbineId);
CREATE INDEX IX_MaintenanceSchedules_TurbineId ON MaintenanceSchedules(TurbineId);
CREATE INDEX IX_MaintenanceSchedules_ScheduledDate ON MaintenanceSchedules(ScheduledDate);

-- ============================================================================
-- INSERT SEED DATA
-- ============================================================================

-- Insert 3 Sample Turbines
INSERT INTO Turbines (Id, Name, Location, Model, Capacity, Status, CreatedAt, IsActive) VALUES
('11111111-1111-1111-1111-111111111111', 'Turbine Alpha-01', 'Jakarta Coastal Wind Farm', 'WTG-2000', 2.5, 'Offline', CURRENT_TIMESTAMP, true),
('22222222-2222-2222-2222-222222222222', 'Turbine Beta-02', 'Jakarta Coastal Wind Farm', 'WTG-2000', 2.5, 'Running', CURRENT_TIMESTAMP, true),
('33333333-3333-3333-3333-333333333333', 'Turbine Gamma-03', 'Jakarta Coastal Wind Farm', 'WTG-3000', 3.5, 'Running', CURRENT_TIMESTAMP, true);

-- Insert Initial Events for each turbine
INSERT INTO TurbineEvents (Id, TurbineId, EventType, Message, Severity, CreatedAt) VALUES
('aaaaaaaa-1111-1111-1111-000000000001', '11111111-1111-1111-1111-111111111111', 'System', 'Turbine registered and initialized', 'Success', CURRENT_TIMESTAMP),
('aaaaaaaa-2222-2222-2222-000000000001', '22222222-2222-2222-2222-222222222222', 'System', 'Turbine started successfully', 'Success', CURRENT_TIMESTAMP),
('aaaaaaaa-3333-3333-3333-000000000001', '33333333-3333-3333-3333-333333333333', 'System', 'Turbine started successfully', 'Success', CURRENT_TIMESTAMP);

-- Insert Component Statuses for each turbine
-- Turbine Alpha-01
INSERT INTO ComponentStatuses (Id, TurbineId, ComponentType, ComponentName, Status, HealthPercentage, LastUpdated, Description) VALUES
('cs-alpha-01-01', '11111111-1111-1111-1111-111111111111', 'Tower', 'Main Tower', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-alpha-01-02', '11111111-1111-1111-1111-111111111111', 'Nacelle', 'Nacelle', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-alpha-01-03', '11111111-1111-1111-1111-111111111111', 'RotorBlade', 'Rotor Blades', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-alpha-01-04', '11111111-1111-1111-1111-111111111111', 'Gearbox', 'Gearbox', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-alpha-01-05', '11111111-1111-1111-1111-111111111111', 'Generator', 'Generator', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition');

-- Turbine Beta-02
INSERT INTO ComponentStatuses (Id, TurbineId, ComponentType, ComponentName, Status, HealthPercentage, LastUpdated, Description) VALUES
('cs-beta-02-01', '22222222-2222-2222-2222-222222222222', 'Tower', 'Main Tower', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-beta-02-02', '22222222-2222-2222-2222-222222222222', 'Nacelle', 'Nacelle', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-beta-02-03', '22222222-2222-2222-2222-222222222222', 'RotorBlade', 'Rotor Blades', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-beta-02-04', '22222222-2222-2222-2222-222222222222', 'Gearbox', 'Gearbox', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-beta-02-05', '22222222-2222-2222-2222-222222222222', 'Generator', 'Generator', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition');

-- Turbine Gamma-03
INSERT INTO ComponentStatuses (Id, TurbineId, ComponentType, ComponentName, Status, HealthPercentage, LastUpdated, Description) VALUES
('cs-gamma-03-01', '33333333-3333-3333-3333-333333333333', 'Tower', 'Main Tower', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-gamma-03-02', '33333333-3333-3333-3333-333333333333', 'Nacelle', 'Nacelle', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-gamma-03-03', '33333333-3333-3333-3333-333333333333', 'RotorBlade', 'Rotor Blades', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-gamma-03-04', '33333333-3333-3333-3333-333333333333', 'Gearbox', 'Gearbox', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('cs-gamma-03-05', '33333333-3333-3333-3333-333333333333', 'Generator', 'Generator', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition');

-- ============================================================================
-- VERIFICATION QUERIES
-- ============================================================================

-- Display all tables
SELECT
    schemaname,
    tablename
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY tablename;

-- Count records in each table
SELECT 'Turbines' as Table_Name, COUNT(*) as Record_Count FROM Turbines
UNION ALL
SELECT 'TurbineReadings', COUNT(*) FROM TurbineReadings
UNION ALL
SELECT 'TurbineEvents', COUNT(*) FROM TurbineEvents
UNION ALL
SELECT 'ComponentStatuses', COUNT(*) FROM ComponentStatuses
UNION ALL
SELECT 'MaintenanceSchedules', COUNT(*) FROM MaintenanceSchedules;

-- Display all turbines
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
-- Database: WindTurbineMonitor_Dev
-- Tables: 5 (Turbines, TurbineReadings, TurbineEvents, ComponentStatuses, MaintenanceSchedules)
-- Seed Data: 3 Turbines with 3 Events and 15 Component Statuses
-- ============================================================================
