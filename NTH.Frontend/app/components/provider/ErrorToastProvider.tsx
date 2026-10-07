import React, { useState, useEffect, useCallback } from 'react';
import ErrorContext from './ErrorContext';
import SVGNTHIcons from '~/tools/SVGNTHIcons';

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
    <div className={`fixed bottom-4 left-1/2 transform -translate-x-1/2 z-75 transition-all duration-300 ease-in-out ${isShow ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-[+20px] '}`}>
      <div className="bg-red-500 text-white px-6 py-3 shadow-xl rounded-lg flex items-center gap-2 max-w-md">
        {/* 错误图标 */}
        <svg xmlns="http://www.w3.org/2000/svg" className="h-10 w-10" viewBox={SVGNTHIcons.IconParams[1].viewBox}>
          <path fill="#ffffff" d={SVGNTHIcons.IconParams[1].pathD} />
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
