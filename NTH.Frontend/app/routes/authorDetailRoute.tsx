import type { Route } from "./+types/authorDetailRoute";

export async function clientLoader({ params }: Route.ClientLoaderArgs) {
  console.debug("authorDetailRoute:", params.id)
}

export default function authorDetailRoute({ params }: Route.ComponentProps) {
  return <div>{params.id}</div>
}
