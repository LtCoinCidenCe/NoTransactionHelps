import LiaoTian from "~/components/liaoTian/LiaoTian";
import type { Route } from "./+types/liaoTianRoute";

export async function clientLoader({ }: Route.ClientLoaderArgs) {
}

export default function liaoTianRoute({ }: Route.ComponentProps) {
  return <LiaoTian/>;
}
