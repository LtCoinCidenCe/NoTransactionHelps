import { useContext, useEffect, useState } from "react";
import { AuthorBasicZod, type AuthorBasic } from "~/types";
import ErrorContext from "./ErrorContext";
import JWTContext from "./JWTContext";
import AuthorContext from "./AuthorContext";

const AuthorContextProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const errorContext = useContext(ErrorContext);
  const { jwt } = useContext(JWTContext);
  const [authors, setAuthors] = useState<AuthorBasic[]>([]);
  useEffect(() => {
    const fetchData = async () => {
      const URLPath = "/api/Author";
      const allAuthorsURL = `${import.meta.env.VITE_BACKEND_URL}${URLPath}`;
      const response = await fetch(allAuthorsURL, { method: "GET", headers: { "Authorization": `Bearer ${jwt}` } });
      if (!response.ok) {
        errorContext("数据读取失败，请刷新");
        return;
      }
      const allAuthors = await response.json();
      if (!Array.isArray(allAuthors)) {
        errorContext(`数据读取失败，请刷新。${URLPath} is not array`);
        return;
      }
      try {
        setAuthors(allAuthors.map(author => AuthorBasicZod.parse(author)));
      } catch (error) {
        errorContext(`${URLPath} doesn't have valid items`);
        return;
      }
    }
    fetchData();
    return;
  }, []); // useEffect
  return <AuthorContext value={{ authors, setAuthors }}>{children}</AuthorContext>;
};

export default AuthorContextProvider;
