import type { Route } from "./+types/homeRoute";
import { Welcome } from "../components/welcome/welcome";

// replaced by <title> & <meta>
// export function meta({ }: Route.MetaArgs) {
//   return [
//     { title: "Home page" },
//     { name: "description", content: "This is the home page." },
//   ];
// }

export async function clientLoader() {
}

export default function Home({ loaderData }: Route.ComponentProps) {
  return (
    <div>
      <title>Home page</title>
      <meta name="description" content="This is the home page." />
      <Welcome />
    </div>);
}
