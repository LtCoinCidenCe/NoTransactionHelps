import UserDetail from "~/components/user/UserDetail";
import type { Route } from "./+types/userDetailRoute";

export async function clientLoader({ params }: Route.ClientLoaderArgs) {
  const userID = Number.parseInt(params.id);
  if (Number.isNaN(userID))
    throw new Error("that's not a userID");
  return userID;
}

export default function userDetailRoute({ params, loaderData }: Route.ComponentProps) {
  return <UserDetail userID={loaderData} />
}
