"use client";
import React, { useState, useEffect, useCallback, useRef } from "react";
import Image from "next/image";
import { RiCloseLine, RiZoomInLine, RiZoomOutLine } from "react-icons/ri";

interface PhotoViewProps {
  isOpen: boolean;
  onClose: () => void;
  src: string;
  alt: string;
  title?: string;
}

const PhotoView: React.FC<PhotoViewProps> = ({
  isOpen,
  onClose,
  src,
  alt,
  title,
}) => {
  const [scale, setScale] = useState(1);
  const [position, setPosition] = useState({ x: 0, y: 0 });
  const [isDragging, setIsDragging] = useState(false);
  const dragStartRef = useRef({ x: 0, y: 0 });

  // Reset state when modal is closed
  useEffect(() => {
    if (!isOpen) {
      setScale(1);
      setPosition({ x: 0, y: 0 });
      setIsDragging(false);
    }
  }, [isOpen]);

  // Handle escape key to close modal
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape" && isOpen) {
        onClose();
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [isOpen, onClose]);

  // Zoom in function
  const zoomIn = useCallback(() => {
    setScale((prevScale) => Math.min(prevScale + 0.5, 4));
  }, []);

  // Zoom out function
  const zoomOut = useCallback(() => {
    setScale((prevScale) => Math.max(prevScale - 0.5, 0.5));
  }, []);

  // Mouse event handlers for dragging
  const handleMouseDown = useCallback(
    (e: React.MouseEvent) => {
      if (scale > 1) {
        e.preventDefault();
        setIsDragging(true);
        dragStartRef.current = {
          x: e.clientX - position.x,
          y: e.clientY - position.y,
        };
      }
    },
    [scale, position]
  );

  const handleMouseMove = useCallback(
    (e: MouseEvent) => {
      if (!isDragging || scale <= 1) return;

      setPosition({
        x: e.clientX - dragStartRef.current.x,
        y: e.clientY - dragStartRef.current.y,
      });
    },
    [isDragging, scale]
  );

  const handleMouseUp = useCallback(() => {
    setIsDragging(false);
  }, []);

  // Touch event handlers for mobile dragging
  const handleTouchStart = useCallback(
    (e: React.TouchEvent) => {
      if (scale > 1) {
        setIsDragging(true);
        dragStartRef.current = {
          x: e.touches[0].clientX - position.x,
          y: e.touches[0].clientY - position.y,
        };
      }
    },
    [scale, position]
  );

  const handleTouchMove = useCallback(
    (e: TouchEvent) => {
      if (!isDragging || scale <= 1) return;

      e.preventDefault(); // Prevent screen scrolling while dragging
      setPosition({
        x: e.touches[0].clientX - dragStartRef.current.x,
        y: e.touches[0].clientY - dragStartRef.current.y,
      });
    },
    [isDragging, scale]
  );

  const handleTouchEnd = useCallback(() => {
    setIsDragging(false);
  }, []);

  // Set up and clean up global event listeners
  useEffect(() => {
    // Only add listeners when dragging is active
    if (isDragging) {
      // Add document-level event listeners
      document.addEventListener("mousemove", handleMouseMove);
      document.addEventListener("mouseup", handleMouseUp);
      document.addEventListener("touchmove", handleTouchMove as EventListener, {
        passive: false,
      });
      document.addEventListener("touchend", handleTouchEnd as EventListener);
    }

    // Clean up function
    return () => {
      document.removeEventListener("mousemove", handleMouseMove);
      document.removeEventListener("mouseup", handleMouseUp);
      document.removeEventListener(
        "touchmove",
        handleTouchMove as EventListener
      );
      document.removeEventListener("touchend", handleTouchEnd as EventListener);
    };
  }, [
    isDragging,
    handleMouseMove,
    handleMouseUp,
    handleTouchMove,
    handleTouchEnd,
  ]);

  // Handle click on backdrop to close modal
  const handleBackdropClick = useCallback(
    (e: React.MouseEvent) => {
      if (e.target === e.currentTarget) {
        onClose();
      }
    },
    [onClose]
  );

  if (!isOpen) return null;

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-80 transition-opacity duration-300 photo-view-enter"
      onClick={handleBackdropClick}
    >
      <div className="relative w-full h-full max-w-4xl max-h-[90vh] mx-auto flex flex-col">
        {/* Header with title and close button */}
        <div className="flex justify-between items-center p-4 text-white">
          {title && <h3 className="text-xl font-semibold">{title}</h3>}
          <div className="flex gap-4">
            <button
              onClick={zoomOut}
              className="p-2 rounded-full hover:bg-gray-800 transition-colors"
              aria-label="Zoom out"
            >
              <RiZoomOutLine size={24} />
            </button>
            <button
              onClick={zoomIn}
              className="p-2 rounded-full hover:bg-gray-800 transition-colors"
              aria-label="Zoom in"
            >
              <RiZoomInLine size={24} />
            </button>
            <button
              onClick={onClose}
              className="p-2 rounded-full hover:bg-gray-800 transition-colors"
              aria-label="Close"
            >
              <RiCloseLine size={24} />
            </button>
          </div>
        </div>

        {/* Image container */}
        <div
          className={`relative flex-grow overflow-hidden ${
            scale > 1
              ? isDragging
                ? "cursor-grabbing"
                : "cursor-grab"
              : "cursor-default"
          }`}
          onMouseDown={handleMouseDown}
          onTouchStart={handleTouchStart}
        >
          <div
            className="absolute w-full h-full transition-transform duration-200"
            style={{
              transform: `translate(${position.x}px, ${position.y}px) scale(${scale})`,
              transformOrigin: "center",
            }}
          >
            <div className="relative w-full h-full">
              <Image
                src={src}
                alt={alt}
                fill
                className="object-contain"
                sizes="(max-width: 768px) 100vw, (max-width: 1200px) 80vw, 70vw"
                priority
                draggable={false}
              />
            </div>
          </div>
        </div>

        {/* Instructions */}
        <div className="text-white text-center p-2 text-sm">
          {scale > 1
            ? "Click and drag to move the image"
            : "Use the zoom buttons to zoom in and out"}
        </div>
      </div>
    </div>
  );
};

export default PhotoView;
