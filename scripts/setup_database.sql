-- Wind Turbine Monitor - Database Setup Script
-- Run this script in pgAdmin 4

-- 1. Create Database
CREATE DATABASE WindTurbineMonitor_Dev;

-- 2. Connect to the new database
\c WindTurbineMonitor_Dev;

-- 3. Verify connection
SELECT current_database(), current_user;

-- Note: Tables will be created automatically by Entity Framework migrations
-- when you run the application for the first time.
