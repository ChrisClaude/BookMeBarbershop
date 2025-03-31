'use client';
import React, { useState, useLayoutEffect } from 'react';

interface RippleProps {
  duration?: number;
  color?: string;
}

const Ripple: React.FC<RippleProps> = ({ duration = 850, color = '#ffffff' }) => {
  const [rippleArray, setRippleArray] = useState<Array<{ x: number; y: number; size: number }>>([]);

  useLayoutEffect(() => {
    if (rippleArray.length > 0) {
      const timer = setTimeout(() => {
        setRippleArray([]);
      }, duration);

      return () => clearTimeout(timer);
    }
  }, [rippleArray.length, duration]);

  const addRipple = (event: React.MouseEvent<HTMLDivElement>) => {
    const rippleContainer = event.currentTarget.getBoundingClientRect();
    const size = Math.max(rippleContainer.width, rippleContainer.height);
    const x = event.clientX - rippleContainer.x - size / 2;
    const y = event.clientY - rippleContainer.y - size / 2;

    const newRipple = {
      x,
      y,
      size,
    };

    setRippleArray([...rippleArray, newRipple]);
  };

  return (
    <div
      className="absolute inset-0 overflow-hidden"
      onMouseDown={addRipple}
    >
      {rippleArray.map((ripple, index) => (
        <span
          key={index}
          style={{
            position: 'absolute',
            left: ripple.x,
            top: ripple.y,
            width: ripple.size,
            height: ripple.size,
            transform: 'scale(0)',
            borderRadius: '50%',
            backgroundColor: color,
            opacity: '0.4',
            animation: `ripple ${duration}ms linear`,
          }}
        />
      ))}
    </div>
  );
};

export default Ripple;