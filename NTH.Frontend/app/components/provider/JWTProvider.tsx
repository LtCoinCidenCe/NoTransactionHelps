import { useContext, useEffect, useMemo, useState } from "react";
import ErrorContext from "./ErrorContext";
import JWTContext from "./JWTContext";
import { parseJWT } from "~/tools/JWTParser";

const JWTProvider: React.FC<{ loaderJWT: string, children: React.ReactNode }> = ({ loaderJWT, children }) => {
  const errorContext = useContext(ErrorContext);
  const [jwtfeed, setjwtfeed] = useState(loaderJWT);
  const [isReady, setReady] = useState(false);

  useEffect(() => {
    const parsedInfo = parseJWT(loaderJWT);
    if (!parsedInfo)
      throw new Error("parsing JWT not successful");
    const { expireTime, userIdentifier } = parsedInfo;
    let remaining = expireTime.valueOf() - new Date().valueOf();
    if (remaining > 15 * 1000) {
      // 15 seconds to do all initial loading
      setReady(true);
    }
    let initialRefreshTimer = Math.max(remaining - 1 * 60 * 1000, 0);
    console.debug(remaining);


    // 通过闭包保存变量，在useEffect返回清理函数中打false
    let isMounted = true;
    let updater = 0;

    // periodic relogin
    const relogin = async () => {
      if (!isMounted)
        return;
      // retrieve username and password every time in case the user logs out.
      let NTHUsername = localStorage.getItem("NTHUsername");
      let NTHPassword = localStorage.getItem("NTHPassword");
      if (NTHUsername === null || NTHPassword === null) {
        return;
      }
      const userLoginDTO = { Username: NTHUsername, Password: NTHPassword };
      try {
        const fetched = await fetch(`${import.meta.env.VITE_BACKEND_URL}/api/Login`,
          {
            method: "POST", headers: { "Content-Type": "application/json" },
            body: JSON.stringify(userLoginDTO), credentials: "include"
          });
        const token = await fetched.text();
        if (fetched.status >= 500) {
          errorContext("server error");
          return;
        }
        if (fetched.status !== 200) {
          errorContext("用户名或密码不正确");
          return;
        }
        console.log("update jwt");
        if (!isMounted) // super edge case because of await
          return;
        localStorage.setItem("appjwt", token);
        setjwtfeed(token);
        setReady(true);
      }
      catch (error) {
        errorContext(`${error}`);
        return;
      }
      // if there is no problem then schedule next login
      updater = Number(setTimeout(relogin, 14 * 60 * 1000));
    };




    // run once after login
    updater = Number(setTimeout(relogin, initialRefreshTimer));

    return () => {
      isMounted = false;
      clearTimeout(updater);
      console.log('定时登录器已清除，组件卸载');
    };
  }, []); // useEffect

  const food = useMemo(() => {
    const info = parseJWT(jwtfeed);
    if (!info)
      throw new Error("parsing JWT not successful");
    const { expireTime, userIdentifier } = info;
    // console.debug("JWTParsed");
    return { jwt: jwtfeed, expireTime, userIdentifier };
  }, [jwtfeed]); // useMemo

  // console.debug("JWTProvider");

  return <JWTContext value={food}>{isReady ? children : undefined}</JWTContext>;
  // 这里这个isReady是挡不住子路径的clientLoader的，但是能挡住控件
};

export default JWTProvider;
