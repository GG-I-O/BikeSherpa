import { IAuthService } from "@/spi/AuthSPI";
import { injectable } from "inversify";
import { Credentials } from "react-native-auth0/lib/typescript/src/core/models";
import {ApiCredentials} from "react-native-auth0";

@injectable()
export default class AuthService implements IAuthService {
    private getCredentials?: (scope?: string | undefined, minTtl?: number | undefined, parameters?: Record<string, unknown> | undefined, forceRefresh?: boolean) => Promise<Credentials>
    private getAPICredentials?: (audience: string, scope?: string, minTtl?: number, parameters?: Record<string, any>) => Promise<ApiCredentials>
    private readonly audience = process.env.EXPO_PUBLIC_AUTH_AUDIENCE;
    private readonly scope = process.env.EXPO_PUBLIC_AUTH_DEV_SCOPE;

    public setCredentialMethod(method: (params: any) => Promise<any>): void {
        this.getCredentials = method;
    }
    
    public setAPICredentialMethod(method: (params: any) => Promise<any>): void {
        this.getAPICredentials = method;
    }

    public async getToken(): Promise<string | null> {
        if (!this.getCredentials || !this.audience)
            throw new Error("AuthService not initialized");
        const credentials = await this.getCredentials(this.scope, undefined, { audience: this.audience });
        return credentials.accessToken;
    }
    
    public async verifyScope(scope: string): Promise<boolean> {
        if (!this.getAPICredentials || !this.audience)
            throw new Error("AuthService not initialized");
        const credentials = await this.getAPICredentials(this.audience, scope);
        if (!credentials.scope) return false;
        return credentials.scope.includes(scope);
    }
}