import { Power } from "lucide-react";
import { motion } from "motion/react";

interface PowerButtonProps {
  isActive: boolean;
  onToggle: () => void;
}

export function PowerButton({ isActive, onToggle }: PowerButtonProps) {
  return (
    <motion.button
      onClick={onToggle}
      className={`relative px-8 py-4 rounded-lg font-bold text-lg shadow-lg transition-all duration-300 flex items-center gap-3 ${
        isActive 
          ? 'bg-[#2ECC71] hover:bg-[#27AE60] text-white shadow-[0_0_20px_rgba(46,204,113,0.4)]' 
          : 'bg-[#E74C3C] hover:bg-[#C0392B] text-white shadow-[0_0_20px_rgba(231,76,60,0.4)]'
      }`}
      whileHover={{ scale: 1.05 }}
      whileTap={{ scale: 0.95 }}
    >
      <motion.div
        animate={isActive ? { rotate: 180 } : { rotate: 0 }}
        transition={{ duration: 0.3 }}
      >
        <Power className="w-6 h-6" />
      </motion.div>
      <span>MASTER POWER {isActive ? 'ON' : 'OFF'}</span>
      
      {/* Pulse Effect */}
      {isActive && (
        <motion.div
          className="absolute inset-0 rounded-lg bg-[#2ECC71]"
          initial={{ opacity: 0.6, scale: 1 }}
          animate={{ opacity: 0, scale: 1.2 }}
          transition={{ duration: 1.5, repeat: Infinity }}
        />
      )}
    </motion.button>
  );
}
