export type Link = {
    href: string;
    rel: string;
    method: string;
}

export type HateoasLinks = {
    links?: Link[];
};

export const hateoasRel = {
    get: "get",
    update: "update",
    delete: "delete",
    patch: "patch",
    stepCourier: {
        post: "postCourier",
        delete: "deleteCourier",
    },
    stepOrder: {
        put: "putOrder"
    },
    stepTime: {
        put: "putTime"
    }
}