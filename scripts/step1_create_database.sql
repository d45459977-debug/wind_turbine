-- ============================================================================
-- STEP 1: Create Database
-- ============================================================================
-- Run this FIRST in pgAdmin Query Tool while connected to 'postgres' database
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

-- ============================================================================
-- SUCCESS! Database created.
-- NOW: Close this query and open a NEW query connected to 'WindTurbineMonitor_Dev'
-- Then run step2_create_tables.sql
-- ============================================================================
