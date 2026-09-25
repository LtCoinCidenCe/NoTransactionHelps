import type { Route } from "./+types/authorRoute";
import AuthorGrid from "~/components/author/AuthorGrid";

export async function clientLoader({ }: Route.ClientActionArgs) {
}

export default function authorRoute({ loaderData }: Route.ComponentProps) {
  return <AuthorGrid />;
}
