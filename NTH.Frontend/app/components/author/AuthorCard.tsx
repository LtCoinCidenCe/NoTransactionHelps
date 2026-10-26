import { useContext } from "react";
import type { AuthorBasic } from "~/types";
import UserContext from "../provider/UserContext";
import masterText from "./masterText.png";
import futagotoYukari from "../user/futagotoYukari.png";

const AuthorCard: React.FC<{ author: AuthorBasic }> = ({ author }) => {
  const { usersMap } = useContext(UserContext);
  const contactUser = usersMap.get(author.contact ?? 0);
  return <div className="border border-gray-200 rounded-lg p-4 transition-colors duration-200 hover:bg-gray-100 dark:hover:bg-gray-900 min-w-110 min-h-50 flex flex-row gap-4">
    <img className="w-16 h-16 rounded-full"
      src={author.authorIconID === "00000000-0000-0000-0000-000000000000" ? masterText : `${import.meta.env.VITE_BACKEND_HOST}/api/Author/Icon/${author.authorIconID}`}
      onError={(e) => e.currentTarget.src = masterText} />
    <div className="flex flex-col gap-2">
      <h3 className="text-lg font-semibold truncate mb-2">{author.name}</h3>
      <div className="flex flex-row items-center gap-2">
        <span>联系人</span>
        <img className="w-8 h-8 rounded-full"
          src={contactUser ? (contactUser.userIconID === "00000000-0000-0000-0000-000000000000" ? futagotoYukari : `${import.meta.env.VITE_BACKEND_HOST}/api/User/Icon/${contactUser.userIconID}`) : futagotoYukari}
          onError={(e) => { e.currentTarget.src = futagotoYukari }}
          title={contactUser ? contactUser.displayname : "无人认领"}
        />
      </div>
      <h4 className="text-md truncate">{author.niconicoHomePage ?? author.youtubeHomePage ?? author.bilibiliHomePage}</h4>
      <div className="flex flex-row">
        {author.allVideoAuthorized && <div className="bg-emerald-100 rounded-full text-gray-700 w-fit p-1">全部授权</div>}
        {author.authorizedPerVideo && <div className="bg-emerald-100 rounded-full text-gray-700 w-fit p-1">部分授权</div>}
      </div>
    </div>
  </div>
}

export default AuthorCard;
