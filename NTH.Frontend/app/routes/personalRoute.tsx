import type { Route } from "./+types/personalRoute";
import PersonalPage from "~/components/personal/PersonalPage";

export async function clientLoader({ }: Route.ClientLoaderArgs) {
}

export default function personalRoute({ }: Route.ComponentProps) {
  return <PersonalPage />;
}
