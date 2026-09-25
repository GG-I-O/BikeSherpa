import {attachmentDomainTypes} from "@/models/AttachmentFile";

type UploadableFile = {
    uri: string;
    name: string;
    mimeType: string;
    domainType: attachmentDomainTypes;
}

export default UploadableFile;