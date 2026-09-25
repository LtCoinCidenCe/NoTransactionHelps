import { Outlet } from "react-router"
import ErrorToastProvider from "~/components/provider/ErrorToastProvider";

export async function clientLoader() {
  console.debug("GlobalLayout ClientLoader");
};

export default function globalLayout() {
  return (
    <ErrorToastProvider>
      <Outlet />
    </ErrorToastProvider>
  );
};
