import ExportView from "~/components/ExportView";
import type { Route } from "./+types/exportRoute";

export async function clientLoader({ }: Route.ClientLoaderArgs) {
  console.log("exportRoute clientLoader");
};

export default function exportRoute({ }: Route.ComponentProps) {
  return <ExportView />;
};
