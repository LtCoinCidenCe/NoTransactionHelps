import { useContext, useEffect, useState } from "react";
import { isVideoInfo, type VideoInfo } from "~/types";
import VideoCard from "./VideoCard";
import JWTContext from "../provider/JWTContext";
import ErrorContext from "../provider/ErrorContext";

const VideoGrid: React.FC = () => {
  const errorContext = useContext(ErrorContext);
  const { jwt } = useContext(JWTContext);
  const [videos, setVideos] = useState<VideoInfo[]>([]);
  useEffect(() => {
    const fetchData = async () => {
      const workStartedURL = `${import.meta.env.VITE_BACKEND_URL}/api/Video/WorkStarted`;
      const response = await fetch(workStartedURL, { method: "GET", headers: { "Authorization": `Bearer ${jwt}` } });
      if (!response.ok) {
        errorContext(`${response.status}`);
        return;
      }
      const workStarted = await response.json();
      if (!Array.isArray(workStarted)) {
        errorContext("/api/Video/WorkStarted is not array");
        return;
      }
      if (!workStarted.every(video => isVideoInfo(video))) {
        errorContext("/api/Video/WorkStarted doesn't have valid items");
        return;
      }
      setVideos(workStarted);
    }
    fetchData();
    return;
  }, []); // useEffect
  if (videos.length === 0)
    return <p style={{ color: "#cccccc" }}>Loading Game...</p>;
  else
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-6">
          {videos.map((video) => (
            <VideoCard key={video.id} video={video} />
          ))}
        </div>
      </div>
    );
}

export default VideoGrid;
