import { useContext } from "react";
import ErrorContext from "../provider/ErrorContext";
import AuthorContext from "../provider/AuthorContext";
import SVGIcons from "~/tools/SVGNTHIcons";

const AuthorGrid: React.FC = () => {
  const errorContext = useContext(ErrorContext);
  const { authors, setAuthors } = useContext(AuthorContext);
  console.log(authors);
  return <div className="mx-auto p-4 max-w-400">
    <form className="relative mb-6">
      <svg xmlns={SVGIcons.xmlns} viewBox={SVGIcons.IconParams[0].viewBox}
        className="absolute px-2 py-2 h-full inset-y-0 left-0">
        <path fill="#c0c0c0" d={SVGIcons.IconParams[0].pathD} />
      </svg>
      <input className="pl-10 pr-4 py-2 w-full border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        type="text" placeholder="搜索作者..." />
    </form>
  </div>;
};

export default AuthorGrid;
