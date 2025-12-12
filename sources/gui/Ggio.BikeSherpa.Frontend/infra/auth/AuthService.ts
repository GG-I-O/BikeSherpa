import { IAuthService } from "@/spi/AuthSPI";
import { injectable } from "inversify";
import { Credentials } from "react-native-auth0/lib/typescript/src/core/models";

@injectable()
export default class AuthService implements IAuthService {
    private getCredentials?: (scope?: string | undefined, minTtl?: number | undefined, parameters?: Record<string, unknown> | undefined, forceRefresh?: boolean) => Promise<Credentials>

    public setCredentialMethod(method: (params: any) => Promise<any>): void {
        this.getCredentials = method;
    }
    public async getToken(): Promise<string | null> {
        if (!this.getCredentials)
            throw new Error("AuthService not initialized");
        const credentials = await this.getCredentials();
        return credentials.accessToken;
    }
}