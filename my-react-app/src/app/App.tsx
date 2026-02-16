import { useState, useEffect } from 'react';
import { TurbineVisualization } from './components/TurbineVisualization';
import { RealtimeChart } from './components/RealtimeChart';
import { PowerButton } from './components/PowerButton';
import { StatusPanel } from './components/StatusPanel';

interface DataPoint {
  time: string;
  value: number;
}

export default function App() {
  const [isActive, setIsActive] = useState(false);
  const [windSpeedData, setWindSpeedData] = useState<DataPoint[]>([]);
  const [rpmData, setRpmData] = useState<DataPoint[]>([]);
  const [powerData, setPowerData] = useState<DataPoint[]>([]);
  
  const [currentWindSpeed, setCurrentWindSpeed] = useState(0);
  const [currentRpm, setCurrentRpm] = useState(0);
  const [currentPower, setCurrentPower] = useState(0);

  const MAX_DATA_POINTS = 20;

  useEffect(() => {
    const interval = setInterval(() => {
      const now = new Date();
      const timeStr = `${now.getHours().toString().padStart(2, '0')}:${now.getMinutes().toString().padStart(2, '0')}:${now.getSeconds().toString().padStart(2, '0')}`;

      if (isActive) {
        // Simulate sensor data with realistic variations
        const baseWindSpeed = 8 + Math.random() * 4; // 8-12 m/s
        const windSpeed = Math.max(0, baseWindSpeed + (Math.random() - 0.5) * 2);
        
        // RPM correlates with wind speed (typical ratio for small turbines)
        const rpm = windSpeed * 15 + (Math.random() - 0.5) * 20; // ~120-180 RPM at 8-12 m/s
        
        // Power output (simplified formula, typically cubic relationship with wind speed)
        const power = Math.pow(windSpeed, 2) * 10 + (Math.random() - 0.5) * 50; // ~640-1440W range

        setCurrentWindSpeed(windSpeed);
        setCurrentRpm(rpm);
        setCurrentPower(power);

        // Update charts
        setWindSpeedData(prev => {
          const newData = [...prev, { time: timeStr, value: windSpeed }];
          return newData.slice(-MAX_DATA_POINTS);
        });

        setRpmData(prev => {
          const newData = [...prev, { time: timeStr, value: rpm }];
          return newData.slice(-MAX_DATA_POINTS);
        });

        setPowerData(prev => {
          const newData = [...prev, { time: timeStr, value: power }];
          return newData.slice(-MAX_DATA_POINTS);
        });
      } else {
        // System off - decay to zero
        setCurrentWindSpeed(prev => Math.max(0, prev * 0.95));
        setCurrentRpm(prev => Math.max(0, prev * 0.9));
        setCurrentPower(0);

        setWindSpeedData(prev => {
          const newData = [...prev, { time: timeStr, value: 0 }];
          return newData.slice(-MAX_DATA_POINTS);
        });

        setRpmData(prev => {
          const newData = [...prev, { time: timeStr, value: 0 }];
          return newData.slice(-MAX_DATA_POINTS);
        });

        setPowerData(prev => {
          const newData = [...prev, { time: timeStr, value: 0 }];
          return newData.slice(-MAX_DATA_POINTS);
        });
      }
    }, 1000); // Update every second

    return () => clearInterval(interval);
  }, [isActive]);

  return (
    <div className="size-full bg-[#F5F5F5] overflow-hidden">
      {/* Header */}
      <header className="bg-[#1A2238] text-white shadow-lg border-b-2 border-[#007BFF]">
        <div className="px-8 py-4">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold mb-1">Wind Turbine HMI</h1>
              <p className="text-slate-400 text-sm">Integrated Monitoring & Control System</p>
            </div>
            <PowerButton isActive={isActive} onToggle={() => setIsActive(!isActive)} />
          </div>
        </div>
      </header>

      {/* Status Panel */}
      <div className="px-8 py-4">
        <StatusPanel 
          isActive={isActive}
          windSpeed={currentWindSpeed}
          rpm={currentRpm}
          power={currentPower}
        />
      </div>

      {/* Main Content */}
      <div className="flex gap-6 px-8 pb-8" style={{ height: 'calc(100vh - 220px)' }}>
        {/* Left Panel - Digital Twin */}
        <div className="flex-1 bg-white rounded-lg shadow-xl border border-slate-200 overflow-hidden">
          <div className="bg-[#1A2238] text-white px-4 py-2 border-b-2 border-[#007BFF]">
            <h2 className="font-bold text-sm tracking-wide">DIGITAL TWIN VISUALIZATION</h2>
          </div>
          <div className="h-full">
            <TurbineVisualization rpm={currentRpm} isActive={isActive} />
          </div>
        </div>

        {/* Right Panel - Data Analytics */}
        <div className="flex-1 flex flex-col gap-4 overflow-auto">
          <div className="bg-[#1A2238] text-white px-4 py-2 rounded-t-lg border-b-2 border-[#007BFF]">
            <h2 className="font-bold text-sm tracking-wide">REAL-TIME DATA ANALYTICS</h2>
          </div>
          
          <RealtimeChart
            data={windSpeedData}
            title="Wind Speed"
            unit="m/s"
            color="#007BFF"
            maxValue={15}
          />
          
          <RealtimeChart
            data={rpmData}
            title="Rotor Speed"
            unit="RPM"
            color="#007BFF"
            maxValue={200}
          />
          
          <RealtimeChart
            data={powerData}
            title="Power Output"
            unit="W"
            color="#2ECC71"
            maxValue={2000}
          />
        </div>
      </div>
    </div>
  );
}
