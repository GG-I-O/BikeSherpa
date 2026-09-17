type AttachmentFile = {
    path: string,
    domainType: attachmentDomainTypes;
}

enum attachmentDomainTypes {
    "signature" = "signature",
    "photo" = "photo",
    "document" = "document"
}

export {AttachmentFile, attachmentDomainTypes};