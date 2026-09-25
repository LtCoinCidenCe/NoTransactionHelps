import React, { useState, useEffect, useCallback } from 'react';
import ErrorContext from './ErrorContext';

// 错误提示组件（全局唯一，通过状态控制显示）
const ErrorToast: React.FC<{ message: string, isShow: boolean, onClose: () => void }> = ({ message, isShow, onClose }) => {
  // 定时消失：默认5秒后关闭
  useEffect(() => {
    if (isShow) {
      const timer = setTimeout(() => {
        onClose();
      }, 5000);
      return () => clearTimeout(timer); // 组件卸载时清除定时器
    }
  }, [message, isShow, onClose]);

  return (
    // 固定在顶部居中，不占用文档流，不拦截点击事件
    <div
      className={`fixed bottom-4 left-1/2 transform -translate-x-1/2 z-75 transition-all duration-300 ease-in-out ${isShow ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-[+20px] pointer-events-none'}`}
      style={{ pointerEvents: 'none' }} // 关键：不占用点击焦点，底层元素可正常点击
    >
      <div className="bg-red-500 text-white px-6 py-3 rounded-lg shadow-lg flex items-center gap-2 max-w-md">
        {/* 错误图标 */}
        <svg xmlns="http://www.w3.org/2000/svg" className="h-10 w-10" viewBox="0 0 640 640">
          <path fill="#ffffff" d="M320 576C178.6 576 64 461.4 64 320C64 178.6 178.6 64 320 64C461.4 64 576 178.6 576 320C576 461.4 461.4 576 320 576zM320 384C302.3 384 288 398.3 288 416C288 433.7 302.3 448 320 448C337.7 448 352 433.7 352 416C352 398.3 337.7 384 320 384zM320 192C301.8 192 287.3 207.5 288.6 225.7L296 329.7C296.9 342.3 307.4 352 319.9 352C332.5 352 342.9 342.3 343.8 329.7L351.2 225.7C352.5 207.5 338.1 192 319.8 192z" />
        </svg>
        {/* 错误信息（自动换行） */}
        <span className="text-sm font-medium">{message}</span>
      </div>
    </div>
  );
};

// 全局状态管理（可集成到根组件，或用 Context 全局调用）
const ErrorToastProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [errorState, setErrorState] = useState({
    isShow: false,
    message: '',
  });

  const closeErrorToast = useCallback(() => {
    setErrorState({ isShow: false, message: errorState.message });
  }, []);

  // 全局调用方法：外部组件可通过 props/Context 调用
  const showErrorToast = useCallback((message: string) => {
    setErrorState({ isShow: true, message });
  }, []);

  return (
    <ErrorContext value={showErrorToast}>
      {/* 错误提示组件挂载在根节点，全局唯一 */}
      <ErrorToast
        message={errorState.message}
        isShow={errorState.isShow}
        onClose={closeErrorToast}
      />
      {/* 业务组件（通过 Context 传递 showErrorToast 供子组件调用） */}
      <div>{children}</div>
    </ErrorContext>
  );
};

export default ErrorToastProvider;
