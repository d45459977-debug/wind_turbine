import { Activity, Zap, Wind, AlertCircle } from "lucide-react";

interface StatusPanelProps {
  isActive: boolean;
  windSpeed: number;
  rpm: number;
  power: number;
}

export function StatusPanel({ isActive, windSpeed, rpm, power }: StatusPanelProps) {
  const statusItems = [
    {
      icon: Activity,
      label: "System Status",
      value: isActive ? "OPERATIONAL" : "STANDBY",
      color: isActive ? "#2ECC71" : "#E74C3C"
    },
    {
      icon: Wind,
      label: "Wind Speed",
      value: `${windSpeed.toFixed(1)} m/s`,
      color: "#007BFF"
    },
    {
      icon: Activity,
      label: "Rotor Speed",
      value: `${rpm.toFixed(1)} RPM`,
      color: "#007BFF"
    },
    {
      icon: Zap,
      label: "Power Output",
      value: `${power.toFixed(0)} W`,
      color: "#2ECC71"
    }
  ];

  return (
    <div className="grid grid-cols-4 gap-4">
      {statusItems.map((item, index) => (
        <div 
          key={index}
          className="bg-white rounded-lg shadow-md p-4 border-l-4 hover:shadow-lg transition-shadow"
          style={{ borderLeftColor: item.color }}
        >
          <div className="flex items-start justify-between mb-2">
            <item.icon className="w-5 h-5" style={{ color: item.color }} />
            {index === 0 && !isActive && (
              <AlertCircle className="w-4 h-4 text-[#E74C3C] animate-pulse" />
            )}
          </div>
          <div className="text-slate-500 text-xs font-medium mb-1">{item.label}</div>
          <div className="text-[#1A2238] text-lg font-bold">{item.value}</div>
        </div>
      ))}
    </div>
  );
}
