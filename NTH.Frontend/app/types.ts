import zod from "zod";

export const JwtClaimsSchema = zod.object({
    exp: zod.number(),
    iss: zod.string(),
    aud: zod.string(),
    sub: zod.string().optional(),
    iat: zod.number().optional(),
})

export type JWTPayload = zod.infer<typeof JwtClaimsSchema>

export const isJWTPayload = (candidate: any): candidate is JWTPayload =>
    JwtClaimsSchema.safeParse(candidate).success


export const VideoInfoZod = zod.object({
    id: zod.number(),
    title: zod.string(),
    introduction: zod.string(),
    youtubePage: zod.string(),
    niconicoPage: zod.string(),
    bilibiliPage: zod.string(),
    authorID: zod.number(),
    authorizedPerVideo: zod.boolean(),
    uploadDate: zod.coerce.date(),
    statusTranslation: zod.number(),
    statusScripting: zod.number(),
    statusHardSubbing: zod.number(),
    additionalRequirement: zod.string(),
    finishedProductLink: zod.string(),
})

export type VideoInfo = zod.infer<typeof VideoInfoZod>;

export const isVideoInfo = (candidate: any): candidate is VideoInfo =>
    VideoInfoZod.safeParse(candidate).success


export const UserBasicZod = zod.object({
    id: zod.number(),
    username: zod.string(),
    displayname: zod.string(),
    userIconID: zod.uuid(),
    titleWords: zod.string(),
    userRole: zod.number(),
    creationDate: zod.coerce.date(),
})

export type UserBasic = zod.infer<typeof UserBasicZod>;

export const isUserBasic = (candidate: any): candidate is UserBasic =>
    UserBasicZod.safeParse(candidate).success

export const User0: UserBasic = { id: 0, username: "", displayname: "", userIconID: "00000000-0000-0000-0000-000000000000", titleWords: "", userRole: 0, creationDate: new Date() };


export const AuthorContactItemZod = zod.object({
    id: zod.number(),
    userID: zod.number(),
    byUserAudit: zod.number(),
    changeDate: zod.coerce.date()
});

export type AuthorContactItem = zod.infer<typeof AuthorContactItemZod>;

export const AuthorBasicZod = zod.object({
    id: zod.number(),
    name: zod.string(),
    youtubeHomePage: zod.string(),
    niconicoHomePage: zod.string(),
    bilibiliHomePage: zod.string(),
    twitterHomePage: zod.string(),
    authorizedPerVideo: zod.boolean(),
    allVideoAuthorized: zod.boolean(),
    authorizationChangeDate: zod.coerce.date(),
    additionalRequirements: zod.string(),
    additionalRequirementsChangeDate: zod.coerce.date(),
    creationDate: zod.coerce.date(),
    contact: zod.array(AuthorContactItemZod),
});

export type AuthorBasic = zod.infer<typeof AuthorBasicZod>;

export const isAuthorBasic = (candidate: any): candidate is AuthorBasic =>
    AuthorBasicZod.safeParse(candidate).success;

export const LiaoTianMessageZod = zod.object({
    id: zod.number(),
    userID: zod.number(),
    receivedTime: zod.coerce.date(),
    words: zod.string(),
    revoked: zod.boolean(),
});

export type LiaoTianMessage = zod.infer<typeof LiaoTianMessageZod>;

export const LiaoTianJiLuZod = zod.array(LiaoTianMessageZod);

export type LiaoTianJiLu = zod.infer<typeof LiaoTianJiLuZod>;
