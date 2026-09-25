import { useContext, useState } from "react";
import UserContext from "../provider/UserContext";
import ErrorContext from "../provider/ErrorContext";
import UserCard from "./UserCard";

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
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 640"
          className="absolute px-2 py-2 h-full inset-y-0 left-0 flex items-center">
          <path fill="#c0c0c0" d="M480 272C480 317.9 465.1 360.3 440 394.7L566.6 521.4C579.1 533.9 579.1 554.2 566.6 566.7C554.1 579.2 533.8 579.2 521.3 566.7L394.7 440C360.3 465.1 317.9 480 272 480C157.1 480 64 386.9 64 272C64 157.1 157.1 64 272 64C386.9 64 480 157.1 480 272zM272 416C351.5 416 416 351.5 416 272C416 192.5 351.5 128 272 128C192.5 128 128 192.5 128 272C128 351.5 192.5 416 272 416z" />
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
