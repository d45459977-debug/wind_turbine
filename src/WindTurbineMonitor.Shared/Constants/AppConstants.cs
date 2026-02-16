namespace WindTurbineMonitor.Shared.Constants;

/// <summary>
/// Konstanta untuk aplikasi
/// </summary>
public static class AppConstants
{
    /// <summary>
    /// Konstanta untuk warna aplikasi
    /// </summary>
    public static class Colors
    {
        // Primary colors - Deep Navy Blue
        public const string PrimaryDark = "#1a2332";
        public const string PrimaryMedium = "#2d3a4a";
        public const string PrimaryLight = "#3d4a5a";

        // Background - Clean White/Light Grey
        public const string BackgroundPrimary = "#ffffff";
        public const string BackgroundSecondary = "#f8f9fa";
        public const string BackgroundTertiary = "#e9ecef";

        // Accent - Electric Blue
        public const string AccentBlue = "#2563eb";
        public const string AccentBlueHover = "#1d4ed8";
        public const string AccentBlueLight = "#dbeafe";

        // Status colors
        public const string StatusOn = "#10b981";        // Emerald Green
        public const string StatusOff = "#ef4444";       // Ruby Red
        public const string StatusWarning = "#f59e0b";
        public const string StatusInfo = "#3b82f6";
        public const string StatusSuccess = "#10b981";
        public const string StatusError = "#ef4444";
        public const string StatusCritical = "#dc2626";

        // Dark mode colors
        public const string DarkBgPrimary = "#1a1a2e";
        public const string DarkBgSecondary = "#16213e";
        public const string DarkBgTertiary = "#0f3460";
        public const string DarkTextPrimary = "#eaeaea";
        public const string DarkTextSecondary = "#b0b0b0";
    }

    /// <summary>
    /// Konstanta untuk SignalR
    /// </summary>
    public static class SignalR
    {
        public const string HubName = "/turbinehub";
        public const string ReceiveDataMethod = "ReceiveTurbineData";
        public const string ReceiveEventMethod = "ReceiveTurbineEvent";
        public const string UpdateStatusMethod = "UpdateTurbineStatus";

        public const int DefaultUpdateIntervalMs = 2000; // 2 seconds
        public const int MinUpdateIntervalMs = 500;      // 0.5 seconds
        public const int MaxUpdateIntervalMs = 10000;    // 10 seconds
    }

    /// <summary>
    /// Konstanta untuk sensor
    /// </summary>
    public static class Sensor
    {
        public const double MinWindSpeedMs = 0;
        public const double MaxWindSpeedMs = 30;
        public const double CutInWindSpeedMs = 3;
        public const double RatedWindSpeedMs = 12;
        public const double CutOutWindSpeedMs = 25;

        public const double MinRotorSpeedRpm = 0;
        public const double MaxRotorSpeedRpm = 25;
        public const double RatedRotorSpeedRpm = 15;

        public const double MinEfficiencyPercent = 0;
        public const double MaxEfficiencyPercent = 100;
    }

    /// <summary>
    /// Konstanta untuk notifikasi
    /// </summary>
    public static class Notifications
    {
        public const string TurbineStarted = "Turbin berhasil di-start";
        public const string TurbineStopped = "Turbin berhasil di-stop";
        public const string TurbineError = "Terjadi error pada turbin";
        public const string MaintenanceRequired = "Maintenance diperlukan";
        public const string HighVibration = "Level vibrasi tinggi terdeteksi";
        public const string HighTemperature = "Suhu komponen tinggi terdeteksi";
        public const string LowEfficiency = "Efisiensi turbin rendah";
    }
}
