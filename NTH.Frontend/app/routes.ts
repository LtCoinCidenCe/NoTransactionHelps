import { type RouteConfig, index, layout, route } from "@react-router/dev/routes";

export default [
    layout("routes/globalLayout.tsx", [

        route("login", "routes/loginRoute.tsx"),

        layout("routes/workspaceLayout.tsx", [
            index("routes/homeRoute.tsx"),
            route("user", "routes/userRoute.tsx"),
            route("userDetail/:id", "routes/userDetailRoute.tsx"),
            route("author", "routes/authorRoute.tsx", [
                route(":id", "routes/authorDetailRoute.tsx")
            ]),
            route("video", "routes/videoRoute.tsx"),
            route("export","routes/exportRoute.tsx"),
            route("personal","routes/personalRoute.tsx"),
            route("liaoTian","routes/liaoTianRoute.tsx"),
        ]),
    ])
] satisfies RouteConfig;
