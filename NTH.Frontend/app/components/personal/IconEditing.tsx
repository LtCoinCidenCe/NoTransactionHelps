import { useContext, useEffect, useRef, useState } from "react";
import UserContext from "../provider/UserContext";
import JWTContext from "../provider/JWTContext";
import ErrorContext from "../provider/ErrorContext";
import zod from "zod";

const CROP_SIZE = 250;

const IconEditing: React.FC = () => {
  const errorContext = useContext(ErrorContext);
  const { currentUser, updateCurrentUser } = useContext(UserContext);
  const { jwt } = useContext(JWTContext);
  const [originalBitmap, setOriginalBitmap] = useState<ImageBitmap>();
  const [useSize, setUseSize] = useState(CROP_SIZE);
  const [position, setPosition] = useState({ x: 0, y: 0 });

  const [isDragging, setIsDragging] = useState(false);
  const [isIconOpen, setIsIconOpen] = useState(false);

  const canvasRef = useRef<HTMLCanvasElement>(null);

  const iconBackgroundImageStyle = currentUser.userIconID === "00000000-0000-0000-0000-000000000000"
    ? undefined
    : `${import.meta.env.VITE_BACKEND_URL}/api/User/Icon/${currentUser.userIconID}`;

  useEffect(() => {
    if (!originalBitmap || !canvasRef.current) return;
    const ctx = canvasRef.current.getContext("2d");
    if (!ctx) return;
    // TODO
    // console.log("useSize", useSize);
    ctx.drawImage(originalBitmap, position.x, position.y, useSize, useSize, 0, 0, CROP_SIZE, CROP_SIZE);
  }, [originalBitmap, useSize, position]); // useEffect

  return <div className="flex w-full gap-5 items-center my-5">
    <input className="w-24 h-24 rounded-full self-start text-black/0 bg-gray-300 bg-contain hover:border-red-500 hover:border-dashed hover:border-2"
      type="file"
      style={{
        backgroundImage: iconBackgroundImageStyle ? `url(${iconBackgroundImageStyle})` : undefined
      }}
      accept="image/*"
      onChange={async (e) => {
        const file = e.target.files?.[0];
        if (!file) return;
        let dotbitmap;
        try { dotbitmap = await createImageBitmap(file); }
        catch (error) {
          alert("无法加载图片，请确保文件是有效的图片格式");
          return;
        }
        if (dotbitmap.width < 200 || dotbitmap.height < 200) {
          alert("请上传至少200x200像素的图片");
          return;
        }
        setOriginalBitmap(dotbitmap);
        setPosition({ x: 0, y: 0 });
        setUseSize(Math.min(dotbitmap.width, dotbitmap.height));
        setIsDragging(false);
      }}
    />

    <button className="px-4 h-12 bg-gray-300 hover:bg-gray-200 rounded-lg"
      onClick={() => setIsIconOpen(true)}>修改头像</button>

    {originalBitmap && // 黑幕
      <div className="fixed inset-0 z-35 bg-black/36 flex justify-center items-center"
        onClick={() => setOriginalBitmap(undefined)}>
        <div className="bg-white rounded-3xl"
          onClick={ // 白幕防退出区
            e => e.stopPropagation()
          }>
          <div className="m-15 lg:m-25 flex flex-col items-center">
            <canvas className="w-50 lg:w-75 cursor-grab active:cursor-grabbing border border-gray-300"
              ref={canvasRef}
              width={CROP_SIZE}
              height={CROP_SIZE}
              onMouseDown={e => {
                setIsDragging(true);
              }}
              onMouseMove={e => {
                if (!isDragging) return;
                console.log("moving", e.movementX, e.movementY, useSize);
                const scale = Math.floor(1 + useSize / CROP_SIZE);
                const newX = Math.min(Math.max(0, position.x - e.movementX * scale), originalBitmap.width - useSize);
                const newY = Math.min(Math.max(0, position.y - e.movementY * scale), originalBitmap.height - useSize);
                setPosition({ x: newX, y: newY });
              }}
              onMouseUp={e => {
                setIsDragging(false);
              }}
              onMouseLeave={e => {
                setIsDragging(false);
              }}
              onWheel={e => {
                e.preventDefault();
                const delta = Math.floor(e.deltaY > 0 ? - useSize / 16 : useSize / 16);
                const mapShort = Math.min(originalBitmap.width, originalBitmap.height);
                const targetUseSize = Math.min(Math.max(30, useSize + delta), mapShort);
                const newPos = position;
                if (position.x + targetUseSize > originalBitmap.width)
                  newPos.x = Math.max(0, originalBitmap.width - targetUseSize);
                if (position.y + targetUseSize > originalBitmap.height)
                  newPos.y = Math.max(0, originalBitmap.height - targetUseSize);
                setPosition(newPos);
                setUseSize(targetUseSize);
              }}
            />
            <button className="mt-5 py-5 px-15 bg-purple-300 hover:bg-purple-300/50 rounded-lg"
              onClick={e => {
                if (!canvasRef.current) return;
                canvasRef.current.toBlob(async blob => {
                  if (!blob) return;
                  // 1. 创建 FormData（对应 curl -F）
                  const formData = new FormData();
                  // 2. 把 Blob 作为文件放进去
                  // 文件名随便填（后端一般不用），type 固定 image/png
                  formData.append(
                    "icon",                // 后端接收的字段名（完全匹配你的curl）
                    blob,                  // 剪切好的图片Blob
                    "newIcon.png"           // 文件名（可自定义）
                  );
                  const userIconURL = `${import.meta.env.VITE_BACKEND_URL}/api/User/${currentUser.id}/Icon`;
                  const response = await fetch(userIconURL, {
                    method: "PUT",
                    headers: { "Authorization": `Bearer ${jwt}` },
                    // 注意：不要手动写 Content-Type: multipart/form-data
                    // 浏览器会自动生成，否则会丢失 boundary 导致上传失败
                    body: formData
                  });
                  if (!response.ok) {
                    switch (response.status) {
                      case 401:
                        errorContext("你无权这么做。");
                        break;
                      case 400:
                        const errorText = await response.text();
                        errorContext(errorText);
                        break;
                      default:
                        break;
                    }
                    return;
                  }
                  const newGuid = await zod.string().safeParseAsync(await response.json());
                  if (!newGuid.success) {
                    errorContext("api/User/{ID}/Icon 变更，请联系开发者");
                  }
                  else
                    updateCurrentUser({ ...currentUser, userIconID: newGuid.data });
                  setOriginalBitmap(undefined);
                  setIsDragging(false);
                  return;
                },
                  "image/png",
                  1);
              }}
            >保存</button>
          </div>
        </div>
      </div>}
  </div>;
};

export default IconEditing;
