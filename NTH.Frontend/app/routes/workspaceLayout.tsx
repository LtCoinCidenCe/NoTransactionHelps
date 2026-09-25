import { Outlet, redirect } from "react-router";
import type { Route } from "./+types/workspaceLayout";
import WorkspaceHeader from "~/components/WorkspaceHeader";
import JWTProvider from "~/components/provider/JWTProvider";
import UserContextProvider from "~/components/provider/UserContextProvider";
import AuthorContextProvider from "~/components/provider/AuthorContextProvider";
import { parseJWT } from "~/tools/JWTParser";

export async function clientLoader({ }: Route.ClientLoaderArgs) {
  let NTHUsername = localStorage.getItem("NTHUsername");
  let NTHPassword = localStorage.getItem("NTHPassword");
  let loaderJWT = localStorage.getItem("appjwt");
  if (NTHUsername === null || NTHPassword === null || loaderJWT === null) {
    return redirect("login");
  }
  const parsedInfo = parseJWT(loaderJWT);
  if (!parsedInfo)
    return redirect("login");
  const { expireTime, userIdentifier } = parsedInfo;

  return { loaderJWT, NTHUsername, userIdentifier };
};

export default function workspaceLayout({ loaderData }: Route.ComponentProps) {
  return (
    <JWTProvider loaderJWT={loaderData.loaderJWT}>
      <UserContextProvider>
        <AuthorContextProvider>
          <WorkspaceHeader NTHUsername={loaderData.NTHUsername} userIdentifier={loaderData.userIdentifier} />
          <Outlet />
        </AuthorContextProvider>
      </UserContextProvider>
    </JWTProvider>
  );
}
