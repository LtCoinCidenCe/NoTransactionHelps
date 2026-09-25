import React from "react";
import futagotoYukari from "./futagotoYukari.png"
import { Link } from "react-router";

// 定义用户角色映射
const roleMap = [
  { mask: 0b0001, name: "游荡" },
  { mask: 0b0010, name: "翻译" },
  { mask: 0b0100, name: "打轴" },
  { mask: 0b1000, name: "管理员" },
  { mask: 0x80, name: "领主" },
];

interface UserCardProps {
  userid: number;
  username: string;
  iconid: string;
  displayname: string;
  userRole: number;
}

const UserCard: React.FC<UserCardProps> = ({ userid, username, iconid, displayname, userRole }) => {
  // 计算用户拥有的角色
  const roles = roleMap
    .filter(role => (userRole & role.mask) !== 0)
    .map(role => role.name);

  return (
    <Link to={`/userDetail/${userid}`} className="border border-gray-200 rounded-lg p-4 transition-colors duration-200 hover:bg-gray-100">
      <div className="flex items-start gap-4">
        {/* 圆形头像 */}
        <div className="flex-shrink-0">
          <img className="w-16 h-16 rounded-full object-cover"
            src={iconid === "00000000-0000-0000-0000-000000000000" ? futagotoYukari : `${import.meta.env.VITE_BACKEND_URL}/api/User/Icon/${iconid}`}
            onError={(e) => { e.currentTarget.src = futagotoYukari }}
          />
        </div>

        {/* 用户信息 */}
        <div className="flex-1 min-w-0">
          <h3 className="text-lg font-semibold text-gray-900 truncate">{displayname}</h3>
          <p className="text-sm text-gray-500 mt-1">{username}</p>

          {/* 角色标签 */}
          {roles.length > 0 && (
            <div className="flex flex-wrap gap-2 mt-2">
              {roles.map(role => (
                <span
                  key={role}
                  className="text-xs px-2 py-1 bg-emerald-100 rounded-full text-gray-700"
                >
                  {role}
                </span>
              ))}
            </div>
          )}
        </div>
      </div>
    </Link>
  );
};

export default UserCard;
