import UploadableFile from "@/models/UploadableFile";
import {Platform} from "react-native";

export default class AttachmentFileService {
    public static async appendFileToFormData(formData: FormData, fieldName: string, file: UploadableFile): Promise<void> {
        if (Platform.OS === "web") {
            const response = await fetch(file.uri);
            const blob = await response.blob();

            formData.append(fieldName, blob, file.name);
            return;
        }

        formData.append(fieldName, {
            uri: file.uri,
            type: file.type,
            name: file.name
        } as unknown as Blob);
    }
}