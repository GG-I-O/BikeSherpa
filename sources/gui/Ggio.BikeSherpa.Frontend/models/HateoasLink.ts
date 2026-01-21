export type Link = {
    href: string;
    rel: string;
    method: string;
}

export type HateoasLinks = {
    links?: Link[];
};

export const hateoasRel = { get: "get", update: "update", delete: "delete" }