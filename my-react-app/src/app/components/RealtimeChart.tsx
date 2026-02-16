import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';

interface DataPoint {
  time: string;
  value: number;
}

interface RealtimeChartProps {
  data: DataPoint[];
  title: string;
  unit: string;
  color: string;
  maxValue?: number;
}

export function RealtimeChart({ data, title, unit, color, maxValue }: RealtimeChartProps) {
  return (
    <div className="bg-white rounded-lg shadow-lg p-4 border border-slate-200">
      <div className="mb-3">
        <h3 className="text-[#1A2238] font-semibold text-lg">{title}</h3>
        <div className="flex items-baseline gap-2 mt-1">
          <span className="text-3xl font-bold" style={{ color }}>
            {data.length > 0 ? data[data.length - 1].value.toFixed(1) : '0.0'}
          </span>
          <span className="text-slate-500 text-sm font-medium">{unit}</span>
        </div>
      </div>
      
      <ResponsiveContainer width="100%" height={180}>
        <LineChart data={data} margin={{ top: 5, right: 10, left: 0, bottom: 5 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#E5E7EB" />
          <XAxis 
            dataKey="time" 
            stroke="#6B7280" 
            style={{ fontSize: '12px' }}
            tickLine={false}
          />
          <YAxis 
            stroke="#6B7280" 
            style={{ fontSize: '12px' }}
            tickLine={false}
            domain={[0, maxValue || 'auto']}
          />
          <Tooltip 
            contentStyle={{ 
              backgroundColor: '#1A2238', 
              border: 'none', 
              borderRadius: '8px',
              padding: '8px 12px',
              color: '#F5F5F5'
            }}
            labelStyle={{ color: '#F5F5F5', fontWeight: 'bold' }}
            formatter={(value: number) => [`${value.toFixed(2)} ${unit}`, title]}
          />
          <Line 
            type="monotone" 
            dataKey="value" 
            stroke={color} 
            strokeWidth={2.5}
            dot={false}
            animationDuration={300}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}
