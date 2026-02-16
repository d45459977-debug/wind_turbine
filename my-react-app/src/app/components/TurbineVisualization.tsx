import { motion } from "motion/react";
import { Wind } from "lucide-react";

interface TurbineVisualizationProps {
  rpm: number;
  isActive: boolean;
}

export function TurbineVisualization({ rpm, isActive }: TurbineVisualizationProps) {
  // Calculate rotation duration based on RPM
  const rotationDuration = isActive && rpm > 0 ? 60 / rpm : 0;

  return (
    <div className="w-full h-full flex items-center justify-center bg-gradient-to-br from-slate-100 to-slate-200 relative overflow-hidden">
      {/* Background Grid */}
      <div className="absolute inset-0 opacity-10">
        <svg width="100%" height="100%" xmlns="http://www.w3.org/2000/svg">
          <defs>
            <pattern id="grid" width="40" height="40" patternUnits="userSpaceOnUse">
              <path d="M 40 0 L 0 0 0 40" fill="none" stroke="#1A2238" strokeWidth="0.5"/>
            </pattern>
          </defs>
          <rect width="100%" height="100%" fill="url(#grid)" />
        </svg>
      </div>

      {/* Turbine Container */}
      <div className="relative z-10">
        {/* Tower */}
        <div className="flex flex-col items-center">
          {/* Nacelle (Housing) */}
          <div className="relative mb-2">
            <div className="w-32 h-24 bg-gradient-to-b from-slate-600 to-slate-700 rounded-2xl shadow-2xl border-2 border-slate-500 relative">
              {/* Status Indicator */}
              <div className="absolute top-2 right-2">
                <div className={`w-3 h-3 rounded-full ${isActive ? 'bg-[#2ECC71] shadow-[0_0_10px_#2ECC71]' : 'bg-[#E74C3C] shadow-[0_0_10px_#E74C3C]'} animate-pulse`}></div>
              </div>
              
              {/* Center Hub */}
              <div className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2">
                <div className="w-16 h-16 bg-gradient-to-br from-slate-700 to-slate-800 rounded-full shadow-xl border-4 border-slate-600 flex items-center justify-center">
                  {/* Rotating Blades */}
                  <motion.div
                    className="relative w-full h-full"
                    animate={isActive ? { rotate: 360 } : {}}
                    transition={{
                      duration: rotationDuration > 0 ? rotationDuration : 999,
                      repeat: isActive ? Infinity : 0,
                      ease: "linear"
                    }}
                  >
                    {/* Blade 1 */}
                    <div className="absolute left-1/2 top-1/2 -translate-x-1/2 origin-bottom" style={{ transform: 'translate(-50%, -50%) rotate(0deg)' }}>
                      <div className="w-3 h-32 bg-gradient-to-t from-white to-slate-300 rounded-t-full shadow-lg" style={{ transformOrigin: 'bottom center', transform: 'translateY(-50%)' }}></div>
                    </div>
                    {/* Blade 2 */}
                    <div className="absolute left-1/2 top-1/2 -translate-x-1/2" style={{ transform: 'translate(-50%, -50%) rotate(120deg)' }}>
                      <div className="w-3 h-32 bg-gradient-to-t from-white to-slate-300 rounded-t-full shadow-lg" style={{ transformOrigin: 'bottom center', transform: 'translateY(-50%)' }}></div>
                    </div>
                    {/* Blade 3 */}
                    <div className="absolute left-1/2 top-1/2 -translate-x-1/2" style={{ transform: 'translate(-50%, -50%) rotate(240deg)' }}>
                      <div className="w-3 h-32 bg-gradient-to-t from-white to-slate-300 rounded-t-full shadow-lg" style={{ transformOrigin: 'bottom center', transform: 'translateY(-50%)' }}></div>
                    </div>
                  </motion.div>
                  
                  {/* Center Cap */}
                  <div className="absolute w-6 h-6 bg-slate-900 rounded-full border-2 border-slate-700 z-10"></div>
                </div>
              </div>
            </div>
          </div>

          {/* Tower Pole */}
          <div className="w-8 h-48 bg-gradient-to-b from-slate-600 to-slate-700 shadow-xl relative">
            {/* Tower Details */}
            <div className="absolute inset-0 flex flex-col justify-evenly items-center">
              <div className="w-full h-px bg-slate-500"></div>
              <div className="w-full h-px bg-slate-500"></div>
              <div className="w-full h-px bg-slate-500"></div>
            </div>
          </div>

          {/* Base */}
          <div className="w-24 h-8 bg-gradient-to-b from-slate-700 to-slate-800 shadow-2xl relative">
            <div className="absolute -bottom-2 left-1/2 -translate-x-1/2 w-32 h-4 bg-slate-300/50 rounded-full blur-sm"></div>
          </div>
        </div>

        {/* Wind Animation */}
        {isActive && (
          <div className="absolute -left-32 top-1/2 -translate-y-1/2">
            <motion.div
              initial={{ x: 0, opacity: 0.6 }}
              animate={{ x: 200, opacity: 0 }}
              transition={{ duration: 2, repeat: Infinity, ease: "linear" }}
            >
              <Wind className="w-8 h-8 text-[#007BFF]" />
            </motion.div>
          </div>
        )}
      </div>

      {/* RPM Display */}
      <div className="absolute bottom-8 left-1/2 -translate-x-1/2 bg-[#1A2238]/90 backdrop-blur-sm px-6 py-3 rounded-lg border border-[#007BFF]/30">
        <div className="text-[#F5F5F5] text-sm font-medium text-center">
          <div className="text-[#007BFF] text-xs mb-1">ROTOR SPEED</div>
          <div className="text-2xl font-bold">{rpm.toFixed(1)} <span className="text-sm font-normal">RPM</span></div>
        </div>
      </div>
    </div>
  );
}
