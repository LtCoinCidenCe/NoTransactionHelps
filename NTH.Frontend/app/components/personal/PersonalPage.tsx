import { useContext, useState } from "react";
import UserContext from "../provider/UserContext";
import IconEditing from "./IconEditing";

const PersonalPage: React.FC = () => {
  const { currentUser, updateCurrentUser } = useContext(UserContext);
  const [displayname, setDisplayname] = useState(currentUser.displayname);

  return <div className="flex flex-col w-sm lg:w-2xl mx-auto space-y-3">
    <IconEditing />
    <div className="flex flex-row gap-4 items-stretch h-11">
      <div className="flex items-center">显示名：</div>
      <input className="grow rounded-sm border border-gray-300"
        value={displayname} onChange={e => setDisplayname(e.target.value)} />
      {displayname !== currentUser.displayname && <button className="px-5 bg-purple-300 hover:bg-purple-300/50 rounded-lg">保存</button>}
    </div>
  </div>;
};

export default PersonalPage;
