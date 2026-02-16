-- ============================================================================
-- STEP 2: Create Tables and Insert Data (FIXED VERSION)
-- ============================================================================
-- IMPORTANT: Run this AFTER step1_create_database.sql
-- Make sure you're connected to 'WindTurbineMonitor_Dev' database
-- ============================================================================

-- First, drop all tables if they exist (in correct order due to foreign keys)
DROP TABLE IF EXISTS MaintenanceSchedules CASCADE;
DROP TABLE IF EXISTS ComponentStatuses CASCADE;
DROP TABLE IF EXISTS TurbineEvents CASCADE;
DROP TABLE IF EXISTS TurbineReadings CASCADE;
DROP TABLE IF EXISTS Turbines CASCADE;

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
    Status VARCHAR(50) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT true
);

-- Verify Turbines table created
SELECT 'Turbines table created successfully' as status;

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
    CONSTRAINT fk_turbinereadings_turbine FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Verify TurbineReadings table created
SELECT 'TurbineReadings table created successfully' as status;

-- Table: TurbineEvents
CREATE TABLE TurbineEvents (
    Id UUID PRIMARY KEY,
    TurbineId UUID NOT NULL,
    EventType VARCHAR(100) NOT NULL,
    Message TEXT NOT NULL,
    Severity VARCHAR(50) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UserId VARCHAR(255),
    AdditionalData TEXT,
    CONSTRAINT fk_turbineevents_turbine FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Verify TurbineEvents table created
SELECT 'TurbineEvents table created successfully' as status;

-- Table: ComponentStatuses
CREATE TABLE ComponentStatuses (
    Id UUID PRIMARY KEY,
    TurbineId UUID NOT NULL,
    ComponentType VARCHAR(50) NOT NULL,
    ComponentName VARCHAR(255) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    HealthPercentage DOUBLE PRECISION NOT NULL,
    LastUpdated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT,
    AdditionalData TEXT,
    CONSTRAINT fk_componentstatuses_turbine FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Verify ComponentStatuses table created
SELECT 'ComponentStatuses table created successfully' as status;

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
    Priority INTEGER NOT NULL DEFAULT 1,
    Title VARCHAR(255),
    CONSTRAINT fk_maintenanceschedules_turbine FOREIGN KEY (TurbineId)
        REFERENCES Turbines(Id) ON DELETE CASCADE
);

-- Verify MaintenanceSchedules table created
SELECT 'MaintenanceSchedules table created successfully' as status;

-- ============================================================================
-- CREATE INDEXES
-- ============================================================================

CREATE INDEX IX_TurbineReadings_TurbineId ON TurbineReadings(TurbineId);
CREATE INDEX IX_TurbineReadings_Timestamp ON TurbineReadings(Timestamp DESC);
CREATE INDEX IX_TurbineEvents_TurbineId ON TurbineEvents(TurbineId);
CREATE INDEX IX_TurbineEvents_CreatedAt ON TurbineEvents(CreatedAt DESC);
CREATE INDEX IX_ComponentStatuses_TurbineId ON ComponentStatuses(TurbineId);
CREATE INDEX IX_MaintenanceSchedules_TurbineId ON MaintenanceSchedules(TurbineId);
CREATE INDEX IX_MaintenanceSchedules_ScheduledDate ON MaintenanceSchedules(ScheduledDate);

SELECT 'All indexes created successfully' as status;

-- ============================================================================
-- INSERT SEED DATA
-- ============================================================================

-- Insert 3 Sample Turbines
INSERT INTO Turbines (Id, Name, Location, Model, Capacity, Status, CreatedAt, IsActive) VALUES
('11111111-1111-1111-1111-111111111111', 'Turbine Alpha-01', 'Jakarta Coastal Wind Farm', 'WTG-2000', 2.5, 'Offline', CURRENT_TIMESTAMP, true),
('22222222-2222-2222-2222-222222222222', 'Turbine Beta-02', 'Jakarta Coastal Wind Farm', 'WTG-2000', 2.5, 'Running', CURRENT_TIMESTAMP, true),
('33333333-3333-3333-3333-333333333333', 'Turbine Gamma-03', 'Jakarta Coastal Wind Farm', 'WTG-3000', 3.5, 'Running', CURRENT_TIMESTAMP, true);

SELECT '3 Turbines inserted successfully' as status;

-- Insert Initial Events for each turbine
INSERT INTO TurbineEvents (Id, TurbineId, EventType, Message, Severity, CreatedAt) VALUES
('aaaaaaaa-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'System', 'Turbine registered and initialized', 'Success', CURRENT_TIMESTAMP),
('bbbbbbbb-2222-2222-2222-222222222222', '22222222-2222-2222-2222-222222222222', 'System', 'Turbine started successfully', 'Success', CURRENT_TIMESTAMP),
('cccccccc-3333-3333-3333-333333333333', '33333333-3333-3333-3333-333333333333', 'System', 'Turbine started successfully', 'Success', CURRENT_TIMESTAMP);

SELECT '3 Events inserted successfully' as status;

-- Insert Component Statuses for each turbine
-- Turbine Alpha-01
INSERT INTO ComponentStatuses (Id, TurbineId, ComponentType, ComponentName, Status, HealthPercentage, LastUpdated, Description) VALUES
('10000000-0001-0001-0001-000000000001', '11111111-1111-1111-1111-111111111111', 'Tower', 'Main Tower', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0001-0001-0001-000000000002', '11111111-1111-1111-1111-111111111111', 'Nacelle', 'Nacelle', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0001-0001-0001-000000000003', '11111111-1111-1111-1111-111111111111', 'RotorBlade', 'Rotor Blades', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0001-0001-0001-000000000004', '11111111-1111-1111-1111-111111111111', 'Gearbox', 'Gearbox', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0001-0001-0001-000000000005', '11111111-1111-1111-1111-111111111111', 'Generator', 'Generator', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition');

-- Turbine Beta-02
INSERT INTO ComponentStatuses (Id, TurbineId, ComponentType, ComponentName, Status, HealthPercentage, LastUpdated, Description) VALUES
('10000000-0002-0002-0002-000000000001', '22222222-2222-2222-2222-222222222222', 'Tower', 'Main Tower', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0002-0002-0002-000000000002', '22222222-2222-2222-2222-222222222222', 'Nacelle', 'Nacelle', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0002-0002-0002-000000000003', '22222222-2222-2222-2222-222222222222', 'RotorBlade', 'Rotor Blades', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0002-0002-0002-000000000004', '22222222-2222-2222-2222-222222222222', 'Gearbox', 'Gearbox', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0002-0002-0002-000000000005', '22222222-2222-2222-2222-222222222222', 'Generator', 'Generator', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition');

-- Turbine Gamma-03
INSERT INTO ComponentStatuses (Id, TurbineId, ComponentType, ComponentName, Status, HealthPercentage, LastUpdated, Description) VALUES
('10000000-0003-0003-0003-000000000001', '33333333-3333-3333-3333-333333333333', 'Tower', 'Main Tower', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0003-0003-0003-000000000002', '33333333-3333-3333-3333-333333333333', 'Nacelle', 'Nacelle', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0003-0003-0003-000000000003', '33333333-3333-3333-3333-333333333333', 'RotorBlade', 'Rotor Blades', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0003-0003-0003-000000000004', '33333333-3333-3333-3333-333333333333', 'Gearbox', 'Gearbox', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition'),
('10000000-0003-0003-0003-000000000005', '33333333-3333-3333-3333-333333333333', 'Generator', 'Generator', 'Healthy', 100.0, CURRENT_TIMESTAMP, 'Component in excellent condition');

SELECT '15 Component Statuses inserted successfully' as status;

-- ============================================================================
-- VERIFICATION QUERIES
-- ============================================================================

-- Display all tables
SELECT '=== ALL TABLES ===' as info;
SELECT
    schemaname,
    tablename
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY tablename;

-- Count records in each table
SELECT '=== RECORD COUNTS ===' as info;
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
SELECT '=== ALL TURBINES ===' as info;
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
SELECT '========================================' as info;
SELECT 'DATABASE SETUP COMPLETED SUCCESSFULLY!' as info;
SELECT 'Database: WindTurbineMonitor_Dev' as info;
SELECT 'Tables: 5' as info;
SELECT 'Seed Data: 3 Turbines, 3 Events, 15 Component Statuses' as info;
SELECT '========================================' as info;
