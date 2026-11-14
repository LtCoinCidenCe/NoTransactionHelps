import { useContext, useState } from "react";
import ErrorContext from "../provider/ErrorContext";
import AuthorContext from "../provider/AuthorContext";
import SVGIcons from "~/tools/SVGNTHIcons";
import AuthorCard from "./AuthorCard";

const AuthorGrid: React.FC = () => {
  const errorContext = useContext(ErrorContext);
  const { authors, setAuthors } = useContext(AuthorContext);
  const [searcher, setSearcher] = useState("");
  console.log(authors);

  return <div className="mx-auto py-4 px-10 max-w-400">
    <form className="relative mb-6"
      onSubmit={e => e.preventDefault()}>
      <svg xmlns={SVGIcons.xmlns} viewBox={SVGIcons.IconParams[0].viewBox}
        className="absolute px-2 py-2 h-full inset-y-0 left-0">
        <path fill="#c0c0c0" d={SVGIcons.IconParams[0].pathD} />
      </svg>
      <input className="pl-10 pr-4 py-2 w-full border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        type="text" placeholder="搜索作者..." value={searcher} onChange={e => setSearcher(e.target.value)} />
    </form>
    <div className="grid grid-cols-1 lg:grid-cols-2 2xl:grid-cols-3 gap-4">
      {authors.map(au=> <AuthorCard key={au.id} author={au} />)}
    </div>
  </div>;
};

export default AuthorGrid;
