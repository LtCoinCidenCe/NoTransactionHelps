import { useContext, useState } from "react";
import UserContext from "../provider/UserContext";
import ErrorContext from "../provider/ErrorContext";
import UserCard from "./UserCard";
import SVGIcons from "~/tools/SVGNTHIcons";

const UserGrid: React.FC = () => {
  const errorContext = useContext(ErrorContext);
  const { users, setUsers } = useContext(UserContext);
  const [filterText, setFilterText] = useState('');

  const fa = filterText.toLowerCase();

  // 备用
  const filterFormSubmit: React.SubmitEventHandler<HTMLFormElement> = (event) => {
    event.preventDefault();
  }

  return (
    <div className="container mx-auto p-4">
      {/* 过滤输入框 */}
      <form className="relative mb-6" onSubmit={filterFormSubmit}>
        <svg xmlns={SVGIcons.xmlns} viewBox={SVGIcons.IconParams[0].viewBox}
          className="absolute px-2 py-2 h-full inset-y-0 left-0">
          <path fill="#c0c0c0" d={SVGIcons.IconParams[0].pathD} />
        </svg>
        <input
          type="text"
          placeholder="搜索用户..."
          value={filterText}
          onChange={(e) => setFilterText(e.target.value)}
          className="pl-10 pr-4 py-2 w-full border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        />
      </form>

      {/* 用户卡片网格 */}
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        {users.filter(x => x.username.toLowerCase().includes(fa) || x.displayname.toLowerCase().includes(fa))
          .map((user) => (
            <UserCard key={user.id} userid={user.id} username={user.username} iconid={user.userIconID}
              displayname={user.displayname} userRole={user.userRole} />
          ))}
      </div>
    </div>
  );
};

export default UserGrid;
