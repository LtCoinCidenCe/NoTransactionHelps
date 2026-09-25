import type { Route } from "./+types/loginRoute";
import LoginForm from "~/components/loginForm/loginForm";

export async function clientLoader({ }: Route.ClientLoaderArgs) {
  let NTHUsername = localStorage.getItem("NTHUsername");
  let NTHPassword = localStorage.getItem("NTHPassword");
  if (NTHUsername === null)
    NTHUsername = "";
  if (NTHPassword === null)
    NTHPassword = "";

  return { username: NTHUsername, password: NTHPassword };
}

export default function Login({ loaderData }: Route.ComponentProps) {
  return (
    <>
      <title>登录</title>
      <meta name="description" content="登录到翻译组管理" />
      <LoginForm username={loaderData.username} password={loaderData.password} />
    </>);
}
