import type { Route } from "./+types/videoRoute";
import VideoGrid from "~/components/video/VideoGrid";

export async function clientLoader() {
  console.debug("videoRoute ClientLoader");
}

export default function videoRoute({ loaderData }: Route.ComponentProps) {
  return <VideoGrid />
}
