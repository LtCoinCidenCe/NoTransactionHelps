import React, { useState, useRef, useEffect } from "react";

interface AvatarCropProps {
  onCropComplete: (croppedBlob: Blob) => void;
}

const AvatarCrop: React.FC<AvatarCropProps> = ({ onCropComplete }) => {
  const [originalImage, setOriginalImage] = useState<string | null>(null);
  const [originalFile, setOriginalFile] = useState<File | null>(null);
  const [originalBitmap, setOriginalBitmap] = useState<ImageBitmap | null>(null);

  const [heidth, setHeidth] = useState(200);

  const [scale, setScale] = useState(1);
  const [position, setPosition] = useState({ x: 0, y: 0 });

  const [isDragging, setIsDragging] = useState(false);
  const [startPos, setStartPos] = useState({ x: 0, y: 0 });

  const canvasRef = useRef<HTMLCanvasElement>(null);
  const imageRef = useRef<HTMLImageElement | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  // 固定裁剪尺寸：你要的正方形头像
  const CROP_SIZE = 200;

  // 选择图片
  const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
    console.log("debug");
    const file = e.target.files?.[0];
    if (!file) return;
    setOriginalFile(file);
    const dotbitmap = await createImageBitmap(file);
    if (dotbitmap.width < 200 || dotbitmap.height < 200) {
      alert("请上传至少200x200像素的图片");
      return;
    }
    setOriginalBitmap(dotbitmap);
    setHeidth(200);
    setPosition({ x: 0, y: 0 });

    const reader = new FileReader();
    reader.onload = (ev) => {
      const img = new Image();
      img.onload = () => {
        imageRef.current = img;
        setOriginalImage(ev.target?.result as string);
        setPosition({ x: 0, y: 0 });
        setScale(1);
      };
      img.src = ev.target?.result as string;
      // console.log("imgsrc", img.src); // base64encoded
    };
    reader.readAsDataURL(file);
  };

  // 鼠标/触摸拖动
  const handleMouseDown = (e: React.MouseEvent) => {
    console.log("position", position);
    console.log("client", { x: e.clientX, y: e.clientY });

    setIsDragging(true);
    setStartPos({ x: position.x - e.clientX, y: position.y - e.clientY });
  };

  const handleMouseMove = (e: React.MouseEvent) => {
    if (!isDragging) return;
    setPosition({
      x: Math.min(0, e.clientX + startPos.x),
      y: Math.min(0, e.clientY + startPos.y),
    });
  };

  const handleMouseUp = () => {
    console.log("startPos", startPos);
    setIsDragging(false)
  };

  // 滚轮缩放
  const handleWheel = (e: React.WheelEvent) => {
    e.preventDefault();
    setScale((s) => Math.min(4, Math.max(0.25, s - e.deltaY * 0.0005)));
  };

  // 绘制裁剪画布
  useEffect(() => {
    (async () => {
      if (!originalImage || !canvasRef.current || !imageRef.current || !originalBitmap) return;
      const canvas = canvasRef.current;
      const ctx = canvas.getContext("2d");
      if (!ctx) return;

      // ctx.clearRect(0, 0, CROP_SIZE, CROP_SIZE);

      // // 绘制灰色遮罩
      // ctx.fillStyle = "rgba(0,0,0,0.4)";
      // ctx.fillRect(0, 0, CROP_SIZE, CROP_SIZE);

      // // 绘制圆形裁剪洞
      // ctx.save();
      // ctx.beginPath();
      // ctx.rect(0, 0, CROP_SIZE, CROP_SIZE);
      // // ctx.arc(CROP_SIZE / 2, CROP_SIZE / 2, CROP_SIZE / 2, 0, Math.PI * 2);
      // ctx.clip();
      // ctx.clearRect(0, 0, CROP_SIZE, CROP_SIZE);

      // 绘制图片
      ctx.drawImage(
        originalBitmap,
        position.x,
        position.y,
        originalBitmap.width * scale,
        originalBitmap.height * scale
      );
      ctx.restore();
    })();
  }, [originalImage, position, scale]);

  // 确认裁剪 → 输出 Blob 用于上传
  const handleCrop = () => {
    if (!canvasRef.current) return;
    canvasRef.current.toBlob(
      (blob) => {
        if (blob) onCropComplete(blob);
      },
      "image/png",
      1
    );
  };

  return (
    <div className="flex flex-col items-center gap-4 p-4">
      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        // className="hidden"
        onChange={handleFileSelect}
      />

      {/* 上传按钮 */}
      <button
        onClick={() => fileInputRef.current?.click()}
        className="px-4 py-2 bg-gray-100 hover:bg-gray-200 rounded-lg"
      >
        选择头像
      </button>

      {/* 裁剪画布 */}
      {originalImage && (
        <div className="relative">
          <canvas
            ref={canvasRef}
            width={CROP_SIZE}
            height={CROP_SIZE}
            className="cursor-grab active:cursor-grabbing border border-gray-300 rounded-full"
            onMouseDown={handleMouseDown}
            onMouseMove={handleMouseMove}
            onMouseUp={handleMouseUp}
            onMouseLeave={handleMouseUp}
            onWheel={handleWheel}
          />{/* canvas */}
          <div className="flex gap-3 mt-4 justify-center">
            <button
              onClick={handleCrop}
              className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
            >
              确认使用
            </button>
            <button
              onClick={() => setOriginalImage(null)}
              className="px-4 py-2 bg-gray-200 rounded-lg hover:bg-gray-300"
            >
              取消
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default AvatarCrop;
