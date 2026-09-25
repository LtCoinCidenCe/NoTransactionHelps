import { Outlet } from "react-router";
import type { Route } from "./+types/userRoute";
import UserGrid from "~/components/user/UserGrid";

export async function clientLoader({ }: Route.ClientActionArgs) {
  console.debug("userRoute clientLoader");
}

export default function userRoute({ loaderData, matches, params, actionData }: Route.ComponentProps) {
  const [root, global, workspace, user] = matches;
  return <><UserGrid /><Outlet /></>;
}
