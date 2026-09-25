import React from 'react';
import type { VideoInfo } from '~/types';

// 单个视频卡片组件
interface VideoCardProps {
  video: VideoInfo;
}

const VideoCard: React.FC<VideoCardProps> = ({ video }) => {
  // 处理日期格式（将 Date 类型转为可读字符串）
  const formatDate = (date: Date) => {
    return new Date(date).toLocaleDateString('zh-CN');
  };

  return (
    // 卡片容器：边框、hover 背景变灰、圆角、阴影、过渡效果
    <div className="border border-gray-300 dark:border-gray-700 rounded-lg overflow-hidden bg-white dark:bg-gray-800 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors duration-200">
      {/* 缩略图区域：320*180 尺寸，顶部显示 */}
      <div className="relative w-[320px] h-[180px]">
        <img src={`${import.meta.env.VITE_BACKEND_URL}/api/Video/${video.id}/Thumbnail`}
          alt={`${video.title} 的缩略图`}
          className="w-full h-full object-cover" />
      </div>

      {/* 视频信息区域：内边距、文字换行处理 */}
      <div className="p-4 space-y-2">
        {/* 视频标题：加粗、超出省略 */}
        <h3 className="font-bold text-lg truncate">{video.title}</h3>

        {/* 视频简介：多行超出省略 */}
        <p className="text-gray-600 dark:text-gray-300 line-clamp-2 text-sm">
          {video.introduction}
        </p>

        {/* 平台链接：横向排列，超链接样式 */}
        <div className="flex gap-3 text-sm mt-2">
          {video.youtubePage && (
            <a
              href={video.youtubePage}
              target="_blank"
              rel="noopener noreferrer"
              className="text-blue-500 hover:underline"
            >
              YouTube
            </a>
          )}
          {video.niconicoPage && (
            <a
              href={video.niconicoPage}
              target="_blank"
              rel="noopener noreferrer"
              className="text-purple-500 hover:underline"
            >
              Niconico
            </a>
          )}
          {video.bilibiliPage && (
            <a
              href={video.bilibiliPage}
              target="_blank"
              rel="noopener noreferrer"
              className="text-pink-500 hover:underline"
            >
              Bilibili
            </a>
          )}
        </div>

        {/* 其他信息：网格布局展示多列信息 */}
        <div className="grid grid-cols-2 gap-2 text-xs text-gray-500 dark:text-gray-400 mt-2">
          <div>上传日期：{formatDate(video.uploadDate)}</div>
          <div>作者ID：{video.authorID}</div>
          <div>翻译状态：{video.statusTranslation}</div>
          <div>脚本状态：{video.statusScripting}</div>
          <div>硬字幕状态：{video.statusHardSubbing}</div>
          <div>授权状态：{video.authorizedPerVideo ? '已授权' : '未授权'}</div>
        </div>

        {/* 额外需求和成品链接 */}
        {video.additionalRequirement && (
          <p className="text-xs text-gray-500 dark:text-gray-400 mt-1">
            额外需求：{video.additionalRequirement}
          </p>
        )}
        {video.finishedProductLink && (
          <a
            href={video.finishedProductLink}
            target="_blank"
            rel="noopener noreferrer"
            className="text-xs text-green-500 hover:underline mt-1 block"
          >
            成品链接
          </a>
        )}
      </div>
    </div>
  );
};

export default VideoCard;
